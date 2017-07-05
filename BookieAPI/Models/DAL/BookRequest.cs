namespace BookieAPI.Models.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BookRequest")]
    public partial class BookRequest
    {
        [Key]
        public int requestID { get; set; }

        public int requestingUserID { get; set; }

        public int respondingUserID { get; set; }

        public int bookID { get; set; }

        public int requestType { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime createdAt { get; set; }

        public virtual Book Book { get; set; }
    }
}
