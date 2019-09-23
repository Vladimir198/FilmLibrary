using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FilmLibrary.Models.DataModels
{
    public class Film: BaseModel
    {
        public Film()
        {
            Actors = new List<Actor>();
        }
        public string Name { get; set; }

        [MaxLength(100)]
        public string Producer { get; set; }
        public int? GanreId { get; set; }
        public string Description { get; set; }
        public double Budget { get; set; }
        public double Score { get; set; }
        public List<Actor> Actors { get; set; }
        public Ganre Ganre { get; set; }
    }
}