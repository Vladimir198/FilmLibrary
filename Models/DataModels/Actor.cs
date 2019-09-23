using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FilmLibrary.Models.DataModels
{
    public class Actor: BaseModel
    {
        [MaxLength(100)]
        public string Name { get; set; }
        public List<Film> Films { get; set; }
    }
}