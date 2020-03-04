using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Dynamic;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using Web.Default;
using Web.Models;
using Web.OData.Models.Meta;

namespace Web.Controllers
{
    public class ResourcesController : Controller
    {
        private Container meta = null;
        public ResourcesController()
        {
            meta = new Container(new Uri(System.Configuration.ConfigurationManager.AppSettings["url_meta"]));
        }
        public ActionResult Index()
        {
            ViewBag.Templates = meta.DynamicTemplates.ToList();
            return View();
        }

        public ActionResult CreateTemplate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateTemplate(DynamicTemplateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newTemplate = new DynamicTemplate { Name = model.Name, Description = model.Description };
                meta.AddToDynamicTemplates(newTemplate);
                meta.SaveChanges();
                return RedirectToAction("ViewTemplate", new { id = newTemplate.Id });
            }
            return View();
        }

        public ActionResult AddTemplateAttribute(int templateId)
        {
            ViewBag.TemplateName = meta.DynamicTemplates.ToList().FirstOrDefault(x => x.Id == templateId).Name;
            return View(new DynamicFieldViewModel
            {
                TemplateId = templateId
            });
        }

        [HttpPost]
        public ActionResult AddTemplateAttribute(DynamicFieldViewModel model)
        {
            if (ModelState.IsValid)
            {
                var field = new DynamicField
                {
                    Label = model.ColumnDescription.Label,
                    Name = model.ColumnDescription.Name,
                    SqlTypeEnumCode = model.ColumnDescription.Type.HasValue ? (int?)model.ColumnDescription.Type.Value : null,
                    ElementType = model.ColumnDescription.EType.ToString(),
                    InputType = model.ColumnDescription.IType.ToString(),
                    IsNullable = model.ColumnDescription.IsNull,
                    DefaultValue = model.ColumnDescription.DefaultValue,
                    TemplateId = model.TemplateId,
                    OrderIndex = model.ColumnDescription.OrderIndex
                };
                meta.AddToDynamicFields(field);
                meta.SaveChanges();
                return RedirectToAction("ViewTemplate", new { id = model.TemplateId });
            }
            return View(model);
        }

        /*public ActionResult BuildSql(int templateId, string f = "T")
        {
            var res = odata.Execute<string>(new Uri(System.Configuration.ConfigurationManager.AppSettings["url_data"]
                + "/CreateTableSmo?templateId=" + templateId + "&f='" + f + "'"), "POST", true);
            var s = (res?.FirstOrDefault() ?? "");
            if (s == "OK")
                return RedirectToAction("Index");
            else
                return Json(new { response = s }, JsonRequestBehavior.AllowGet);
        }*/

        /*public ActionResult ReloadService()
        {
            var res = odata.Execute<string>(new Uri(System.Configuration.ConfigurationManager.AppSettings["url_data"]
                + "/ReloadService"), "POST", true);
            var s = (res?.FirstOrDefault() ?? "");
            if (s == "OK")
                return RedirectToAction("Index");
            else
                return Json(new { response = s }, JsonRequestBehavior.AllowGet);
        }*/

        public ActionResult ViewTemplate(int id)
        {
            var model = new DynamicTemplateViewModel
            {
                Template = meta.DynamicTemplates.ToList().FirstOrDefault(x => x.Id == id),
                DynamicFieldViewModels = meta.DynamicFields.ToList().Where(a => a.TemplateId == id).Select(a => new DynamicFieldViewModel
                {
                    ColumnDescription = new ColumnDescription
                    {
                        Label = a.Label,
                        Name = a.Name,
                        Type = a.SqlTypeEnumCode != null ? (ColumnDescription.SqlDataType?)a.SqlTypeEnumCode.Value : null,
                        EType = (ColumnDescription.ElementType)Enum.Parse(typeof(ColumnDescription.ElementType), a.ElementType, true),
                        IType = string.IsNullOrEmpty(a.InputType) ? null : (ColumnDescription.InputType?)Enum.Parse(typeof(ColumnDescription.InputType), a.InputType, true),
                        IsNull = a.IsNullable,
                        DefaultValue = a.DefaultValue,
                        OrderIndex = a.OrderIndex
                    },
                    TemplateId = id
                }).ToList()
            };
            return View(model);
        }

        /*public ActionResult DeleteSql(int templateId, string f = "T")
        {
            var tmplObj = meta.DynamicTemplates.ToList().FirstOrDefault(x => x.Id == templateId);
            if(tmplObj != null)
            {
                if (tmplObj.IsExist == 1)
                {
                    var res = odata.Execute<string>(new Uri(System.Configuration.ConfigurationManager.AppSettings["url_data"]
                + "/DropTableSmo?templateId=" + templateId + "&f='" + f + "'"), "POST", true);
                    var s = (res?.FirstOrDefault() ?? "");
                    if (s == "OK")
                        return RedirectToAction("Index");
                    else
                        return Json(new { response = s }, JsonRequestBehavior.AllowGet);
                }
                else
                    throw new ApplicationException("This template doesn't have the table in DB");
            }
            else
                throw new ApplicationException("Template is not found!");
        }*/
        public ActionResult DeleteTemplate(int templateId)
        {
            var tmplObj = meta.DynamicTemplates.ToList().FirstOrDefault(x => x.Id == templateId);
            if (tmplObj != null)
            {
                if (tmplObj.IsExist == 1)
                {
                    throw new ApplicationException("This template has the table in DB! Drop it first.");
                }
                else if (tmplObj.IsExist == 0)
                {
                    //meta.AttachTo("DynamicTemplates", tmplObj);
                    meta.DeleteObject(tmplObj);
                    meta.SaveChanges();
                    return RedirectToAction("Index");
                }
                else throw new ApplicationException("Unknown DB-status! IsExist:" + tmplObj.IsExist);
            }
            else
                throw new ApplicationException("Template is not found!");
        }
    }
}