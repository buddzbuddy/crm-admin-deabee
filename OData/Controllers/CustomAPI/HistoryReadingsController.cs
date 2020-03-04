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
    public class HistoryReadingsController : ControllerBase
    {
        private readonly ODataDbContext _context;

        public HistoryReadingsController(ODataDbContext context) => _context = context;
        
        [Route("api/[controller]/[action]/{patientId}")]
        [HttpGet]
        public ActionResult GetAll(int patientId)
        {
            var glucoseReadings = _context.GlucoseReadingResources.Where(x => x.UserResourceId == patientId).OrderByDescending(x => x.created);
            var weightReadings = _context.WeightReadingResources.Where(x => x.UserResourceId == patientId).OrderByDescending(x => x.created);
            var ketoneReadings = _context.KetoneReadingResources.Where(x => x.UserResourceId == patientId).OrderByDescending(x => x.created);
            var pressureReadings = _context.PressureReadingResources.Where(x => x.UserResourceId == patientId).OrderByDescending(x => x.created);
            var cholesterolReadings = _context.CholesterolReadingResources.Where(x => x.UserResourceId == patientId).OrderByDescending(x => x.created);
            var hb1acReadings = _context.HB1ACReadingResources.Where(x => x.UserResourceId == patientId).OrderByDescending(x => x.created);

            var insulinReadings = _context.InsulinReadingResources.Where(x => x.UserResourceId == patientId).Include("MealTimeResource").OrderByDescending(x => x.created);

            var foodReadings = new List<FoodItem>();
            var foodReadingsDb = _context.FoodReadingResources.Include("MealTimeResource").Include("ProductResource.QualityLevelResource").Where(x => x.UserResourceId == patientId).OrderByDescending(x => x.created).ToList();
            foreach (var reading in foodReadingsDb)
            {
                var model = new FoodItem
                {
                    id = reading.Id,
                    reading = reading.reading,
                    created = reading.created.ToString("yyyy-MM-dd HH:mm")
                };
                if (reading.ProductResource != null)
                {
                    model.productName = reading.ProductResource.Name;
                    if (reading.ProductResource.QualityLevelResource != null)
                    {
                        model.qualityColor = reading.ProductResource.QualityLevelResource.Color;
                        model.qualityName = reading.ProductResource.QualityLevelResource.Name;
                    }
                    model.breadUnit = CalcBreadUnit(reading.reading, reading.ProductResource.Id);
                }
                if (reading.MealTimeResource != null)
                {
                    model.mealtime = reading.MealTimeResource.Name;
                }
                foodReadings.Add(model);
            }
            return Ok(
                new
                {
                    glucoseReadings = glucoseReadings.Select(reading => new {
                        reading.reading,
                        created = reading.created.ToString("yyyy-MM-dd HH:mm"),
                        id = reading.Id,
                        reading.notes,
                        reading.reading_type
                    }).ToArray(),
                    weightReadings = weightReadings.Select(x => new
                    {
                        id = x.Id,
                        created = x.created.ToString("yyyy-MM-dd HH:mm"),
                        x.height,
                        x.weight
                    }).ToArray(),
                    ketoneReadings = ketoneReadings.Select(x => new
                    {
                        id = x.Id,
                        created = x.created.ToString("yyyy-MM-dd HH:mm"),
                        x.reading
                    }).ToArray(),
                    pressureReadings = pressureReadings.Select(x => new
                    {
                        id = x.Id,
                        created = x.created.ToString("yyyy-MM-dd HH:mm"),
                        x.minReading,
                        x.maxReading
                    }).ToArray(),
                    cholesterolReadings = cholesterolReadings.Select(x => new
                    {
                        id = x.Id,
                        created = x.created.ToString("yyyy-MM-dd HH:mm"),
                        x.totalReading,
                        x.HDLReading,
                        x.LDLReading
                    }).ToArray(),
                    hb1acReadings = hb1acReadings.Select(x => new
                    {
                        id = x.Id,
                        created = x.created.ToString("yyyy-MM-dd HH:mm"),
                        x.reading
                    }).ToArray(),
                    foodReadings = foodReadings.OrderByDescending(x => x.created).ToArray(),
                    insulinReadings = insulinReadings.Select(x => new
                    {
                        id = x.Id,
                        created = x.created.ToString("yyyy-MM-dd HH:mm"),
                        x.reading,
                        mealTime = x.MealTimeResource.Name,
                        x.insulinType
                    }).ToArray()
                });
        }

        double CalcBreadUnit(double reading, int productId)
        {
            double val = 0;


            var buObj = _context.BreadUnitResources.FirstOrDefault(x => x.ProductResourceId == productId);
            if(buObj != null)
            {
                val = reading / buObj.Gram * buObj.BU;
            }

            return val;
        }

        public class FoodItem
        {
            public int id { get; set; }
            public double reading { get; set; }
            public string created { get; set; }
            public string time { get; set; }
            public string productName { get; set; }
            public string qualityColor { get; set; }
            public string qualityName { get; set; }
            public string mealtime { get; set; }
            public double breadUnit { get; set; }
        }


        [Route("api/[controller]/[action]/{patientId}")]
        [HttpGet]
        public ActionResult GetByDate(int patientId, int year, int month, int day)
        {
            var fd = new DateTime(year, month, day);
            var ld = fd.AddDays(1).AddSeconds(-1);//new DateTime(year, month, day, 23, 59, 59);
            var glucoseReadings = _context.GlucoseReadingResources.Where(x => x.UserResourceId == patientId && x.created >= fd && x.created <= ld).OrderBy(x => x.created);
            var insulinReadings = _context.InsulinReadingResources.Where(x => x.UserResourceId == patientId && x.created >= fd && x.created <= ld).Include("MealTimeResource").OrderBy(x => x.created);
            var foodReadings = new List<FoodItem>();
            var foodReadingsDb = _context.FoodReadingResources.Include("MealTimeResource").Include("ProductResource.QualityLevelResource").Where(x => x.UserResourceId == patientId && x.created >= fd && x.created <= ld).OrderBy(x => x.created).ToList();
            foreach (var reading in foodReadingsDb)
            {
                var model = new FoodItem
                {
                    id = reading.Id,
                    time = reading.created.ToString("HH:mm")
                };
                if (reading.ProductResource != null)
                {
                    model.productName = reading.ProductResource.Name;
                    if (reading.ProductResource.QualityLevelResource != null)
                    {
                        model.qualityColor = reading.ProductResource.QualityLevelResource.Color;
                        model.qualityName = reading.ProductResource.QualityLevelResource.Name;
                    }
                    model.breadUnit = Math.Round(CalcBreadUnit(reading.reading, reading.ProductResource.Id), 2);
                }
                if (reading.MealTimeResource != null)
                {
                    model.mealtime = reading.MealTimeResource.Name;
                }
                foodReadings.Add(model);
            }

            var totalOrderedReadings = new List<DiaryItem>();

            totalOrderedReadings.AddRange(glucoseReadings.Select(x => new DiaryItem { created = x.created, prefix = "glucose", valueId = x.Id }));
            totalOrderedReadings.AddRange(insulinReadings.Select(x => new DiaryItem { created = x.created, prefix = "insulin", valueId = x.Id }));
            totalOrderedReadings.AddRange(foodReadingsDb.Select(x => new DiaryItem { created = x.created, prefix = "food", valueId = x.Id }));

            return Ok(
                new
                {
                    totalOrderedReadings = totalOrderedReadings.OrderBy(x => x.created).Select(x => new { x.prefix, x.valueId }).ToArray(),
                    glucoseReadings = glucoseReadings.Select(reading => new {
                        reading.reading,
                        time = reading.created.ToString("HH:mm"),
                        id = reading.Id,
                        reading.notes,
                        reading.reading_type
                    }).ToArray(),
                    insulinReadings = insulinReadings.Select(x => new
                    {
                        id = x.Id,
                        time = x.created.ToString("HH:mm"),
                        x.reading,
                        mealTime = x.MealTimeResource,
                        x.insulinType
                    }).ToArray(),
                    foodReadings = foodReadings.ToArray()
                });
        }
    }

    public class DiaryItem
    {
        public DateTime created { get; set; }
        public string prefix { get; set; }
        public int valueId { get; set; }
    }
}