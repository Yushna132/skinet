using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Console.WriteLine("=== EF DEBUG ===");
//Console.WriteLine("ENV=" + builder.Environment.EnvironmentName);
//Console.WriteLine("CS=" + builder.Configuration.GetConnectionString("DefaultConnection"));

// Add services to the container.

builder.Services.AddControllers();


// EF Core + SQL Server
builder.Services.AddDbContext<StoreContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure the HTTP request pipeline.

var app = builder.Build();

app.MapControllers();

app.Run();
