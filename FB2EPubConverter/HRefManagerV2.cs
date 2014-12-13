﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EPubLibrary;
using EPubLibrary.XHTML_Items;
using FB2Library.Elements;
using XHTMLClassLibrary.BaseElements;
using XHTMLClassLibrary.BaseElements.BlockElements;
using XHTMLClassLibrary.BaseElements.InlineElements;
using EPubLibrary.ReferenceUtils;
using XHTMLClassLibrary.BaseElements.InlineElements.TextBasedElements;

namespace FB2EPubConverter
{
    internal class HRefManagerV2 : IHRefManager
    {
        /// <summary>
        /// List of IDs 
        /// </summary>
        private readonly Dictionary<string, IHTMLItem> _ids = new Dictionary<string, IHTMLItem>();


        /// <summary>
        /// List of references
        /// </summary>
        private readonly Dictionary<string, List<Anchor>> _references = new Dictionary<string, List<Anchor>>();


        /// <summary>
        /// List of references
        /// </summary>
        private readonly Dictionary<string, List<Image>> _images = new Dictionary<string, List<Image>>();

        /// <summary>
        /// List of remapped attributes 
        /// </summary>
        private readonly Dictionary<string, string> _attributesRemap = new Dictionary<string, string>();

        /// <summary>
        /// Resets the manager to empty state,
        /// releases all references etc
        /// </summary>
        public void Reset()
        {
            _ids.Clear();
            _references.Clear();
            _attributesRemap.Clear();
            _images.Clear();
        }

        public string AddImageRefferenced(InlineImageItem item, Image img)
        {
            if (item == null)
            {
                return string.Empty;
            }
            string validName = MakeValidImageName(item.HRef);
            if (!_images.ContainsKey(validName))
            {
                var entryList = new List<Image>();
                _images.Add(validName, entryList);
            }
            _images[validName].Add(img);
            return ReferencesUtils.FormatImagePath(validName,FlatStructure);
        }

        public string AddImageRefferenced(ImageItem item, Image img)
        {
            if (item == null)
            {
                return string.Empty;
            }
            string validName = MakeValidImageName(item.HRef);
            
            if (!_images.ContainsKey(validName))
            {
               var entryList = new List<Image>();
                _images.Add(validName, entryList);
            }
            _images[validName].Add(img);
            return ReferencesUtils.FormatImagePath(validName, FlatStructure);
        }


        private static string MakeValidImageName(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return string.Empty;
            }
            string result = id;
            if (id.StartsWith("#") && id.Length > 1)
            {
                result = id.Substring(1);
            }
            return result;
        }

        /// <summary>
        /// Add used ID to the list
        /// </summary>
        /// <param name="id">Id to list as used</param>
        /// <param name="item">item that ID belong to</param>
        /// <returns>returns and registers the id if available, empty string if ID already exists</returns>
        public string AddIdUsed(string id,IHTMLItem item)
        {
            if (string.IsNullOrEmpty(id))
            {
                return string.Empty;
            }
            string newid = EnsureGoodId(id);
            if (_ids.ContainsKey(newid))
            {
                // we get here if in file same ID used twice
                // The assumption here that since the text located in FB2 main body we should not have same ID used more than once
                // otherwise we would not know how (and where to) go back
                Logger.Log.InfoFormat("item with ID {0} already defined, ignoring to avoid inconsistences", newid);
                return string.Empty;
            }
            _ids.Add(newid, item);
            return newid;
        }

