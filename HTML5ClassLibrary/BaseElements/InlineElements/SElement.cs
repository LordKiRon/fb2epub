﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTML5ClassLibrary.BaseElements.InlineElements
{
    /// <summary>
    /// The "s" element is redefined in HTML5, and is now used to define text that is no longer correct.
    /// </summary>
    public class SElement : TextBasedElement
    {
        internal const string ElementName = "s";

        #region Overrides of TextBasedElement

        protected override string GetElementName()
        {
            return ElementName;
        }

        #endregion

    }
}
