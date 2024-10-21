using API.Context.SP;
using API.Context.Table;
using API.Repository;
using API.Repository.Interfaces;
using API.Services;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Find the directory where the executable is located
var exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
var exeDirectory = Path.GetDirectoryName(exePath);

// Set up configuration
var config = new ConfigurationBuilder()
    .SetBasePath(exeDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

builder.Configuration.AddConfiguration(config); // Add shared configuratio
                                                // Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication("AuthScheme").AddCookie("AuthScheme", options =>
    {
        options.LoginPath = "/Auth/Login"; // Set the login path
        options.LogoutPath = "/Auth/Logout"; // Set the logout path
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Set the access denied path
        options.ReturnUrlParameter = "/Home/Index"; // Set the return URL parameter
    });

// Register the AuthService that implements the IAuthService interface
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IStockInRepository, StockInRepository>();
builder.Services.AddScoped<IStockInService, StockInService>();
builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();
builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<MKSSPContextProcedures>();
builder.Services.AddDbContext<MKSTableContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MKS"));
}, ServiceLifetime.Scoped);
builder.Services.AddDbContext<MKSSPContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MKS"));
});

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes("AuthScheme")
        .RequireAuthenticatedUser()
        .Build();
});
var app = builder.Build();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
