﻿using System;
using System.Collections.Generic;
using System.Linq;
using FB2Library.Elements;
using XHTMLClassLibrary.BaseElements;
using XHTMLClassLibrary.BaseElements.InlineElements;
using XHTMLClassLibrary.BaseElements.InlineElements.TextBasedElements;

namespace FB2EPubConverter.ElementConverters
{
    internal class SimpleTextElementConverterParams
    {
        public ConverterOptions Settings { get; set; }  
        public bool NeedToInsertDrop { get; set; }
    }

    internal class SimpleTextElementConverter : BaseElementConverter
    {
        /// <summary>
        /// Converts FB2 simple text 
        /// ( simple text is normal text or text with one of the "styles")
        /// </summary>
        /// <param name="styletypeItem">item to convert</param>
        /// <param name="compatibility"></param>
        /// <param name="simpleTextElementConverterParams"></param>
        /// <returns></returns>
        public List<IHTMLItem> Convert(StyleType styletypeItem,HTMLElementType compatibility,SimpleTextElementConverterParams simpleTextElementConverterParams)
        {

            if (styletypeItem == null)
            {
                throw new ArgumentNullException("styletypeItem");
            }

            var list = new List<IHTMLItem>();

            if (styletypeItem is SimpleText)
            {
                var text = styletypeItem as SimpleText;
                switch (text.Style)
                {
                    case FB2Library.Elements.TextStyles.Normal:
                        if (text.HasChildren)
                        {
                            list.AddRange(text.Children.SelectMany(x=>Convert(x,compatibility,simpleTextElementConverterParams)));
                        }
                        else
                        {
                            if (simpleTextElementConverterParams.NeedToInsertDrop && text.Text.Length > 0)
                            {
                                AddAsDrop(list,text,compatibility);
                            }
                            else
                            {
                                list.Add(new SimpleHTML5Text(compatibility) { Text = text.Text });
                            }
                        }
                        break;
                    case FB2Library.Elements.TextStyles.Code:
                        var code = new CodeText(compatibility);
                        if (text.HasChildren)
                        {
                            foreach (var item in text.Children.SelectMany(x=>Convert(x,compatibility,simpleTextElementConverterParams)))
                            {
                                code.Add(item);
                            }
                        }
                        else
                        {
                            code.Add(new SimpleHTML5Text(compatibility) { Text = text.Text });
                        }
                        list.Add(code);
                        break;
                    case FB2Library.Elements.TextStyles.Emphasis:
                        var emph = new EmphasisedText(compatibility);
                        if (text.HasChildren)
                        {
                            foreach (var item in text.Children.SelectMany(x=>Convert(x,compatibility,simpleTextElementConverterParams)))
                            {
                                emph.Add(item);
                            }
                        }
                        else
                        {
                            emph.Add(new SimpleHTML5Text(compatibility) { Text = text.Text });
                        }
                        list.Add(emph);
                        break;
                    case FB2Library.Elements.TextStyles.Strong:
                        var str = new Strong(compatibility);
                        if (text.HasChildren)
                        {
                            foreach (var child in text.Children)
                            {
                                foreach (var item in Convert(child, compatibility,simpleTextElementConverterParams))
                                {
                                    str.Add(item);
                                }
                            }
                        }
                        else
                        {
                            str.Add(new SimpleHTML5Text(compatibility) { Text = text.Text });
                        }
                        list.Add(str);
                        break;
                    case FB2Library.Elements.TextStyles.Sub:
                        var sub = new Sub(compatibility);
                        if (text.HasChildren)
                        {
                            foreach (var child in text.Children)
                            {
                                foreach (var item in Convert(child,compatibility,simpleTextElementConverterParams))
                                {
                                    sub.Add(item);
                                }
                            }
                        }
                        else
                        {
                            sub.Add(new SimpleHTML5Text(compatibility) { Text = text.Text });
                        }
                        list.Add(sub);
                        break;
                    case FB2Library.Elements.TextStyles.Sup:
                        var sup = new Sup(compatibility);
                        if (text.HasChildren)
                        {
                            foreach (var child in text.Children)
                            {
                                foreach (var item in Convert(child,compatibility,simpleTextElementConverterParams))
                                {
                                    sup.Add(item);
                                }
                            }
                        }
                        else
                        {
                            sup.Add(new SimpleHTML5Text(compatibility) { Text = text.Text });
                        }
                        list.Add(sup);
                        break;
                    case FB2Library.Elements.TextStyles.Strikethrough:
                        var strike = new DeletedText(compatibility);
                        if (text.HasChildren)
                        {
                            foreach (var child in text.Children)
                            {
                                foreach (var item in Convert(child,compatibility,simpleTextElementConverterParams))
                                {
                                    strike.Add(item);
                                }
                            }
                        }
                        else
                        {
                            strike.Add(new SimpleHTML5Text(compatibility) { Text = text.Text });
                        }
                        list.Add(strike);
                        break;
                }
            }
            else if (styletypeItem is InternalLinkItem)
            {
                var linkConverter = new InternalLinkConverter();
                list.AddRange(linkConverter.Convert(styletypeItem as InternalLinkItem, compatibility, new InternalLinkConverterParams
                {
                    NeedToInsertDrop = simpleTextElementConverterParams.NeedToInsertDrop, Settings = simpleTextElementConverterParams.Settings,
                }));
            }
            else if (styletypeItem is InlineImageItem)
            {
                var inlineImageConverter = new InlineImageConverter();
                list.Add(inlineImageConverter.Convert(styletypeItem as InlineImageItem,compatibility,
                    new InlineImageConverterParams { Settings = simpleTextElementConverterParams.Settings}));
            }

            return list;
        }