        /// <summary>
        /// Checks if ID already used
        /// </summary>
        /// <param name="id">ID to check</param>
        /// <returns>true if used, false otherwise</returns>
        private bool IsIdUsed(string id)
        {
            if ( id.StartsWith("#") && (id.Length > 1))
            {
                id = id.Substring(1);
            }
            if (_ids.ContainsKey(EnsureGoodId(id)))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Adds reference 
        /// Used to add hyperlinks (anchors) from one part of the document to another
        /// </summary>
        /// <param name="reference">reference name (name of the link we going to "jump to" - href)</param>
        /// <param name="anchor">anchor element that contains the reference (place we "jump from")</param>
        public void AddReference(string reference, Anchor anchor)
        {
            if (!ReferencesUtils.IsExternalLink(reference)) // we count only "internal" references (no need for http://... )
            {
                reference = EnsureGoodReference(reference); // make sure that reference name is valid according to HTML rules

                List<Anchor> list;
                if (!_references.ContainsKey(reference)) // if this a first time we see "jump" to this ID
                {
                    list = new List<Anchor>(); //allocate new list of referenced objects
                    _references.Add(reference, list); // add list to dictionary
                }
                else // if we already have at least one reference to this ID
                {
                    list = _references[reference]; // find this ID in references dictionary
                }
                if (string.IsNullOrEmpty((string)anchor.GlobalAttributes.ID.Value)) // if anchor does not have ID already (FB2 link element does not have ID so this is just in case), we need this for backlinking
                {
                    string backLink = string.Format("{0}_back", reference); // generate back link
                    if (backLink.StartsWith("#"))
                    // most references starts with # as part of the href linking, so we need to strip this character to get valid ID 
                    {
                        backLink = backLink.Substring(1);
                    }
                    while (IsIdUsed(backLink))
                    {
                        backLink += "x";
                    }
                    anchor.GlobalAttributes.ID.Value = backLink;
                }
                anchor.GlobalAttributes.ID.Value = AddIdUsed((string)anchor.GlobalAttributes.ID.Value, anchor);              
                list.Add(anchor);
            }
        }


        public void AddBackReference(string reference, Anchor anchor)
        {
            if (!ReferencesUtils.IsExternalLink(reference)) // we count only "internal" references (no need for http://... )
            {
                reference = EnsureGoodReference(reference); // make sure that reference name is valid according to HTML rules

                List<Anchor> list;
                if (!_references.ContainsKey(reference)) // if this a first time we see "jump" to this ID
                {
                    list = new List<Anchor>(); //allocate new list of referenced objects
                    _references.Add(reference, list); // add list to dictionary
                }
                else // if we already have at least one reference to this ID
                {
                    list = _references[reference]; // find this ID in references dictionarry
                }
                list.Add(anchor);
            }
        }


        /// <summary>
        /// Processes all the references and removes invalid anchors
        /// Invalid meaning pointing to non-existing IDs
        /// only "internal" anchors are removed
        /// </summary>
        public void RemoveInvalidAnchors()
        {
            var listToRemove = new List<string>();
            foreach (var reference in _references)
            {
                // If reference does not points on one of valid IDs and it's not external reference
                if (!IsIdUsed(reference.Key) && !ReferencesUtils.IsExternalLink(reference.Key))
                {
                    listToRemove.Add(reference.Key);
                    // remove all references to this ID
                    foreach (var element in _references[reference.Key])
                    {
                        // The Anchor element can't have empty reference so
                        // we remove it and in case it has some meaningful content
                        // replace with span that is meaningless non-block element
                        // so contained text etc are kept
                        if (element.SubElements().Count != 0)
                        {
                            var spanElement = new Span(element.HTMLStandard);
                            foreach (var subElement in element.SubElements())
                            {
                                spanElement.Add(subElement);
                            }
                            if (element.Parent != null)
                            {
                                int index = element.Parent.SubElements().IndexOf(element);
                                if (index != -1)
                                {
                                    spanElement.Parent = element.Parent;
                                    element.Parent.SubElements().Insert(index, spanElement);
                                }                                
                            }
                            if (!string.IsNullOrEmpty((string)element.GlobalAttributes.ID.Value))
                            {
                                spanElement.GlobalAttributes.ID.Value = element.GlobalAttributes.ID.Value; // Copy ID anyway - may be someone "jumps" here
                                _ids[(string)element.GlobalAttributes.ID.Value] = spanElement;     // and update the "pointer" to element                           
                            }
                            spanElement.GlobalAttributes.Class.Value = "ex_bad_link";
                        }
                        if (element.Parent != null)
                        {
                            element.Parent.Remove(element);   
                        }
                    }                                     
                }
            }
            // Remove the unused anchor (and their ID if they have one)
            // from the lists
            foreach (var toRemove in listToRemove)
            {
                _references.Remove(toRemove);
            }
        }


        /// <summary>
        /// Ensures that all IDs are valid
        /// This still require remapping the references to updated IDs
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string EnsureGoodId(string id)
        {
            if (id == null)
            {
                return "";
            }
            if (id.Length == 0)
            {
                return id;
            }
            if (_attributesRemap.Keys.Any(x => x == id))
            {
                return _attributesRemap[id];
            }
            bool remaped = false;
            var res = new StringBuilder();
            var pattern = new Regex(@"[^a-zA-Z]");
            if (pattern.IsMatch(id[0].ToString(CultureInfo.InvariantCulture)))
            {
                res.Append("ID");
                remaped = true;
            }
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_";
            foreach (var character in id)
            {
                if (validChars.Contains(character.ToString(CultureInfo.InvariantCulture)))
                {
                    res.Append(character);
                }
                else
                {
                    remaped = true;
                    res.Append("_");
                }
            }
            if (remaped)
            {
                while (_attributesRemap.Keys.Any(x => x == id))
                {
                    res.Append(new Random().Next(0, 9).ToString(CultureInfo.InvariantCulture));
                }
                _attributesRemap.Add(id, res.ToString());
            }
            return res.ToString();
        }

        public string EnsureGoodReference(string reference)
        {
            if ((string.IsNullOrEmpty(reference)) || (reference[0] != '#') || (reference.Length < 2))
            {
                return reference;
            }
            string subRef = reference.Substring(1);
            return string.Format("#{0}", EnsureGoodId(subRef));
        }


        public void RemapAnchors(EPubFile epubFile)
        {
            foreach (var link in _references)
            {
                if (!ReferencesUtils.IsExternalLink(link.Key))
                {
                    string idString = ReferencesUtils.GetIdFromLink(link.Key);
                    BaseXHTMLFile iDDocument = GetIDParentDocument(epubFile, _ids[idString]);
                    if (iDDocument != null)
                    {
                        int count = 0;
                        foreach (var anchor in link.Value)
                        {
                            BaseXHTMLFile idDocument = GetIDParentDocument(epubFile, anchor);
                            var referencedItem = _ids[(string)anchor.HRef.Value];
                            var newParent = DetectParentContainer(referencedItem);
                            if (newParent == null)
                            {
                                continue;
                            }
                            var newAnchor = new Anchor(newParent.HTMLStandard);
                            if (idDocument == iDDocument)
                            {
                                anchor.HRef.Value = string.Format("#{0}", idString);
                                newAnchor.HRef.Value = string.Format("#{0}", anchor.GlobalAttributes.ID.Value);
                            }
                            else
                            {
                                anchor.HRef.Value = string.Format("{0}#{1}", iDDocument.FileName, idString);
                                if (idDocument == null)
                                {
                                    continue;
                                }
                                newAnchor.HRef.Value = string.Format("{0}#{1}", idDocument.FileName, anchor.GlobalAttributes.ID.Value);
                            }
                            if ((iDDocument is BookDocument) && ((iDDocument as BookDocument).Type == SectionTypeEnum.Links))  // if it's FBE notes section
                            {
                                newAnchor.GlobalAttributes.Class.Value = "note_anchor";
                                newParent.Add(new EmptyLine(newParent.HTMLStandard));
                                newParent.Add(newAnchor);
                                count++;
                                newAnchor.Add(new SimpleHTML5Text(newAnchor.HTMLStandard) { Text = (link.Value.Count > 1) ? string.Format("(<< back {0})  ", count) : string.Format("(<< back)  ") });
                            }
                        }
                    }
                    else
                    {
                        //throw new Exception("Internal consistency error - Used ID has to be in one of the book documents objects");
                        Logger.Log.Error("Internal consistency error - Used ID has to be in one of the book documents objects");
                        //continue;
                    }
                }
            }
        }

        /// <summary>
        /// Detect parent container of the element
        /// </summary>
        /// <param name="referencedItem"></param>
        /// <returns></returns>
        private static IHTMLItem DetectParentContainer(IHTMLItem referencedItem)
        {
            if (referencedItem is IBlockElement)
            {
                return referencedItem;
            }
            if (referencedItem.Parent is IBlockElement)
            {
                return referencedItem.Parent;
            }
            return null;
        }



        private BaseXHTMLFile GetIDParentDocument(EPubFile file, IHTMLItem value)
        {
            if ((file.AnnotationPage != null) && file.AnnotationPage.PartOfDocument(value))
            {
                return file.AnnotationPage;
            }
            return file.BookDocuments.FirstOrDefault(document => document.PartOfDocument(value));
        }


        public void RemoveInvalidImages(Dictionary<string, BinaryItem> dictionary)
        {
            foreach (var imageId in _images.Keys)
            {
                if (!dictionary.ContainsKey(imageId))
                {
                    foreach (var element in _images[imageId])
                    {
                        if (element.Parent != null)
                        {
                            element.Parent.Remove(element);                            
                        }
                    }
                }
            }
        }


        public bool FlatStructure { get; set; }
    }
}
