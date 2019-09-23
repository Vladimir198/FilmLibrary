using System.Web;
using System.Web.Optimization;

namespace FilmLibrary
{
    public class BundleConfig
    {
        // Дополнительные сведения об объединении см. на странице https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/jquery.dataTables.js")
                .Include("~/Scripts/jquery.autocomplete.js")
                .Include("~/Scripts/dataTables.select.js")
                .Include("~/Scripts/dataTables.buttons.js")
                .Include("~/Scripts/jquery.mask.min.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/jqueryval")
                .Include("~/Scripts/jquery.validate*")
                .Include("~/Scripts/jquery.validate*")
                );

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/bootstrap.css",
                      "~/Content/site.css")
                .Include("~/Content/dataTables.bootstrap4.css")
                .Include("~/Content/fontawesome-all.min.css"));
        }
    }
}
