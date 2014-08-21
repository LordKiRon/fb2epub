﻿using System.Collections.Generic;
using XHTMLClassLibrary.Attributes;

namespace XHTMLClassLibrary.BaseElements.InlineElements
{
    /// <summary>
    /// The "embed" tag defines a container for an external application or interactive content (a plug-in).
    ///</summary>
    [HTMLItemAttribute(ElementName = "embed", SupportedStandards = HTMLElementType.HTML5)]
    public class Embed : HTMLItem, IInlineItem
    {
        [AttributeTypeAttributeMember(Name = "height", SupportedStandards = HTMLElementType.HTML5 | HTMLElementType.XHTML5)]
        private readonly LengthAttribute _heightAttribute = new LengthAttribute();

        [AttributeTypeAttributeMember(Name = "src", SupportedStandards = HTMLElementType.HTML5 | HTMLElementType.XHTML5)]
        private readonly URITypeAttribute _sourceAttribute = new URITypeAttribute();

        [AttributeTypeAttributeMember(SupportedStandards = HTMLElementType.HTML5 | HTMLElementType.XHTML5)]
        private readonly MIMETypeAttribute _typeAttribute = new MIMETypeAttribute();

        [AttributeTypeAttributeMember(Name = "width", SupportedStandards = HTMLElementType.HTML5 | HTMLElementType.XHTML5)]
        private readonly LengthAttribute _widthAttribute = new LengthAttribute();


        /// <summary>
        /// Specifies the height of the embedded content
        /// </summary>
        public LengthAttribute Height { get { return _heightAttribute; }}


        /// <summary>
        /// Specifies the address of the external file to embed
        /// </summary>
        public URITypeAttribute Src { get { return _sourceAttribute; }}


        /// <summary>
        /// Specifies the media type of the embedded content
        /// </summary>
        public MIMETypeAttribute Type { get { return _typeAttribute; }}


        /// <summary>
        /// Specifies the width of the embedded content
        /// </summary>
        public LengthAttribute Width { get { return _widthAttribute; }}

        public override bool IsValid()
        {
            return true;
        }

        public override List<IHTMLItem> SubElements()
        {
            return null;
        }
    }
}
