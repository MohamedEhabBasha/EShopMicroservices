var builder = WebApplication.CreateBuilder(args);

// Add services to the container

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    // that means we are going to register all services into this project into the mediator class library.
    config.RegisterServicesFromAssembly(assembly);
    // we basically add our validation behavior as a pipeline behavior into mediator. (executes before handle method)
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));

    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);

// this is adding the necessary services for Carter into ASP.Net code dependency injection.
builder.Services.AddCarter();

builder.Services.AddMarten(config =>
{
    config.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();
// Seeding data only in case of development enviroment
if(builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<CatalogInitialData>();


builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

var app = builder.Build();

// Configure the HTTP request pipeline
// mapCarter maps the routes defined in the I Carter module implementation.
app.MapCarter();

//  we configure the application to use our custom exception handler, the empty option parameter indicates that we are relying on custom configured handler.
app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        //Return the response as json format
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();
