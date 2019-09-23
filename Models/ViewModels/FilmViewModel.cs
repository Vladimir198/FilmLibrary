
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FilmLibrary.Models.ViewModels
{
    public class FilmViewModel
    {
        public FilmViewModel()
        {
            Actors = new List<string>();
            
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Producer { get; set; }

        public int? GanreId { get; set; }

        public string Ganre { get; set; }
        public List<SelectListItem> GeanreSelectList { get; set; }
        public string Description { get; set; }
        public double Budget { get; set; }
        public double Score { get; set; }
        public List<string> Actors { get; set; }
        public string ActorsString {
            get
            {
                string s= "";
                foreach (var item in Actors)
                {
                    s += String.IsNullOrEmpty(s) ? item : "; " + item;
                }
                return s;
            }
            set
            {
                Actors = value.Split(';').Select(a => a.Trim()).ToList(); ;
            }
        }
        public string ActorsHtmlText
        {
            get
            {
                var s = "<ul>";
                Actors.ForEach(a=> {
                    s += $"<li>{a}</li>";
                });
                s += "</ul>";
                return s;
            }
        }
        public string ScoreString
        {
            get
            {
                return Math.Round(Score, 1).ToString().Replace(',', '.');
            }
        }
        public string ScoreHtml
        {
            get
            {
                return $"<span>{ScoreString}</span>";
            }
        }

        public string BudgetHtml
        {
            get
            {
                return $"<span>${Math.Round(Budget)}</span>";
            }
        }

        public string MenuHtml
        {
            get
            {
                return $"<div class='tabMenu align-middle'><div filmId='{Id}' type='button' class='btn btn-danger align-self-center del-btn'/><span><i class='fas fa-trash-alt'></i></span></div></div>";
            }
        }
    }
}