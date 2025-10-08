using API.Middleware;
using API.SignalR;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

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
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddCors();
builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
{
    var connecString = builder.Configuration.GetConnectionString("Redis")
        ?? throw new Exception("Cannot get redis connection string");
    var configuration = ConfigurationOptions.Parse(connecString, true);
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<ICartService, CartService>();
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<StoreContext>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddSignalR();

// Configure the HTTP request pipeline.
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins(
        "http://localhost:4200",
        "https://localhost:4200"
    )
    .AllowCredentials()
);

// Auth obligatoire avant SignalR
app.UseAuthentication(); //permet d’identifier l’utilisateur (via cookie, JWT, etc.).
app.UseAuthorization(); //aplique les règles [Authorize] si tu veux restreindre l’accès au hub ou à certaines méthodes.

// Servir index.html comme fichier par défaut
//sert automatiquement index.html depuis wwwroot si on appelle /
app.UseDefaultFiles();

// Servir tous les fichiers statiques (JS, CSS, assets…)
app.UseStaticFiles();

// Routes API
app.MapControllers();

// C’est la nouveauté de .NET 8 : Identity API Endpoints.
// MapIdentityApi<AppUser>() → ajoute tous les endpoints par défaut d’ASP.NET Identity 
//(login, logout, refresh, register, manage password, etc.).
// Comme tu les mets derrière MapGroup("api"), tous ces endpoints seront accessibles sous le préfixe /api/....
app.MapGroup("api").MapIdentityApi<AppUser>();

// Mapping du hub dans le environment.developement.ts du client(Angular)
app.MapHub<NotificationHub>("hub/notifications");

// Fallback : si aucune route API ou statique ne correspond
//redirige les routes inconnues vers Angular (ex: /shop/42)
//Le FallbackController est là uniquement pour gérer le routage côté Angular.
app.MapFallbackToController("Index", "Fallback");


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
