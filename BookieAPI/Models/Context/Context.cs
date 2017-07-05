namespace BookieAPI.Models.Context
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using DAL;
    public partial class Context : DbContext
    {
        public Context()
            : base("name=Context4")
        {
        }

        public virtual DbSet<Block> Blocks { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BookInteraction> BookInteractions { get; set; }
        public virtual DbSet<BookRequest> BookRequests { get; set; }
        public virtual DbSet<BookTransaction> BookTransactions { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Infiltrator> Infiltrators { get; set; }
        public virtual DbSet<LovedGenre> LovedGenres { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<ReportBook> ReportBooks { get; set; }
        public virtual DbSet<ForgottenPassword> ForgottenPasswords { get; set; }


        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<Context>(null);            
        }

    }
}
