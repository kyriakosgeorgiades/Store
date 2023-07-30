using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Store;
using Store.Context;
using Store.Interface;
using Store.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<Seed>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
    builder.Configuration.AddJsonFile("appsettings.Local.json", true, true);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddCors();


var app = builder.Build();

app.UseCors(builder => builder.AllowAnyOrigin());

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
dbContext.Database.Migrate();


// Check if the Books table is empty
if (!dbContext.Books.Any())
{
    SeedData(app); // If empty, run the seed logic
}

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Seed>>();
    logger.LogInformation("Beginning data seeding.");


    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<Seed>();
        service.SeedData();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
