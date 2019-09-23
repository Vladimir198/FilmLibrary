using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FilmLibrary.Models.Filter
{
    public class FilmsFilter
    {
        /// <summary>
        /// Наименование колонки для сортировки
        /// </summary>
        public string OrderColumn { get; set; }
        /// <summary>
        /// Направление сортировки, true - по возрасьанию, false - по убыванию
        /// </summary>
        public bool SortDirection { get; set; }
        /// <summary>
        /// Максимальное значение оценки фильма
        /// </summary>
        public double MaxScore { get; set; }
        /// <summary>
        /// Минимальное значение оценки фильма
        /// </summary>
        public double MinScore { get; set; }
    }
}