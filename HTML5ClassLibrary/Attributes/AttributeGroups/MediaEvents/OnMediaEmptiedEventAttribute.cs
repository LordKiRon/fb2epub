﻿using HTMLClassLibrary.Attributes.Events;

namespace HTMLClassLibrary.Attributes.AttributeGroups.MediaEvents
{
    /// <summary>
    /// Script to be run when something bad happens and the file is suddenly unavailable (like unexpectedly disconnects)
    /// </summary>
    public class OnMediaEmptiedEventAttribute: OnEventAttribute
    {
        #region Overrides of OnEventAttribute

        protected override string GetAttributeName()
        {
            return "onemptied";
        }

        #endregion
    }
}