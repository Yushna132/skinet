using Core.Interfaces;
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


// *** Note : Durée vie d'un service *** //
//AddScoped : le service va vivre aussi longtemps que la requete HTTP
//AddTransient : Limité au niveau de la méthode, et non au niveau de la demande
//AddSingleton : le service sera créer au démarrage de l'application et ne sera supprimé qu'au moment
// où l'application s'arretera.

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Configure the HTTP request pipeline.

var app = builder.Build();

app.MapControllers();

//Inserting data in our database instead of running the command in the CLI
try
{
    //***CreateScope***//
    // Cela signifiie que tout code que nous créons et qui utilise cette variable, une fois
    // qu'il a fini de s'éxécuter, le framework va se débaresser de tous les services que nous
    // avons utilisés. Parceque nous l'utilisons en dehors du contexte de l'injection de dépendances
    // Lorsque nous utilison des services à l'interieur de ProductController en utilisant le dépendances,
    // c'est au framework de les instancier et de s'en débarraser une fois qu'il est en dehors du champ
    // de l'application.
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    // on apelle notre bd
    var context = services.GetRequiredService<StoreContext>();
    //Cette methode applique de manière asynchrone toutes les migrations en attente et elle crée
    //la db si elle n'existe pas déjà.
    await context.Database.MigrateAsync();
    //populating our db
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw;
}


app.Run();
