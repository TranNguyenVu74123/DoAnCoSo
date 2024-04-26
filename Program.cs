using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Owl.reCAPTCHA;
using WEBSAIGONGLISTEN.Models;
using WEBSAIGONGLISTEN.Repositories;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddDefaultTokenProviders()
        .AddDefaultUI()
        .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/LogoutPath";
    options.LogoutPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddRazorPages();

// ??t tr??c AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
// Trong ph??ng th?c ConfigureServices c?a class Startup
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<IPasswordHasher<ApplicationUser>, CustomPasswordHasher<ApplicationUser>>();

builder.Services.AddreCAPTCHAV2(x =>
{
    x.SiteKey = "6LcorbkpAAAAAOSZZ5kTimR5bsbRmU4TeErp84bO";
    x.SiteSecret = "6LcorbkpAAAAAC5oGihIiVHY50_IsXRSnxrQUnHA";
});

builder.Services.AddAuthentication()
   .AddGoogle(options =>
   {
       IConfigurationSection googleAuthNSection = config.GetSection("Authentication:Google");
       options.ClientId = googleAuthNSection["ClientId"];
       options.ClientSecret = googleAuthNSection["ClientSecret"];
   })
   .AddFacebook(options =>
   {
       IConfigurationSection FBAuthNSection = config.GetSection("Authentication:Facebook");
       options.AppId = FBAuthNSection["AppId"];
       options.AppSecret = FBAuthNSection["AppSecret"];
   });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseSession();
// ??t tr??c UseRouting



app.UseRouting();

app.UseAuthentication(); ;
app.UseAuthorization();


//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");


//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//      name: "Admin",
//      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
//    endpoints.MapControllerRoute(
//        name: "Company",
//        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
//    endpoints.MapControllerRoute(
//      name: "default",
//      pattern: "{controller=Home}/{action=Index}/{id?}");
//    endpoints.MapControllerRoute(
//       name: "search",
//       pattern: "product/search",
//       defaults: new { controller = "Product", action = "Search" },
//       constraints: new { httpMethod = new HttpMethodRouteConstraint("POST") }
//   );
//});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "AdminArea",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "AdminProductSearch",
        pattern: "{area:exists}/{controller=Product}/search",  // ???ng d?n ??n action "Search"
        defaults: new { action = "Search" },
        constraints: new { area = "Admin", httpMethod = new HttpMethodRouteConstraint("POST") }
    );

    endpoints.MapControllerRoute(
       name: "AdminCategorySearch",
       pattern: "{area:exists}/{controller=Product}/searchcategory",  // ???ng d?n ??n action "SearchCategory"
       defaults: new { controller = "Product", action = "SearchCategory" },  // Th�m action "SearchCategory" ? ?�y
       constraints: new { area = "Admin", httpMethod = new HttpMethodRouteConstraint("POST") }
   );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});



app.MapRazorPages();
app.Run();