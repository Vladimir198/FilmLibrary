using FilmLibrary.Models.DataModels;
using FilmLibrary.Models.Filter;
using FilmLibrary.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FilmLibrary.Services
{
    public class FilmsService
    {

        #region Films

        public FilmViewModel GetFilmById(int id)
        {

            var film = new Film();
            using (var ctx = new FilmsDbContext())
            {
                if (Properties.Settings.Default.UsStoredProcedure)
                {
                    var idParam = new System.Data.SqlClient.SqlParameter("@id", id);
                    film = ctx.Database.SqlQuery<Film>("GetFilm @id", idParam)
                        .FirstOrDefault();
                    idParam = new System.Data.SqlClient.SqlParameter("@filmId", id);
                    film.Actors = ctx.Database.SqlQuery<Actor>("GetFilmActors @filmId", idParam).ToList();
                    idParam = new System.Data.SqlClient.SqlParameter("@filmId", id);
                    film.Ganre = ctx.Database.SqlQuery<Ganre>("GetFilmGanre @filmId", idParam).FirstOrDefault();
                }
                else
                {
                    film = ctx.Films
                        .Include("Actors")
                        .Include("Ganre")
                        .FirstOrDefault(f => f.Id == id);
                }
            }
            var filmView = new FilmViewModel();
            if (film != null)
            {
                filmView = FilmToFilmViewModel(film);
            }
            return filmView;
        }

        public bool RemoveFilm(int id)
        {
            using (var ctx = new FilmsDbContext())
            {
                var model = ctx.Films.FirstOrDefault(f=>f.Id == id);
                if (model != null)
                {
                    ctx.Films.Remove(model);
                    ctx.SaveChanges();
                }
                else
                {
                    throw new Exception($"В базе нет ыильма с ID = {id}");
                }

            }
            return true;
        }

        public List<FilmViewModel> GetFilmsByFilter(FilmsFilter filter)
        {
            if (filter == null)
            {
                throw new NullReferenceException("filter");
            }
            var result = new List<FilmViewModel>();
            var films = new List<Film>();
            using (var ctx = new FilmsDbContext())
            {
                if (Properties.Settings.Default.UsStoredProcedure)
                {
                    var maxScoreParam = new System.Data.SqlClient.SqlParameter("@maxScore", filter.MaxScore);
                    var minScoreParam = new System.Data.SqlClient.SqlParameter("@minScore", filter.MinScore);

                    films = ctx.Database.SqlQuery<Film>("GetFilmList @maxScore, @minScore", new[] { maxScoreParam, minScoreParam })
                        .ToList();
                    films.ForEach(f=> 
                    {
                        var idParam = new System.Data.SqlClient.SqlParameter("@filmId", f.Id);
                        f.Actors = ctx.Database.SqlQuery<Actor>("GetFilmActors @filmId", idParam).ToList();
                        idParam = new System.Data.SqlClient.SqlParameter("@filmId", f.Id);
                        f.Ganre = ctx.Database.SqlQuery<Ganre>("GetFilmGanre @filmId", idParam).FirstOrDefault();
                    });
                }
                else
                {
                    films = ctx.Films
                        .Include("Actors")
                        .Include("Ganre")
                        .Where(f => f.Score >= filter.MinScore && f.Score <= filter.MaxScore).ToList();
                }
                if (!string.IsNullOrEmpty(filter.OrderColumn))
                {
                    FilmsOrderBy(ref films, filter.OrderColumn, filter.SortDirection);
                }

                result = films.Select(f => new FilmViewModel
                {
                    Actors = f.Actors.Select(a => a.Name).ToList(),
                    GanreId = f.GanreId,
                    Ganre = f.Ganre.Name,
                    Budget = f.Budget,
                    Description = f.Description,
                    Name = f.Name,
                    Id = f.Id,
                    Producer = f.Producer,
                    Score = f.Score
                }).ToList();
            }
            return result;

        }
        
        /// <summary>
        /// Сортировка фильмов по колонкам
        /// </summary>
        /// <param name="query">запрос к базе</param>
        /// <param name="column">колонка по которой происходит сортировка</param>
        /// <param name="des">Направление сортировки, true - по возрасьанию, false - по убыванию</param>
        void FilmsOrderBy(ref List<Film> query, string column, bool des)
        {
            switch (column)
            {
                case "Name":
                    {
                        query = des ? query.OrderBy(f => f.Name).ToList() : query.OrderByDescending(f => f.Name).ToList();
                    }
                    break;
                case "Ganre":
                    {
                        query = des ? query.OrderBy(f => f.Ganre.Name).ToList() : query.OrderByDescending(f => f.Ganre.Name).ToList();
                    }
                    break;
                case "Producer":
                    {
                        query = des ? query.OrderBy(f => f.Producer).ToList() : query.OrderByDescending(f => f.Producer).ToList();
                    }
                    break;
                case "ScoreHtml":
                    {
                        query = des ? query.OrderByDescending(f => f.Score).ToList() : query.OrderBy(f => f.Score).ToList();
                    }
                    break;
                case "BudgetHtml":
                    {
                        query = des ? query.OrderByDescending(f => f.Budget).ToList() : query.OrderBy(f => f.Budget).ToList();
                    }
                    break;
                default:
                    break;
            }
        }
        public bool SaveFilm(FilmViewModel film)
        {
            if (film == null)
            {
                throw new NullReferenceException("film");
            }

            using (var ctx = new FilmsDbContext())
            {
                var model = film.Id == 0
                    ? new Film()
                    : ctx.Films
                    .Include("Actors")
                    .Include("Ganre")
                    .FirstOrDefault(f => f.Id == film.Id);

                model.Name = film.Name;
                model.Producer = film.Producer;
                model.Score = film.Score;
                model.GanreId = SaveGanre(film.Ganre);
                model.Description = film.Description;
                model.Budget = film.Budget;
                var actorsIds = CompareAndSaveActors(film.Actors);
                var existActorsId = model.Actors.Select(a => a.Id).ToList();
                var actorsToAdd = actorsIds.Except(existActorsId).ToList();
                var actorsToDel = existActorsId.Except(actorsIds).ToList();
                var addActors = ctx.Actors.Where(a=>actorsToAdd.Contains(a.Id)).ToList();
                model.Actors.RemoveAll(a=> actorsToDel.Contains(a.Id));
                model.Actors.AddRange(addActors);

                if (film.Id == 0)
                {
                    ctx.Films.Add(model);
                }
                ctx.SaveChanges();
            }
            return true;
        }

        private FilmViewModel FilmToFilmViewModel(Film film)
        {
            var result = new FilmViewModel();
            result.GanreId = film.GanreId;
            result.Ganre = film.Ganre.Name;
            result.Budget = film.Budget;
            result.Description = film.Description;
            result.Name = film.Name;
            result.Id = film.Id;
            result.Producer = film.Producer;
            result.Score = film.Score;
            result.GeanreSelectList = GetGanresSelectListItems(result.Ganre);
            result.Actors = film.Actors.Select(a=>a.Name).ToList();
            return result;
        }

        #endregion

        #region Actors
        public List<Actor> GetActorsByName(string name)
        {

            var result = new List<Actor>();
            using (var ctx = new FilmsDbContext())
            {
                result = ctx.Actors.Where(a => a.Name.Contains(name)).ToList();
            }
            return result;
        }

        public List<int> CompareAndSaveActors(List<string> actorNames)
        {
            if (actorNames == null)
            {
                throw new NullReferenceException("actorNames");
            }
            List<int> result = new List<int>();
            using (var ctx = new FilmsDbContext())
            {
                var exist = ctx.Actors.Where(a => actorNames.Contains(a.Name));
                result.AddRange(exist.Select(a => a.Id).ToList());
                var existNames = exist.Select(a => a.Name).ToList();
                var newNames = actorNames.Except(existNames).ToList();
                var newActors = newNames.Select(n => new Actor() { Name = n }).ToList();
                newActors = ctx.Actors.AddRange(newActors).ToList();
                ctx.SaveChanges();
                result.AddRange(newActors.Select(a => a.Id).ToList());
            }
            return result;
        }
        #endregion

        #region Garnres
        public int SaveGanre(string ganre)
        {
            if (ganre == null)
            {
                throw new NullReferenceException("ganre");
            }
            int id;
            using (var ctx = new FilmsDbContext())
            {

                var model = ctx.Ganres.FirstOrDefault(g => g.Name == ganre);
                if (model == null)
                {
                    model = ctx.Ganres.Add(new Ganre() { Name = ganre});
                }
                id = model.Id;
            }
            return id;
        }

        public List<string> GetGanresByName(string name)
        {
            var result = new List<string>();
            using (var ctx = new FilmsDbContext())
            {
                result = ctx.Ganres.Where(g => g.Name.Contains(name)).Select(g=>g.Name).ToList();
            }
            return result;
        }
       
        public List<SelectListItem> GetGanresSelectListItems(string selectedName)
        {
            var result = new List<SelectListItem>();
            using (var ctx = new FilmsDbContext())
            {
                result = ctx.Ganres.Select(g => new SelectListItem()
                {
                    Text = g.Name,
                    Value = g.Name,
                    Selected = g.Name == selectedName
                }).ToList();
            }
            return result;
        }
        #endregion

        #region Producer

        public List<string> GetProducerNames(string producer)
        {
            List<string> result;
            using (var ctx = new FilmsDbContext())
            {
                result = ctx.Films.Where(f => f.Producer.Contains(producer)).Select(f=>f.Producer).ToList();
                
            }
            return result;
        }

        #endregion

    }
}