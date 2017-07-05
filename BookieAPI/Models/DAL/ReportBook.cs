using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.DAL
{
    [Table("ReportBook")]
    public partial class ReportBook
    {
        [Key]
        public int reportID { get; set; }
        public int userID { get; set; }
        public int bookID { get; set; }
        public string reportInfo { get; set; }
        public int reportCode { get; set; }
        public DateTime createdAt { get; set; }

    }
}