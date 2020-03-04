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
    public class AddFullReeadingsController : ControllerBase
    {
        private readonly ODataDbContext _context;

        public AddFullReeadingsController(ODataDbContext context) => _context = context;
        
        [Route("api/[controller]/[action]/{patientId}")]
        [HttpPost]
        public ActionResult Save([FromBody]FullReading request, int patientId)
        {
            try
            {
                var date = DateTime.ParseExact(request.date, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var mealtimeId = _context.MealTimeResources.FirstOrDefault(x => x.Name.Trim().ToUpper() == request.mealtime.Trim().ToUpper()).Id;
                _context.GlucoseReadingResources.Add(new GlucoseReadingResource
                {
                    created = date,
                    reading = Convert.ToDouble(request.glucose, System.Globalization.CultureInfo.InvariantCulture),
                    reading_type = request.mealtime,
                    UserResourceId = patientId,
                    notes = request.notes
                });
                _context.InsulinReadingResources.Add(new InsulinReadingResource
                {
                    created = date,
                    insulinType = 1,
                    reading = Convert.ToDouble(request.insulin_food, System.Globalization.CultureInfo.InvariantCulture),
                    MealTimeResourceId = mealtimeId,
                    UserResourceId = patientId
                });
                _context.InsulinReadingResources.Add(new InsulinReadingResource
                {
                    created = date,
                    insulinType = 2,
                    reading = Convert.ToDouble(request.insulin_levemir, System.Globalization.CultureInfo.InvariantCulture),
                    MealTimeResourceId = mealtimeId,
                    UserResourceId = patientId
                });
                foreach (var foodItem in request.food.Split('|'))
                {
                    var productName = foodItem.Split(':')[0];
                    var gramm = Convert.ToDouble(foodItem.Split(':')[1]);

                    _context.FoodReadingResources.Add(new FoodReadingResource
                    {
                        MealTimeResourceId = mealtimeId,
                        created = date,
                        ProductResourceId = GetProductIdByName(productName),
                        reading = gramm,
                        UserResourceId = patientId
                    });
                }
                _context.SaveChanges();

                return Ok(new { result = true });
            }
            catch (Exception e)
            {
                return Ok(new { result = false, errorMessage = e.Message, trace = e.StackTrace });
            }
            
        }

        [Route("api/[controller]/[action]/{patientId}")]
        [HttpPost]
        public ActionResult Save2([FromBody]FullReading2 request, int patientId)
        {
            try
            {
                var mealtimeId = _context.MealTimeResources.FirstOrDefault(x => x.Name.Trim().ToUpper() == request.mealtime.Trim().ToUpper()).Id;
                _context.GlucoseReadingResources.Add(new GlucoseReadingResource
                {
                    created = DateTime.Now,
                    reading = Convert.ToDouble(request.glucose, System.Globalization.CultureInfo.InvariantCulture),
                    reading_type = request.mealtime,
                    UserResourceId = patientId
                });
                _context.InsulinReadingResources.Add(new InsulinReadingResource
                {
                    created = DateTime.Now,
                    insulinType = 1,
                    reading = Convert.ToDouble(request.insulinFood, System.Globalization.CultureInfo.InvariantCulture),
                    MealTimeResourceId = mealtimeId,
                    UserResourceId = patientId
                });
                _context.InsulinReadingResources.Add(new InsulinReadingResource
                {
                    created = DateTime.Now,
                    insulinType = 2,
                    reading = Convert.ToDouble(request.insulinLevemir, System.Globalization.CultureInfo.InvariantCulture),
                    MealTimeResourceId = mealtimeId,
                    UserResourceId = patientId
                });

                if(!string.IsNullOrEmpty(request.foodGram_1) && !string.IsNullOrEmpty(request.foodName_1))
                _context.FoodReadingResources.Add(new FoodReadingResource
                {
                    MealTimeResourceId = mealtimeId,
                    created = DateTime.Now,
                    ProductResourceId = GetProductIdByName(request.foodName_1),
                    reading = Convert.ToDouble(request.foodGram_1),
                    UserResourceId = patientId
                });
                _context.SaveChanges();

                return Ok(new { result = true });
            }
            catch (Exception e)
            {
                return Ok(new { result = false, errorMessage = e.Message, trace = e.StackTrace });
            }

        }
        private int GetProductIdByName(string name)
        {
            var obj = _context.ProductResources.FirstOrDefault(x => x.Name.Trim().ToUpper() == name.Trim().ToUpper());
            if (obj != null) return obj.Id;
            obj = new ProductResource { Name = name };
            _context.ProductResources.Add(obj);
            _context.SaveChanges();
            return obj.Id;
        }

        [Route("api/[controller]/[action]/{patientId}")]
        [HttpPost]
        public ActionResult CheckMealtime([FromBody]CheckMealtimeRequest request, int patientId)
        {
            var date = DateTime.ParseExact(request.date, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);

            var mealtimeItems = _context.MealTimeResources.ToList();
            if (_context.GlucoseReadingResources.Any(x => x.UserResourceId == patientId && x.created.Date == date.Date && x.reading_type.Trim().ToUpper() == request.mealtime.Trim().ToUpper()))
            {
                return Ok(new { result = false });
            }

            return Ok(new { result = true });
        }

        public class FullReading
        {
            public string glucose { get; set; }
            public string insulin_food { get; set; }
            public string insulin_levemir { get; set; }
            public string food { get; set; }
            public string mealtime { get; set; }
            public string notes { get; set; }
            public string date { get; set; }

        }

        public class FullReading2
        {
            public String glucose { get; set; }
            public String insulinFood { get; set; }
            public String insulinLevemir { get; set; }
            public String mealtime { get; set; }
            public String foodName_1 { get; set; }
            public String foodGram_1 { get; set; }
            public String foodName_2 { get; set; }
            public String foodGram_2 { get; set; }
            public String foodName_3 { get; set; }
            public String foodGram_3 { get; set; }
            public String foodName_4 { get; set; }
            public String foodGram_4 { get; set; }
            public String foodName_5 { get; set; }
            public String foodGram_5 { get; set; }

        }

        public class CheckMealtimeRequest
        {
            public string mealtime { get; set; }
            public string date { get; set; }
        }
    }
}