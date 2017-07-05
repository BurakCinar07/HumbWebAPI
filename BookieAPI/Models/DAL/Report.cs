namespace BookieAPI.Models.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Report")]
    public partial class Report
    {
        public int reportID { get; set; }

        public int fromUserID { get; set; }

        public int toUserID { get; set; }

        public string reportInfo { get; set; }

        public int reportCode { get; set; }
        public DateTime createdAt { get; set; }
    }
}
