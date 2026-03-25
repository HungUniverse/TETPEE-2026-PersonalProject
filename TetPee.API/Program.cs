using Microsoft.EntityFrameworkCore;
using TetPee.Api.Extensions;
using TetPee.API.Middlewares;
using TetPee.Repository;
using TetPee.Service.Category;
using TetPee.Service.Identity;
using TetPee.Service.JwtService;
using TetPee.Service.Product;
using TetPee.Service.Seller;
using TetPee.Service.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.EnableRetryOnFailure()
        )
    );

 builder.Services.AddJwtServices(builder.Configuration);
 builder.Services.AddSwaggerServices();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISellerService, SellerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddTransient<GlobalExceptionHandleMiddleware>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandleMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerAPI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();