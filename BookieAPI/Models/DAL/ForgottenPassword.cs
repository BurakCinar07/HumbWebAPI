using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.DAL
{
    [Table("ForgottenPassword")]
    public class ForgottenPassword
    {
        public int ID { get; set; }
        public string newPassword { get; set; }
        public string token { get; set; }
        public string email { get; set; }
        public DateTime createdAt { get; set; }
    }
}