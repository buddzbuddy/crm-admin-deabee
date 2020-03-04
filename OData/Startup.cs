using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using OData.Models.Data;
using OData.Models.Meta;

namespace OData
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ODataDbContext>(options =>
                   options.UseSqlServer(
                       Configuration.GetConnectionString("DefaultConnection")));

            services.AddOData();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(builder =>
            {
                builder.Select().Expand().Filter().OrderBy().MaxTop(1000).Count();
                builder.MapODataServiceRoute("odata", "odata", GetEdmModelData());
                builder.MapODataServiceRoute("ometa", "ometa", GetEdmModelMeta());
            });
            
        }
        private static IEdmModel GetEdmModelData()
        {
            var builder = new ODataConventionModelBuilder();
            //Data
            //DEABEE
            builder.EntitySet<UserResource>("UserResources");
            builder.EntitySet<UserTypeResource>("UserTypeResources");
            builder.EntitySet<CholesterolReadingResource>("CholesterolReadingResources");
            builder.EntitySet<GlucoseReadingResource>("GlucoseReadingResources");
            builder.EntitySet<HB1ACReadingResource>("HB1ACReadingResources");
            builder.EntitySet<KetoneReadingResource>("KetoneReadingResources");
            builder.EntitySet<PressureReadingResource>("PressureReadingResources");
            builder.EntitySet<WeightReadingResource>("WeightReadingResources");

            builder.EntitySet<ProductResource>("ProductResources");
            builder.EntitySet<QualityLevelResource>("QualityLevelResources");
            builder.EntitySet<BreadUnitResource>("BreadUnitResources");
            builder.EntitySet<FoodReadingResource>("FoodReadingResources");
            builder.EntitySet<MealTimeResource>("MealTimeResources");
            builder.EntitySet<DoctorUserResource>("DoctorUserResources");
            builder.EntitySet<TextMessageResource>("TextMessageResources");
            builder.EntitySet<NotificationResource>("NotificationResources");
            builder.EntitySet<ProductPhotoResource>("ProductPhotoResources");
            builder.EntitySet<InsulinReadingResource>("InsulinReadingResources");
            builder.EntitySet<DrugTypeResource>("DrugTypeResources");
            builder.EntitySet<DrugResource>("DrugResources");
            builder.EntitySet<ICDResource>("ICDResources");


            return builder.GetEdmModel();
        }
        private static IEdmModel GetEdmModelMeta()
        {
            var builder = new ODataConventionModelBuilder();
            //Meta
            builder.EntitySet<DynamicField>("DynamicFields");
            builder.EntitySet<DynamicTemplate>("DynamicTemplates");
            builder.EntitySet<MenuItem>("MenuItems");
            return builder.GetEdmModel();
        }
    }
}
