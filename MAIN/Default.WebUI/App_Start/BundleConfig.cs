using System.Web;
using System.Web.Optimization;

namespace Default.WebUI
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {


            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/tablesorter").Include(
                        "~/Scripts/jquery.tablesorter*"));
            /*DEFAULT*/
            bundles.Add(new StyleBundle("~/content/css").Include(
                      "~/app_themes/default/css/bootstrap.css",
                      "~/app_themes/default/css/font-awesome.min.css",
                      "~/app_themes/default/css/jquery-ui.theme.css",
                      "~/app_themes/default/css/ionicons.min.css",
                      "~/app_themes/default/css/component.css",
                      "~/app_themes/default/css/icons.css",
                      "~/app_themes/default/css/navbar.css",
                      "~/app_themes/default/css/normalize.css",
                      "~/app_themes/default/css/zoom.css",
                      "~/app_themes/default/css/btn.css",
                      "~/app_themes/default/css/jquery.excoloSlider.css",
                      "~/app_themes/default/css/flyout-window.css",
                      "~/app_themes/default/css/site.css",
                      "~/app_themes/default/lib/lightbox/css/lightbox.css",
                      "~/app_themes/default/lib/SocialSharingButtons/css/rrssb.css"
                      ));

            bundles.Add(new ScriptBundle("~/content/js").Include(
                      "~/app_themes/default/js/hacktimer/HackTimer.silent.min.js",
                      "~/app_themes/default/js/hacktimer/HackTimerWorker.min.js",
                      "~/app_themes/default/js/jquery-{version}.js",
                      "~/app_themes/default/js/jquery-ui.js",
                      "~/app_themes/default/js/bootstrap.js",
                      "~/app_themes/default/js/respond.js",
                      "~/app_themes/default/js/classie.js",
                      "~/app_themes/default/js/accordion.js",
                      "~/app_themes/default/js/jquery.zoom.min.js",
                      "~/app_themes/default/js/jquery.excoloSlider.js",
                      "~/app_themes/default/js/jquery-ui.searchflyout.js",
                      "~/app_themes/default/js/jquery.jscroll.min.js",
                      "~/app_themes/default/js/jquery.waitforimages.js",
                      "~/app_themes/default/js/core.js",
                      "~/app_themes/default/lib/lightbox/js/lightbox.js",
                      "~/app_themes/default/lib/SocialSharingButtons/js/rrssb.js",
                      "~/app_themes/default/lib/animate-css/animate-css.js",
                      "~/app_themes/default/lib/animate-css/jquery.waypoints.min.js",

                      "~/app_themes/default/js/defaultsite.js"
                      ));


            bundles.Add(new ScriptBundle("~/content/paperbackground").Include(
                    "~/app_themes/default/js/paper-full.min.js",
                    "~/app_themes/default/js/paper-helper.js"
                      ));
            
            /*Landing*/
            bundles.Add(new StyleBundle("~/landing/css").Include(
                      "~/app_themes/landing/lib/bootstrap/css/bootstrap.min.css",
                      "~/app_themes/landing/lib/font-awesome/css/font-awesome.min.css",
                      "~/app_themes/landing/lib/magnific-popup/magnific-popup.css",
                      "~/app_themes/landing/css/main.css"
                      ));

            bundles.Add(new ScriptBundle("~/landing/js").Include(
                      "~/app_themes/default/js/hacktimer/HackTimer.silent.min.js",
                      "~/app_themes/default/js/hacktimer/HackTimerWorker.min.js",
                      "~/app_themes/default/js/jquery-{version}.js",
                      "~/app_themes/default/js/jquery-ui.js",
                      "~/app_themes/default/js/bootstrap.js",
                      "~/app_themes/default/js/respond.js",
                      "~/app_themes/default/js/classie.js",
                      "~/app_themes/default/js/accordion.js",
                      "~/app_themes/default/js/jquery.zoom.min.js",
                      "~/app_themes/default/js/jquery.excoloSlider.js",
                      "~/app_themes/default/js/jquery-ui.searchflyout.js",
                      "~/app_themes/default/js/jquery.jscroll.min.js",
                      "~/app_themes/default/js/jquery.waitforimages.js",
                      "~/app_themes/default/js/infcon3.js",
                      "~/app_themes/default/js/core.js",
                      "~/app_themes/default/lib/lightbox/js/lightbox.js",
                      "~/app_themes/default/lib/SocialSharingButtons/js/rrssb.js",
                      "~/app_themes/default/lib/animate-css/animate-css.js",
                      "~/app_themes/default/lib/animate-css/jquery.waypoints.min.js",

                      "~/app_themes/default/js/defaultsite.js",
                      "~/app_themes/landing/js/site.js"
                      ));
            

            /*ADMIN*/
            bundles.Add(new ScriptBundle("~/admin/js").Include(
                      "~/App_Themes/admin/js/jquery-{version}.js",
                      "~/App_Themes/admin/js/jquery-ui.js",
                      "~/App_Themes/admin/js/bootstrap.js",
                      "~/App_Themes/admin/js/respond.js",
                      "~/App_Themes/admin/js/jquery.metadata.js",
                      "~/App_Themes/admin/js/jquery.tablesorter*",
                      "~/App_Themes/admin/js/bootstrap-colorpicker.min.js",

                      /*Codemirror*/
                      "~/App_Themes/admin/lib/codemirror/lib/codemirror.js",
                      "~/App_Themes/admin/lib/codemirror/mode/xml/xml.js",
                      "~/App_Themes/admin/lib/codemirror/mode/javascript/javascript.js",
                      "~/App_Themes/admin/lib/codemirror/mode/css/css.js",
                      "~/App_Themes/admin/lib/codemirror/mode/htmlmixed/htmlmixed.js",

                      /*Summernote*/
                      "~/App_Themes/admin/lib/summernote/dist/summernote.js",
                      "~/App_Themes/admin/lib/summernote/lang/summernote-de-DE.js",
                      "~/App_Themes/admin/lib/elfile/js/elfinder.full.js",
                      "~/App_Themes/admin/lib/elfile/js/i18n/elfinder.de.js",

                      /*custom js*/
                      "~/App_Themes/admin/js/admin_site.js",
                      "~/App_Themes/admin/js/admin.custom.forms.js",
                      "~/app_themes/admin/lib/Chartmaster/chart.js",
                      "~/app_themes/default/js/core.js"
                      ));
            bundles.Add(new ScriptBundle("~/admin/bundles/jqueryval").Include(
            "~/App_Themes/admin/js/jquery.validate.*"));

            bundles.Add(new StyleBundle("~/admin/css").Include(
                      "~/App_Themes/admin/css/bootstrap.min.css",
                      "~/App_Themes/admin/css/font-awesome.min.css",
                      "~/App_Themes/admin/css/ionicons.min.css",
                      "~/App_Themes/admin/css/jquery-ui.min.css",
                      "~/App_Themes/admin/css/jquery-ui.theme.css",
                      "~/App_Themes/admin/css/bootstrap-colorpicker.min.css",
                      /*Codemirror*/
                      "~/App_Themes/admin/lib/codemirror/lib/codemirror.css",
                      "~/App_Themes/admin/lib/codemirror/theme/material.css",

                      /*Summernote*/
                      "~/App_Themes/admin/lib/summernote/dist/summernote.css",
                      "~/App_Themes/admin/lib/elfile/css/elfinder.full.css",
                      "~/App_Themes/admin/css/admin_site.css",

                      "~/App_Themes/admin/css/tablesorter.css"
                      ));

            /*ADMIN in Frontend*/
            bundles.Add(new ScriptBundle("~/admin/sharedjs").Include(
                "~/App_Themes/admin/js/bootstrap-colorpicker.min.js",
                "~/App_Themes/admin/lib/elfile/js/elfinder.full.js",
                "~/App_Themes/admin/lib/elfile/js/i18n/elfinder.de.js"
                ));

            bundles.Add(new StyleBundle("~/admin/sharedcss").Include(
                "~/App_Themes/admin/css/bootstrap-colorpicker.min.css",
                "~/App_Themes/admin/lib/elfile/css/elfinder.full.css"
                ));

            System.Web.Optimization.BundleTable.EnableOptimizations = true;
        }
    }
}
