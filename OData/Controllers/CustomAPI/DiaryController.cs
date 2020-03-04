using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OData.Models.Data;

namespace OData.Controllers.CustomAPI
{
    [ApiController]
    public class DiaryController : ControllerBase
    {
        private readonly ODataDbContext _context;

        public DiaryController(ODataDbContext context) => _context = context;
        
        [Route("api/[controller]/[action]")]
        [HttpGet]
        public ActionResult<DiaryItem[]> Get(int patientId, string dateStr)
        {
            var date = DateTime.ParseExact(dateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);

            var glucoseItems = _context.GlucoseReadingResources.Where(x => x.UserResourceId == patientId && x.created.Date == date.Date).ToList();
            var insulinItems = _context.InsulinReadingResources.Where(x => x.UserResourceId == patientId && x.created.Date == date.Date).ToList();
            var foodItems = _context.FoodReadingResources.Include("ProductResource").Where(x => x.UserResourceId == patientId && x.created.Date == date.Date).ToList();

            var mealtimeItems = _context.MealTimeResources.ToList();

            var model = new List<DiaryItem>();
            foreach (var mealtime in mealtimeItems.OrderBy(x => x.Id))
            {
                var modelItem = new DiaryItem
                {
                    Time = GetTime(mealtime.Id),
                    Mealtime = mealtime.Name
                };

                //Glucose
                var glucose = glucoseItems.FirstOrDefault(x => x.reading_type == mealtime.Name);
                if(glucose != null)
                {
                    modelItem.Glucose = glucose.reading.ToString();
                }

                //Insulin
                var insulinFood = insulinItems.FirstOrDefault(x => x.MealTimeResourceId == mealtime.Id && x.insulinType == 1);
                var insulinLevemir = insulinItems.FirstOrDefault(x => x.MealTimeResourceId == mealtime.Id && x.insulinType == 2);
                if(insulinFood != null)
                {
                    if(insulinLevemir != null)
                    {
                        modelItem.Insulin = string.Format("{0} ед. | {1} ед. (левемир)", insulinFood.reading, insulinLevemir.reading);
                    }
                    else
                    {
                        modelItem.Insulin = string.Format("{0} ед.", insulinFood.reading);
                    }
                }

                //Food
                var foods = foodItems.Where(x => x.MealTimeResourceId == mealtime.Id);
                var foodStr = new List<string>();
                double buTotal = 0;
                foreach (var foodItem in foods)
                {
                    var productName = foodItem.ProductResource.Name;
                    var gramm = foodItem.reading;
                    
                    double bu = CalcBreadUnit(gramm, foodItem.ProductResourceId.Value); ;
                    foodStr.Add(string.Format("{0} - {1} гр. ({2} ХЕ)", productName, gramm, bu > 0 ? Math.Round(bu, 1).ToString() : "-"));
                    buTotal += bu;
                }
                modelItem.Food = string.Join(", ", foodStr);
                modelItem.BreadUnitTotal = Math.Round(buTotal, 1).ToString();
                model.Add(modelItem);
            }

            return Ok(model);
        }


        [Route("api/[controller]/[action]")]
        [HttpGet]
        public ActionResult<DiaryItemFlutter[]> GetForFlutter(int patientId, string dateStr)
        {
            var date = DateTime.ParseExact(dateStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            var glucoseItems = _context.GlucoseReadingResources.Where(x => x.UserResourceId == patientId && x.created.Date == date.Date).ToList();
            var insulinItems = _context.InsulinReadingResources.Where(x => x.UserResourceId == patientId && x.created.Date == date.Date).ToList();
            var foodItems = _context.FoodReadingResources.Include("ProductResource").Where(x => x.UserResourceId == patientId && x.created.Date == date.Date).ToList();

            var mealtimeItems = _context.MealTimeResources.ToList();

            var model = new List<DiaryItemFlutter>();
            foreach (var mealtime in mealtimeItems.OrderBy(x => x.Id))
            {
                var modelItem = new DiaryItemFlutter();

                //Glucose
                var glucose = glucoseItems.FirstOrDefault(x => x.reading_type == mealtime.Name);
                if (glucose != null)
                {
                    modelItem.Glucose = glucose.reading.ToString();
                }

                //Insulin
                var insulinFood = insulinItems.FirstOrDefault(x => x.MealTimeResourceId == mealtime.Id && x.insulinType == 1);
                var insulinLevemir = insulinItems.FirstOrDefault(x => x.MealTimeResourceId == mealtime.Id && x.insulinType == 2);
                if (insulinFood != null)
                {
                    if (insulinLevemir != null)
                    {
                        modelItem.Insulin = string.Format("{0} ед. | {1} ед. (левемир)", insulinFood.reading, insulinLevemir.reading);
                    }
                    else
                    {
                        modelItem.Insulin = string.Format("{0} ед.", insulinFood.reading);
                    }
                }

                //Food
                var foods = foodItems.Where(x => x.MealTimeResourceId == mealtime.Id);
                var foodStr = new List<string>();
                double buTotal = 0;
                foreach (var foodItem in foods)
                {
                    var productName = foodItem.ProductResource.Name;
                    var gramm = foodItem.reading;

                    double bu = CalcBreadUnit(gramm, foodItem.ProductResourceId.Value); ;
                    foodStr.Add(string.Format("{0} - {1} гр. ({2} ХЕ)", productName, gramm, bu > 0 ? Math.Round(bu, 1).ToString() : "-"));
                    buTotal += bu;
                }
                modelItem.Food = string.Join(", ", foodStr);
                modelItem.BreadUnitTotal = Math.Round(buTotal, 1).ToString();
                model.Add(modelItem);
            }

            return Ok(model);
        }

        double CalcBreadUnit(double reading, int productId)
        {
            double val = 0;


            var buObj = _context.BreadUnitResources.FirstOrDefault(x => x.ProductResourceId == productId);
            if (buObj != null)
            {
                val = reading / buObj.Gram * buObj.BU;
            }

            return val;
        }

        static string GetTime(int mealtime)
        {
            switch (mealtime)
            {
                case 1:
                    return "08:30";
                case 2:
                    return "12:30";
                case 3:
                    return "15:30";
                case 4:
                    return "19:00";
                case 5:
                    return "11:00";
                default:
                    return "-:-";
            }
        }
        public class DiaryItem
        {
            public string Time { get; set; }
            public string Mealtime { get; set; }
            public string Glucose { get; set; }
            public string Insulin { get; set; }
            public string Food { get; set; }
            public string BreadUnitTotal { get; set; }
            public string Notes { get; set; }
        }
        public class DiaryItemFlutter
        {
            public string Glucose { get; set; }
            public string Insulin { get; set; }
            public string Food { get; set; }
            public string BreadUnitTotal { get; set; }
            public string Notes { get; set; }
        }
    }
}