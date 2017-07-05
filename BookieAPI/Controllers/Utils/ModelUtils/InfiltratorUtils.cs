using BookieAPI.Models.Context;
using BookieAPI.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Controllers.Utils.ModelUtils
{
    public class InfiltratorUtils
    {
        public static void AddInfiltrator(Context context)
        {
            string ip = HttpContext.Current.Request.UserHostAddress;

            Infiltrator infiltrator = new Infiltrator();
            infiltrator.IPAdress = ip;
            infiltrator.createdAt = DateTime.Now;
            context.Infiltrators.Add(infiltrator);
            context.SaveChanges();
        }
        public static void AddInfiltrator(Context context, int reason, string extraInfo)
        {
            string ip = HttpContext.Current.Request.UserHostAddress;

            Infiltrator infiltrator = new Infiltrator();
            infiltrator.IPAdress = ip;
            infiltrator.reason = reason;
            if (!string.IsNullOrEmpty(extraInfo))
            {
                infiltrator.extraInfo = extraInfo;
            }
            infiltrator.createdAt = DateTime.Now;
            context.Infiltrators.Add(infiltrator);
            context.SaveChanges();
        }
    }
}