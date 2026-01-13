using Microsoft.EntityFrameworkCore;
using WasmProject.Models;
using Microsoft.AspNetCore.StaticFiles; 

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<WasmProject.Services.PropertySyncService>();
builder.Services.AddDataProtection();


builder.Services.AddDbContext<WasmDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".Wasm.Session";
});


builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();


var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".mp4"] = "video/mp4"; 

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});


app.UseRouting();


app.UseCookiePolicy();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();