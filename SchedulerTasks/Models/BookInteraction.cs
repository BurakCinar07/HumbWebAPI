namespace SchedulerTasks.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BookInteraction")]
    public partial class BookInteraction
    {
        [Key]
        public int interactionID { get; set; }

        public int userID { get; set; }

        public int bookID { get; set; }

        public int interactionType { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime createdAt { get; set; }

        public virtual Book Book { get; set; }

        public virtual User User { get; set; }
    }
}
