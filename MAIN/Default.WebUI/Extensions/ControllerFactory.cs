using Default.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Default.WebUI.Extensions
{
//    /*
     
//routes.MapRoute
//(
//"FriendlyUrl",
//"{name}",
//new { controller = "FriendlyUrl", action = "Index", name = UrlParameter.Optional }
//);
     
     
//     Application_Start():
//     ControllerBuilder.Current.SetControllerFactory(new ControllerFactory());
      
     
//     */
//    public class ControllerFactory : DefaultControllerFactory
//    {
//        //public override IController CreateController(RequestContext requestContext, string controllerName)
//        //{
//        //    return base.CreateController(requestContext, 
//        //        controllerName == "Home" ? 
//        //        WAFContext.Request.GetContent().GetType().Name : controllerName);
//        //}

//        //protected override Type GetControllerType(RequestContext requestContext, string controllerName)
//        //{

//        //    return base.GetControllerType(requestContext, 
//        //        controllerName == "Home" ? 
//        //        WAFContext.Request.GetContent().GetType().Name : controllerName);
//        //}

//        public override IController CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)        
//        {
//            if(controllerName != "Home")
//            {
//                return base.CreateController(requestContext,controllerName);
//            }

//            string controllername = requestContext.RouteData.Values["controller"].ToString();           
//            // Debug.WriteLine(string.Format("Controller Name : {0}", controllername));            
//            Type controllerType = Type.GetType(string.Format(typeof(HomeController).ToString()));
            
//            // (/*need to set full qualifiere name*/"Custom_Controller_Factory.Controllers.{0}",controllername));
//            // typeof(Home);            
//            IController controller = Activator.CreateInstance(controllerType) as IController;  
          
//            return controller;         
//    }
//        public override void ReleaseController(IController controller)
//        {
//            IDisposable dispose = controller as IDisposable;
//            if (dispose != null)
//            {
//                dispose.Dispose();
//            }
//        }
//    }
}