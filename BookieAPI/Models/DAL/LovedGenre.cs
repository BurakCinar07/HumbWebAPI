namespace BookieAPI.Models.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LovedGenre")]
    public partial class LovedGenre
    {
        public int lovedGenreID { get; set; }

        public int userID { get; set; }

        public int genreCode { get; set; }

        public virtual User User { get; set; }
    }
}
