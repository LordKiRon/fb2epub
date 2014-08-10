﻿using System.Xml.Linq;

namespace HTML5ClassLibrary.Attributes
{
    public class PreloadAttribute : BaseAttribute
    {
        private enum PreloadValues
        {
            Invalid,
            Auto,
            Metadata,
            None,
        }

        private const string AttributeName = "preload";

        private PreloadValues _preload = PreloadValues.Invalid;

        public override void AddAttribute(XElement xElement)
        {
            if (!_hasValue)
            {
                return;
            }
            xElement.Add(new XAttribute(AttributeName, Value));
        }

        public override void ReadAttribute(XElement element)
        {
            _hasValue = false;
            XAttribute xObject = element.Attribute(AttributeName);
            if (xObject != null)
            {
                Value = xObject.Value;
            }
        }

        public override string Value
        {
            get
            {
                switch (_preload)
                {
                    case PreloadValues.Auto:
                        return "auto";
                    case PreloadValues.Metadata:
                        return "metadata";
                    case PreloadValues.None:
                        return "none";
                }
                return string.Empty;
            }

            set
            {
                switch (value.ToLower())
                {
                    case "auto":
                        _preload = PreloadValues.Auto;
                        break;
                    case "metadata":
                        _preload = PreloadValues.Metadata;
                        break;
                    case "none":
                        _preload = PreloadValues.None;
                        break;
                    default:
                        _preload = PreloadValues.Invalid;
                        break;
                }
            }
        }

    }
}