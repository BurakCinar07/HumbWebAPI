namespace BookieAPI.Models.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Block")]
    public partial class Block
    {
        public int blockID { get; set; }

        public int fromUserID { get; set; }

        public int toUserID { get; set; }
        public DateTime createdAt { get; set; }
    }
}
