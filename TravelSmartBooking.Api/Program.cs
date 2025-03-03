using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelSmartBooking.Api.Data;
using TravelSmartBooking.Api.Interfaces;
using TravelSmartBooking.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

builder.Services.AddControllers();
builder.Services.AddDbContext<TravelSmartContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TravelSmartDatabase"))
);

builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingFacade, BookingFacade>();
builder.Services.AddScoped<IPackageFactory, PackageFactory>();

builder.Services.AddTransient<
    IConfirmationSenderStrategyFactory,
    ConfirmationSenderStrategyFactory
>();
builder.Services.AddTransient<IConfirmationSenderStrategy, EmailConfirmationSenderStrategy>();
builder.Services.AddTransient<IConfirmationSenderStrategy, SmsConfirmationSenderStrategy>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.MapGet(
    "/index.html",
    async context =>
    {
        await context.Response.SendFileAsync(
            Path.Combine(app.Environment.WebRootPath, "html/index.html")
        );
    }
);

app.Run();
