﻿using HTMLClassLibrary.Attributes.Events;

namespace HTMLClassLibrary.Attributes.AttributeGroups.FormEvents
{
    /// <summary>
    /// Script to be run when an element gets user input
    /// </summary>
    public class OnInputEventAttribute : OnEventAttribute
    {
        #region Overrides of OnEventAttribute

        protected override string GetAttributeName()
        {
            return "oninput";
        }

        #endregion
    }
}