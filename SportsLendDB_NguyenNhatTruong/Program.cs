
using Microsoft.AspNetCore.Authentication.Cookies;
using SportsLend.BLL.Models;
using SportsLend.BLL.Repository;
using SportsLend.BLL.Service;
using SportsLendDB_NguyenNhatTruong.Hubs;

namespace SportsLendDB_NguyenNhatTruong
{
    public class Program
    {
        public static void Main(string[] args)
        {


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<EquipmentService>();

            builder.Services.AddSignalR();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/Authetication/Login";
                        options.AccessDeniedPath = "/Authetication/AccessDenied";
                        options.LogoutPath = "/Authetication/Logout";
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                        options.SlidingExpiration = false;

                        options.Cookie.IsEssential = true;
                        options.Cookie.HttpOnly = true;
                    });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.MapHub<SignalRHubcs>("/equipmentHub");

            app.Run();
        }
    }
}