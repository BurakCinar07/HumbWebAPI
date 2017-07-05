namespace BookieAPI.Models.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Message")]
    public partial class Message
    {
        public int messageID { get; set; }

        public int fromUserID { get; set; }

        public int toUserID { get; set; }

        [Required]
        public string messageText { get; set; }

        public int fromUserMessageState { get; set; }
        public int toUserMessageState { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime createdAt { get; set; }
    }
}
