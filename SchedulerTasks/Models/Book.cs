namespace SchedulerTasks.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Book")]
    public partial class Book
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Book()
        {
            BookInteractions = new HashSet<BookInteraction>();
            BookTransactions = new HashSet<BookTransaction>();
        }

        public int bookID { get; set; }

        [StringLength(50)]
        public string bookName { get; set; }

        public int bookState { get; set; }

        [StringLength(50)]
        public string author { get; set; }

        public int genreCode { get; set; }

        [Required]
        [StringLength(1024)]
        public string bookPictureURL { get; set; }

        [Required]
        [StringLength(1024)]
        public string bookPictureThumbnailURL { get; set; }

        public int ownerID { get; set; }

        public int addedByID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime createdAt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookInteraction> BookInteractions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookTransaction> BookTransactions { get; set; }
    }
}
