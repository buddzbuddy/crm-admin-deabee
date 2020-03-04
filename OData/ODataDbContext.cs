using Microsoft.EntityFrameworkCore;
using OData.Models.Data;
using OData.Models.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OData
{
    public class ODataDbContext : DbContext
    {
        public ODataDbContext(DbContextOptions<ODataDbContext> options)
               : base(options)
        {

        }

        public DbSet<DynamicTemplate> DynamicTemplates { get; set; }
        public DbSet<DynamicField> DynamicFields { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        //DEABEE
        public DbSet<UserResource> UserResources { get; set; }
        public DbSet<UserTypeResource> UserTypeResources { get; set; }

        public DbSet<CholesterolReadingResource> CholesterolReadingResources { get; set; }
        public DbSet<GlucoseReadingResource> GlucoseReadingResources { get; set; }
        public DbSet<HB1ACReadingResource> HB1ACReadingResources { get; set; }
        public DbSet<KetoneReadingResource> KetoneReadingResources { get; set; }
        public DbSet<PressureReadingResource> PressureReadingResources { get; set; }
        public DbSet<WeightReadingResource> WeightReadingResources { get; set; }
        public DbSet<InsulinReadingResource> InsulinReadingResources { get; set; }

        public DbSet<ProductResource> ProductResources { get; set; }
        public DbSet<QualityLevelResource> QualityLevelResources { get; set; }
        public DbSet<BreadUnitResource> BreadUnitResources { get; set; }
        public DbSet<FoodReadingResource> FoodReadingResources { get; set; }
        public DbSet<MealTimeResource> MealTimeResources { get; set; }
        public DbSet<DoctorUserResource> DoctorUserResources { get; set; }
        public DbSet<TextMessageResource> TextMessageResources { get; set; }
        public DbSet<NotificationResource> NotificationResources { get; set; }
        public DbSet<ProductPhotoResource> ProductPhotoResources { get; set; }
        public DbSet<DrugTypeResource> DrugTypeResources { get; set; }
        public DbSet<DrugResource> DrugResources { get; set; }
        public DbSet<ICDResource> ICDResources { get; set; }
    }
}
