using Microsoft.EntityFrameworkCore;
using Quartz;
using TetPee.Api.Extensions;
using TetPee.Api.Middlewares;

using TetPee.Repository;
using TetPee.Service.BackgroundJobService;
using TetPee.Service.Cart;
using TetPee.Service.Category;
using TetPee.Service.CloudinaryService;
using TetPee.Service.Identity;
using TetPee.Service.JwtService;
using TetPee.Service.MailService;
using TetPee.Service.MediaService;
using TetPee.Service.Order;
using TetPee.Service.Product;
using TetPee.Service.Seller;
using TetPee.Service.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

 builder.Services.AddJwtServices(builder.Configuration);
 builder.Services.AddSwaggerServices();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISellerService, SellerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IMediaService, CloudinaryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddQuartz(options =>
{
    var jobKey = new JobKey(nameof(ProcessTransactionPendingJob));

    options
        .AddJob<ProcessTransactionPendingJob>(jobKey)
        .AddTrigger(trigger =>
            trigger
                .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithIntervalInMinutes(2)
                    .RepeatForever()
                )
        );
});
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerAPI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();