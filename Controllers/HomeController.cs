using FilmLibrary.Models.Filter;
using FilmLibrary.Models.Table;
using FilmLibrary.Models.ViewModels;
using FilmLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;


namespace FilmLibrary.Controllers
{
    public class HomeController : Controller
    {
        FilmsService _service;
        public HomeController()
        {
            _service = new FilmsService();
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetFilmsData(TableRequest request)
        {
            try
            {
             
            var filter = request.Filter;
            
            filter.OrderColumn = request.Columns[request.Order[0].Column].Data;
            filter.SortDirection = request.Order[0].Dir == "desc";
            var result = _service.GetFilmsByFilter(filter);
            var total = result.Count;
            result = result.Skip(request.Start).Take(request.Length).ToList();
            var filtred = result.Count;
            var data = new
            {
                draw = request.Draw,
                recordsTotal = total,
                recordsFiltered = filtred,
                data = result.Select(f => (object)new
                {
                    TD_RowId = f.Id,
                    f.ActorsHtmlText,
                    f.BudgetHtml,
                    f.Description,
                    f.Ganre,
                    f.ScoreHtml,
                    f.Producer,
                    f.Name,
                    f.MenuHtml
                }).ToList()
            };
            return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }); 
                //TODO: Запись в лог, бросить исключение.
            }
        }

        [HttpPost]
        public JsonResult RemoveFilm(int id)
        {
            try
            {
                if (id == 0)
                {
                    return Json(new { message = "Id не должен быть равен 0"});
                }
                var res = _service.RemoveFilm(id);
                    
                return new JsonResult() { Data = new { message = "Ok"} };
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult AddOrUpdateFilm(int id)
        {
            FilmViewModel model;
            if (id==0)
            {
                model = new FilmViewModel();
                model.GeanreSelectList = _service.GetGanresSelectListItems("");
            }
            else
            {
                model = _service.GetFilmById(id);
            }
              
            return PartialView("FilmViewPartial", model);
        }

        [HttpPost]
        public ActionResult AddUrUpdateFilm(FilmViewModel film)
        {
            try
            {
                _service.SaveFilm(film);
                return new JsonResult() { Data = new { message ="Ok"} };
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message});
            }
        }
        [HttpPost]
        public JsonResult GetAutorsAutoComplit(string query)
        {
            try
            {
                List<string> suggestions = new List<string>();
                suggestions = _service.GetActorsByName(query).Select(a=>a.Name).ToList();
                return Json(new { suggestions }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new HttpException(404, "not found");
            }
        }

        [HttpPost]
        public JsonResult GetProducerAutoComplit(string query)
        {
            try
            {
                List<string> suggestions = new List<string>();
                suggestions = _service.GetProducerNames(query).ToList();
                return Json(new { suggestions }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new HttpException(404, "not found");
            }
        }
    }
}