using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private DynamicEntities meta = null;
        private ProxyODataService odata = null;
        public HomeController()
        {
            meta = new DynamicEntities(new Uri(System.Configuration.ConfigurationManager.AppSettings["url_meta"]));
            odata = new ProxyODataService(new Uri(System.Configuration.ConfigurationManager.AppSettings["url_data"]));

            InitServices();
        }
        public ActionResult Index()
        {
            return View();
        }

        private void InitServices()
        {
            var templates = (from t in meta.DynamicTemplates.Expand("DynamicTemplateAttributes/DynamicAttribute")
                             select t);
            odata.LoadTypes(templates);
            
            ViewBag.TableList = templates.ToList().Select(x => x.Name);
            ViewBag.IsTest = true;
        }

        public ActionResult LoadData(string tableName)
        {
            var qry = odata.GetServiceQuery(tableName);
            var data = qry.AsQueryable()/*.Where("Firstname=\"Igor\"")*/.ToDynamicArray();
            if (data != null && data.Count() > 0)
            {
                return Json(new { message = "Data is loaded!", data }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = "Data is empty!" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateEntity(string tableName, IEnumerable<ColumnDescription> columns, bool isTest = true)
        {
            string columnsStr = string.Join(",", columns.Select(c => string.Format("{0}:{1}:{2}:{3}", c.Name, (int)c.Type, c.IsNull, c.DefaultValue)));
            
            var res = odata.Execute<string>(new Uri(System.Configuration.ConfigurationManager.AppSettings["url_data"]
                + "/CreateTableSmo?template='" + tableName
                + "'&operation='" + "create"
                + "'&tableName='" + tableName
                + "'&f='" + (isTest ? "T" : "Y")
                + "'&columns='" + columnsStr + "'"), "POST", true);
            var s = (res?.FirstOrDefault() ?? "");
            ViewBag.Message = s;
            return View("Index");
        }

        private void DeleteEntity(string tableName, bool isTest = true)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                var res = odata.Execute<string>(new Uri(System.Configuration.ConfigurationManager.AppSettings["url_data"]
                    + "/CreateTableSmo?template='" + tableName
                    + "'&operation='" + "drop"
                    + "'&tableName='" + tableName
                    + "'&f='" + (isTest ? "T'" : "Y'")), "POST", true);
                var s = (res?.FirstOrDefault() ?? "");
            }
        }
    }

    public class ColumnDescription
    {
        public string Name { get; set; }
        public SqlDataType Type { get; set; }
        public bool IsNull { get; set; } = true;
        public string DefaultValue { get; set; }

        public enum SqlDataType
        {
            Integer = 10,
            Boolean = 3,
            Datetime = 6,
            Decimal = 7,
            Text = 15
        }
    }
}