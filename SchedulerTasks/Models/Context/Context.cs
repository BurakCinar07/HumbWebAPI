namespace SchedulerTasks.Models.Context
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Context : DbContext
    {
        public Context()
            : base("name=Model1")
        {
        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BookInteraction> BookInteractions { get; set; }
        public virtual DbSet<BookTransaction> BookTransactions { get; set; }
        public virtual DbSet<Infiltrator> Infiltrators { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<Context>(null);
        }
    }
}