        /// <summary>
        /// Create a text with Capital Drop
        /// </summary>
        /// <param name="parent">parent container insert into</param>
        /// <param name="text">text to insert</param>
        /// <param name="compatibility"></param>
        private static void AddAsDrop(List<IHTMLItem> parent, SimpleText text,HTMLElementType compatibility)
        {
            var span1 = new Span(compatibility);
            span1.GlobalAttributes.Class.Value = "drop";
            int dropEnd = 0;
            // "pad" the white spaces so drop starts from visible character
            while (dropEnd < text.Text.Length && UnicodeHelpers.IsSpaceLike(text.Text[dropEnd]) )
            {
                dropEnd++;
            }
            if (dropEnd >= text.Text.Length) // in case the text is too short for drop
            {
                parent.Add(new SimpleHTML5Text(compatibility) { Text = text.Text });
                return;
            }
            // calculate the initial drop part
            string dropPart = text.Text.Substring(0, dropEnd + 1);
            // non-drop part starts from the next character
            int nondropPosition = dropEnd + 1;
            // If first character is dash/hyphen like we need to add character to 
            // capital drop so it looks better with next character
            if (UnicodeHelpers.IsNeedToBeJoinInDrop(dropPart[dropEnd]))
            {
                // we need to add to capital drop all spaces if any
                while (nondropPosition < text.Text.Length && UnicodeHelpers.IsSpaceLike(text.Text[nondropPosition]))
                {
                    nondropPosition++;
                }
                // we need to advance to include one following nonspace character , unless
                // we already at last character of the text
                if (nondropPosition < text.Text.Length)
                {
                    nondropPosition++;
                }
                // update drop part with the "string" we calculated
                dropPart += text.Text.Substring(dropEnd + 1, nondropPosition - dropEnd - 1);
            }
            span1.Add(new SimpleHTML5Text(compatibility) { Text = dropPart });
            parent.Add(span1);
            string substring = text.Text.Substring(nondropPosition);
            if (substring.Length > 0)
            {
                parent.Add(new SimpleHTML5Text(compatibility) { Text = substring });
            }

        }
    }
}
