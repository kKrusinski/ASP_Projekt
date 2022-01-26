using EquipmentRentalService.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using EquipmentRentalService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EquipmentRentalService.Models;

namespace EquipmentRentalService
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
            services.AddControllersWithViews();

            services.AddDbContext<ApplicationDbContext>(config => config.UseSqlServer(Configuration.GetConnectionString("SQL_Database")));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.AddScoped<IRentalService, RentalService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var serviceScope = app.ApplicationServices.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            bool createdDb = dbContext.Database.EnsureCreated();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                try
                {
                    dbContext.Database.Migrate(); // Apply pending migrations in developer mode
                }
                catch (SqlException) { }
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            if (createdDb) // Fill newly created database with test records
            {


                // RentalEquipment
                var equTest = new RentalEquipment
                {
                    Name = "Koparka CAT",

                    BaseRentPrice = 250.00,
                    DailyRentPrice = 75.00,
                    OverdueRate = 15,
                    IsAvailable = false
                };

                var equTest1 = new RentalEquipment
                {
                    Name = "Koparka CAT2",

                    BaseRentPrice = 250.00,
                    DailyRentPrice = 75.00,
                    OverdueRate = 15
                };

                var equTest2 = new RentalEquipment
                {
                    Name = "Koparka CAT3",

                    BaseRentPrice = 250.00,
                    DailyRentPrice = 75.00,
                    OverdueRate = 15
                };

                // Users 
                var user_root = new IdentityUser("root");
                var user_toor = new IdentityUser("toor");

                // RentalHistory
                var rh1 = new RentalHistory
                {
                    RentingUser = user_root,
                    RentalEquipment = equTest,
                    RentedDate = DateTimeOffset.Parse("2021-12-16"),
                    RentedDue = DateTimeOffset.Parse("2021-12-30"),
                };

                var rh2 = new RentalHistory
                {
                    RentingUser = user_root,
                    RentalEquipment = equTest1,
                    RentedDate = DateTimeOffset.Parse("2021-12-12"),
                    RentedDue = DateTimeOffset.Parse("2021-12-31"),
                    ReturnedDate = DateTimeOffset.Parse("2021-12-31")
                };

                var rh3 = new RentalHistory
                {
                    RentingUser = user_toor,
                    RentalEquipment = equTest2,
                    RentedDate = DateTimeOffset.Parse("2021-12-12"),
                    RentedDue = DateTimeOffset.Parse("2021-12-20"),
                    ReturnedDate = DateTimeOffset.Parse("2021-12-20")
                };

                dbContext.Users.Add(user_root);
                dbContext.Users.Add(user_toor);
                dbContext.RentalEquipment.Add(equTest);
                dbContext.RentalEquipment.Add(equTest1);
                dbContext.RentalEquipment.Add(equTest2);
                dbContext.RentalHistory.Add(rh1);
                dbContext.RentalHistory.Add(rh2);
                dbContext.RentalHistory.Add(rh3);

                dbContext.SaveChanges();

            }

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "equipment",
                    pattern: "equipment/{controller=Equipment}/{action=Index}/{id?}");

                endpoints.MapRazorPages();

            });
        }
    }
}
