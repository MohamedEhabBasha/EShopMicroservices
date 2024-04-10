var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddCarter();   // this is adding the necessary services for Carter into ASP.Net code dependency injection.
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly); // that means we are going to register all services into this project into the mediator class library.
});
builder.Services.AddMarten(config =>
{
    config.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapCarter(); // mapCarter maps the routes defined in the I Carter module implementation.

app.Run();
