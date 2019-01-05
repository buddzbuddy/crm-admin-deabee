using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using WebClient.MetadataService;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var meta = new DynamicEntities(new Uri("http://localhost:60086/DynamicMetadataService.svc"));
            var templates = (from t in meta.DynamicTemplates.Expand("DynamicTemplateAttributes/DynamicAttribute")
                             select t);
            var odata = new ProxyODataService(new Uri("http://localhost:60086/DynamicODataService.svc"));
            odata.LoadTypes(templates);

            var qry = odata.GetServiceQuery("SomeTable");
            ViewBag.Data = new ObservableCollection<dynamic>(qry.AsQueryable()/*.Where("Firstname=\"Igor\"")*/.ToDynamicArray());
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}