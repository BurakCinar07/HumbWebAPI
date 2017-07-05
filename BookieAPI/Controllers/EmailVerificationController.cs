using BookieAPI.Controllers.Utils;
using BookieAPI.Models.Context;
using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//UnTouched
namespace BookieAPI.Controllers
{
    public class EmailVerificationController : ApiController
    {
        Context context = new Context();

        [HttpGet]
        public VerificationResponse Verificate(string email, string verificationHash)
        {
            VerificationResponse response = new VerificationResponse();

            if(string.IsNullOrEmpty(email) && string.IsNullOrEmpty(verificationHash))
            {
                response.text = "Invalid Request";
            }
            else if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(verificationHash))
            {
                response.text = "Invalid Request";
            }
            else if (!TextUtils.IsEmailValid(email))
            {
                response.text = "Invalid Request";
            }
            else
            {
                if (context.Users.Any(x=>x.email == email))
                {
                    verificationHash = TextUtils.SanitizeInput(verificationHash);
                    User user = context.Users.Where(x => x.email == email).FirstOrDefault();
                    if(verificationHash == user.verificationHash)
                    {
                        user.emailVerified = true;
                        context.SaveChanges();

                        FcmUtils.EmailVerified(context, email);
                        response.text = "Hesabınız aktifleştirildi! Mantreads' i tüm özelliklikleriyle birlikte kullanabilirsiniz.";
                    }
                    else
                    {
                        response.text = "Invalid Request";
                    }
                }
                else
                {
                    response.text = "Invalid Request";
                }
            }
            return response;
        }
    }
}
