using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Default;
using Web.OData.Models.Meta;

namespace Web.Controllers
{
    public class MenuController : Controller
    {
        Container meta = null;
        public MenuController()
        {

            meta = new Container(new Uri(System.Configuration.ConfigurationManager.AppSettings["url_meta"]));
            meta.MergeOption = MergeOption.OverwriteChanges;
        }
        // GET: Menu
        public ActionResult Index()
        {
            return View(meta.MenuItems.Expand("MenuItems").ToList().Where(x => x.ParentMenuItemId == null));
        }

        public ActionResult ViewItem(int Id)
        {
            return View(meta.MenuItems.Expand("MenuItems").ToList().Find(x => x.Id == Id));
        }
        
        public ActionResult CreateItem(int? parentMenuItemId)
        {
            return View(new MenuItem
            {
                IconName = "format_list_bulleted",
                Route = "resources",
                ParentMenuItemId = parentMenuItemId
            });
        }

        [HttpPost]
        public ActionResult CreateItem(MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                meta.AddToMenuItems(menuItem);
                meta.SaveChanges();
                return RedirectToAction("ViewItem", new { menuItem.Id });
            }
            return View(menuItem);
        }



        public ActionResult EditItem(int Id)
        {
            return View(meta.MenuItems.Expand("MenuItems").ToList().Find(x => x.Id == Id));
        }

        [HttpPost]
        public ActionResult EditItem(MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                meta.AttachTo("MenuItems", menuItem);
                meta.UpdateObject(menuItem);
                meta.SaveChanges();
                return RedirectToAction("ViewItem", new { menuItem.Id });
            }
            return View(menuItem);
        }

        public ActionResult DeleteItem(int Id)
        {
            var obj = meta.MenuItems.ToList().Find(x => x.Id == Id);
            if(obj != null)
            {
                var parentId = obj.ParentMenuItemId;
                meta.DeleteObject(obj);
                meta.SaveChanges();
                if (parentId.HasValue)
                {
                    return RedirectToAction("ViewItem", new { Id = obj.ParentMenuItemId });
                }
                return RedirectToAction("Index");
            }
            throw new ApplicationException("Id: " + Id + " not found!");
        }
    }
}