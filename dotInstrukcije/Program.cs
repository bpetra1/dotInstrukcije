using dotInstrukcije.Helper;
using dotInstrukcije.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddSingleton<MongoDbService>();

builder.Services.AddSingleton<JwtTokenService>(provider =>
{
    var secretKey = builder.Configuration["Jwt:SecretKey"];
    var expiryHours = Convert.ToInt32(builder.Configuration["Jwt:ExpiryHours"]);

    return new JwtTokenService(secretKey, expiryHours);
});


var app = builder.Build();

app.UseCors(builder =>
{
    builder.WithOrigins("http://localhost:5173")
           .AllowAnyHeader()
           .AllowAnyMethod();
});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
