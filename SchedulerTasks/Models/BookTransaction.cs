namespace SchedulerTasks.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BookTransaction")]
    public partial class BookTransaction
    {
        [Key]
        public int transactionID { get; set; }

        public int giverUserID { get; set; }

        public int takerUserID { get; set; }

        public int bookID { get; set; }

        public int transactionType { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime createdAt { get; set; }

        public virtual Book Book { get; set; }
    }
}
