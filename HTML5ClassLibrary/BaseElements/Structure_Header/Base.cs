﻿using HTML5ClassLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using HTML5ClassLibrary.Attributes.AttributeGroups.FormEvents;
using HTML5ClassLibrary.Attributes.AttributeGroups.HTMLGlobal;
using HTML5ClassLibrary.Attributes.AttributeGroups.KeyboardEvents;
using HTML5ClassLibrary.Attributes.AttributeGroups.MediaEvents;
using HTML5ClassLibrary.Attributes.AttributeGroups.MouseEvents;
using HTML5ClassLibrary.Attributes.AttributeGroups.WindowEventAttributes;
using HTML5ClassLibrary.Exceptions;

namespace HTML5ClassLibrary.BaseElements.Structure_Header
{
    /// <summary>
    /// To resolve relative URLs, Web browsers will use the base URL from where the Web page was downloaded. 
    /// In some circumstances, it is necessary to instruct the Web browser to use a different base URL, 
    /// in which case the base element is used.
    /// </summary>
    public class Base : IHTML5Item
    {
        public const string ElementName = "base";

        // Basic attributes
        private readonly HrefAttribute _hrefAttribute = new HrefAttribute();
        private readonly FormTargetAttribute _targetAttribute = new FormTargetAttribute();
        private readonly HTMLGlobalAttributes _globalAttributes = new HTMLGlobalAttributes();
        private readonly FormEvents _formEvents = new FormEvents();
        private readonly KeyboardEvents _keyboardEvents = new KeyboardEvents();
        private readonly MediaEvents _mediaEvents = new MediaEvents();
        private readonly MouseEvents _mouseEvents = new MouseEvents();
        private readonly WindowEventAttributes _windowEventAttributes = new WindowEventAttributes();



        public HTMLGlobalAttributes GlobalAttributes { get { return _globalAttributes; }}


        public FormEvents FormEvents { get { return _formEvents; } }

        public KeyboardEvents KeyboardEvents { get { return _keyboardEvents; } }

        public MediaEvents MediaEvents { get { return _mediaEvents; } }

        public MouseEvents MouseEvents { get { return _mouseEvents; } }

        public WindowEventAttributes WindowEvents { get { return _windowEventAttributes; } }

        /// <summary>
        /// Specifies an absolute URL that acts as the base URL for resolving relative URLs. 
        /// This attribute is required.
        /// </summary>
        public static XNamespace XhtmlNameSpace = @"http://www.w3.org/1999/xhtml";

        /// <summary>
        /// Specifies the base URL for all relative URLs in the page
        /// </summary>
        public HrefAttribute HRef { get { return _hrefAttribute; } }

        /// <summary>
        /// Specifies the default target for all hyperlinks and forms in the page
        /// </summary>
        public FormTargetAttribute Target { get { return _targetAttribute; }}
        
        public void Load(XNode xNode)
        {
            if (xNode.NodeType != XmlNodeType.Element)
            {
                throw new HTML5ViolationException("xNode is not of element type");
            }
            var xElement = (XElement)xNode;
            if (xElement.Name.LocalName != ElementName)
            {
                throw new HTML5ViolationException(string.Format("xNode is not {0} element", ElementName));
            }

            _globalAttributes.ReadAttributes(xElement);
            _formEvents.ReadAttributes(xElement);
            _keyboardEvents.ReadAttributes(xElement);
            _mediaEvents.ReadAttributes(xElement);
            _mouseEvents.ReadAttributes(xElement);
            _windowEventAttributes.ReadAttributes(xElement);
            _hrefAttribute.ReadAttribute(xElement);
            _targetAttribute.ReadAttribute(xElement);
        }


        public XNode Generate()
        {
            var xElement = new XElement(XhtmlNameSpace + ElementName);
            
            _globalAttributes.AddAttributes(xElement);
            _formEvents.AddAttributes(xElement);
            _keyboardEvents.AddAttributes(xElement);
            _mediaEvents.AddAttributes(xElement);
            _mouseEvents.AddAttributes(xElement);
            _windowEventAttributes.AddAttributes(xElement);
            _hrefAttribute.AddAttribute(xElement);
            _targetAttribute.AddAttribute(xElement);

            return xElement;
        }

        public bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Adds sub-item to the item , only if 
        /// allowed by the rules and element can accept content
        /// </summary>
        /// <param name="item">sub-item to add</param>
        public void Add(IHTML5Item item)
        {
            throw new Exception("This element does not contain sub-items");
        }

        public void Remove(IHTML5Item item)
        {
            throw new Exception("This element does not contain sub-items");
        }

        public List<IHTML5Item> SubElements()
        {
            return null;
        }
        /// <summary>
        /// Get/Set item parent in the XHTML "tree"
        /// </summary>
        public IHTML5Item Parent { get; set; }
    }
}
