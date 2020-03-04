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
    public class FoodReadingExplainController : ControllerBase
    {
        private readonly ODataDbContext _context;

        public FoodReadingExplainController(ODataDbContext context) => _context = context;
        
        [Route("api/[controller]/[action]/{patientId}")]
        [HttpGet]
        public ActionResult<ExplainItem[]> Get(int patientId)
        {
            var modelItems = new List<ExplainItem>();
            var foodReadings = _context.FoodReadingResources.Include("MealTimeResource").Include("ProductResource.QualityLevelResource").Where(x => x.UserResourceId == patientId);
            foreach (var reading in foodReadings)
            {
                var model = new ExplainItem
                {
                    reading = reading.reading,
                    created = reading.created
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
                modelItems.Add(model);
            }
            
            return Ok(modelItems.ToArray());
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

        [Route("api/[controller]/[action]")]
        [HttpGet]
        public ActionResult<double> CalcBU(double reading, int productId)
        {
            return CalcBreadUnit(reading, productId);
        }

        [Route("api/[controller]/[action]")]
        [HttpPost]
        public ActionResult CalcBUStr([FromBody]BUCalcItem request)
        {
            var result = new List<string>();
            foreach (var foodItem in request.formattedFoods.Split('|'))
            {
                var productName = foodItem.Split(':')[0];
                var gramm = Convert.ToDouble(foodItem.Split(':')[1]);
                var productObj = _context.ProductResources.FirstOrDefault(x => x.Name.Trim().ToUpper() == productName.Trim().ToUpper());
                double bu = 0.0;
                if (productObj != null)
                {
                    var productId = productObj.Id;
                    bu = CalcBreadUnit(gramm, productId);
                }
                result.Add(string.Format("{0} - {1} гр. ({2} ХЕ)", productName, gramm, bu > 0 ? Math.Round(bu, 1).ToString() : "-"));
            }
            return Ok(new { result = true, text = string.Join(", ", result) });
        }

        public class ExplainItem
        {
            public double reading { get; set; }
            public DateTime created { get; set; }
            public string productName { get; set; }
            public string qualityColor { get; set; }
            public string qualityName { get; set; }
            public string mealtime { get; set; }
            public double breadUnit { get; set; }
        }
        public class BUCalcItem
        {
            public string formattedFoods { get; set; }
        }
    }
}