using System.Web;
using System.Web.Optimization;

namespace WebAccess
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.1.1.js",
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/jquery-ui.js",
                        "~/Scripts/jquery.are-you-sure.js",
                        "~/Scripts/alertify.js",
                        "~/Scripts/ays-beforeunload-shim.js",
                         "~/Scripts/masonry.pkgd.js"));


            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/notify.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap_yeti.css",
                      "~/Content/jquery.dataTables.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/bootstrap-datepicker.css",
                      "~/Content/alertify.bootstrap.css",
                      "~/Content/glyphicons.css",
                      "~/Content/site.css"));
        }
    }
}
