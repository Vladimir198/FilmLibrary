using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FilmLibrary.Models.DataModels
{
    public class FilmsDbContext : DbContext
    {
        public FilmsDbContext() : base("MainDb")
        {
            Database.SetInitializer<FilmsDbContext>(new FilmsDbContextInitializer());
        }

        public DbSet<Film> Films { get; set; }
        public DbSet<Ganre> Ganres { get; set; }
        public DbSet<Actor> Actors { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Film>()
                .HasMany(f => f.Actors)
                .WithMany(a => a.Films);

            modelBuilder.Entity<Film>()
                .HasIndex(f=>f.Producer);


            modelBuilder.Entity<Film>()
                .HasOptional(f => f.Ganre)
                .WithMany(g => g.Films);

            modelBuilder.Entity<Actor>()
                .HasIndex(a => a.Name);
            
            modelBuilder.Entity<Ganre>()
                .HasIndex(g => g.Name);

            modelBuilder.Entity<Actor>()
                 .MapToStoredProcedures();
            modelBuilder.Entity<Ganre>()
                 .MapToStoredProcedures();
            modelBuilder.Entity<Film>()
                 .MapToStoredProcedures();

        }
    }
    class FilmsDbContextInitializer : CreateDatabaseIfNotExists<FilmsDbContext>
    {
        protected override void Seed(FilmsDbContext db)
        {
            Ganre ganre = new Ganre() { Name = "Комедия" };
            Ganre ganre1 = new Ganre() { Name = "Фантастика" };
            Ganre ganre2 = new Ganre() { Name = "Боевик" };

            Actor actor = new Actor() { Name = "Брюс Уилес" };
            Actor actor1 = new Actor() { Name = "Арнольд Шварценегер" };
            Actor actor2 = new Actor() { Name = "Джим Кери" };

            var actors = db.Actors.AddRange(new List<Actor>() { actor, actor1, actor2 }).ToList();
            var ganres = db.Ganres.AddRange(new List<Ganre>() { ganre, ganre1, ganre2 }).ToList();
            db.SaveChanges();
            Film film = new Film()
            {
                Name = "Крепкий орешек",
                Ganre = ganres[2],
                Producer = "Джим Камерон",
                Description = "Клевый фильм",
                Budget = 30000000000,
                Actors = new List<Actor>() { actors[0] },
                Score =9.5
            };
            Film film1 = new Film()
            {
                Name = "Терминатор",
                Ganre = ganres[1],
                Producer = "Режисер терминатора",
                Description = "Очень клевый фильм",
                Budget = 35600000000,
                Actors = new List<Actor>() { actors[1] },
                Score = 9.5
            };
            Film film2 = new Film()
            {
                Name = "Маска",
                Ganre = ganres[0],
                Producer = "Режисер маски",
                Description = "Клевый фильм",
                Budget = 35680000000,
                Actors = new List<Actor>() { actors[2] },
                Score = 9.5
            };
            db.Films.AddRange(new List<Film>() {film, film1, film2 });
        }
    }
}