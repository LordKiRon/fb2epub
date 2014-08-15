﻿using System.Xml.Linq;

namespace HTMLClassLibrary.Attributes.AttributeGroups.HTMLGlobal
{
    /// <summary>
    /// The contenteditable attribute specifies whether the content of an element is editable or not.
    /// Note: When the contenteditable attribute is not set on an element, the element will inherit it from its parent.
    /// </summary>
    public class ContentEditableAttribute : BaseAttribute
    {
        private enum Editable
        {
            Invalid,
            True,
            False,
        }

        private Editable _autocomplete = Editable.Invalid;

        private const string AttributeName = "contenteditable";

        #region Overrides of BaseAttribute


        public override string Value
        {
            get
            {
                switch (_autocomplete)
                {
                    case Editable.True:
                        return "true";
                    case Editable.False:
                        return "false";
                }
                return string.Empty;
            }

            set
            {
                switch (value.ToLower())
                {
                    case "true":
                        _autocomplete = Editable.True;
                        break;
                    case "false":
                        _autocomplete = Editable.False;
                        break;
                    default:
                        _autocomplete = Editable.Invalid;
                        break;
                }
            }
        }


        public override void AddAttribute(XElement xElement)
        {
            if (!AttributeHasValue)
            {
                return;
            }
            xElement.Add(new XAttribute(AttributeName, Value));
        }

        public override void ReadAttribute(XElement element)
        {
            AttributeHasValue = false;
            XAttribute xObject = element.Attribute(AttributeName);
            if (xObject != null)
            {
                Value = xObject.Value;
            }
        }

        #endregion

    }
}