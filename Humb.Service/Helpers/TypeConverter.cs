﻿using Humb.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Service.Helpers
{
    public static class TypeConverter
    {
        public static int BookStateToInteractionType(int bookState)
        {
            if (bookState == ResponseConstant.STATE_OPENED_TO_SHARE)
                return ResponseConstant.INTERACTION_OPEN_TO_SHARE;
            else if (bookState == ResponseConstant.STATE_READING)
                return ResponseConstant.INTERACTION_READ_START;
            else
                return ResponseConstant.INTERACTION_CLOSE_TO_SHARE;
        }
        public static int InteractionTypeToBookState(int interactionType)
        {
            if (interactionType == ResponseConstant.INTERACTION_READ_START)
                return ResponseConstant.STATE_READING;
            else if (interactionType == ResponseConstant.INTERACTION_OPEN_TO_SHARE)
                return ResponseConstant.STATE_OPENED_TO_SHARE;
            else
                return ResponseConstant.STATE_CLOSED_TO_SHARE;
        }
    }
}