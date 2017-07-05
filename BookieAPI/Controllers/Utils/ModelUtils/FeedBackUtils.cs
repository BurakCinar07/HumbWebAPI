using BookieAPI.Models.Context;
using BookieAPI.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Controllers.Utils.ModelUtils
{
    public static class FeedBackUtils
    {
        internal static void AddFeedback(Context context, string email, string feedback)
        {
            Feedback fb = new Feedback();
            fb.createdAt = DateTime.Now;
            fb.isChecked = false;
            fb.text = feedback;
            fb.userID = UserUtils.GetUserID(context, email);
            context.Feedbacks.Add(fb);
            context.SaveChanges();
        }
    }
}