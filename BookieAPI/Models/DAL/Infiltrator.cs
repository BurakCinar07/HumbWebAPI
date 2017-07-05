namespace BookieAPI.Models.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Infiltrator")]
    public partial class Infiltrator
    {
        public int infiltratorID { get; set; }

        [Required]
        [StringLength(50)]
        public string IPAdress { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime createdAt { get; set; }
        public int reason { get; set; }
        public string extraInfo { get; set; }
    }
}
