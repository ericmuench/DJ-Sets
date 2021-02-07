using DJSets.clerks.filesystem;
using Microsoft.EntityFrameworkCore;

namespace DJSets.model.entityframework
{
    /// <summary>
    /// This class provides Sqlite-Access to data used by the application.
    /// </summary>
    public class DjSetsSqliteDbContext : DbContext
    {
        #region DB Entities
        /// <summary>
        /// This <see cref="DbSet{TEntity}"/> stores all <see cref="Song"/>-Objects used by the application
        /// </summary>
        public DbSet<Song> Songs { get; set; }

        /// <summary>
        /// This <see cref="DbSet{TEntity}"/> stores all <see cref="Setlist"/>-Objects used by the application
        /// </summary>
        public DbSet<Setlist> Setlists { get; set; }

        /// <summary>
        /// This <see cref="DbSet{TEntity}"/> stores all <see cref="SetlistPosition"/>-Objects used by the application
        /// </summary>
        public DbSet<SetlistPosition> SetlistPositions { get; set; }
        #endregion

        #region Functions
        /// <see cref="OnConfiguring"/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={new DjSetsFilePathManager().ApplicationDbPath()}");
        }

        /// <see cref="OnModelCreating"/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //configure Model / columns
            ConfigSongs(modelBuilder);
            ConfigSetlists(modelBuilder);
            ConfigSetlistPositions(modelBuilder);
        }

        #endregion

        #region Help Fuctions
        /// <summary>
        /// This function configures the Song-Table
        /// </summary>
        /// <param name="modelBuilder">Modelbuilder to do the configuration with</param>
        private void ConfigSongs(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Song>()
                .Property(it => it.Title)
                .IsRequired();

            modelBuilder
                .Entity<Song>()
                .Property(it => it.Artist)
                .IsRequired();
        }

        /// <summary>
        /// This function configures the Setlist-Table
        /// </summary>
        /// <param name="modelBuilder">Modelbuilder to do the configuration with</param>
        private void ConfigSetlists(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Setlist>()
                .Property(it => it.Title)
                .IsRequired();
        }

        /// <summary>
        /// This function configures the SetlistPositions-Table
        /// </summary>
        /// <param name="modelBuilder">Modelbuilder to do the configuration with</param>
        private void ConfigSetlistPositions(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<SetlistPosition>()
                .HasOne(it => it.Setlist)
                .WithMany(it => it.SetlistPositions)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            

            modelBuilder
                .Entity<SetlistPosition>()
                .HasOne(it => it.Song)
                .WithMany(it => it.SetlistPositions)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
        #endregion

    }
}
