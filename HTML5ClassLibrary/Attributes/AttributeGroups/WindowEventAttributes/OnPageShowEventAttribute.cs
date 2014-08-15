﻿using HTMLClassLibrary.Attributes.Events;

namespace HTMLClassLibrary.Attributes.AttributeGroups.WindowEventAttributes
{
    /// <summary>
    /// Script to be run when the window becomes visible 
    /// </summary>
    public class OnPageShowEventAttribute : OnEventAttribute
    {
        #region Overrides of OnEventAttribute

        protected override string GetAttributeName()
        {
            return "onpageshow";
        }

        #endregion
    }
}