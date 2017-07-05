namespace BookieAPI.Models.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Feedback")]
    public partial class Feedback
    {
        public int feedbackID { get; set; }

        [Required]
        public string text { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime createdAt { get; set; }

        public bool isChecked { get; set; }

        public int userID { get; set; }
    }
}
