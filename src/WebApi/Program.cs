using Application.UseCases;
using Domain.Interfaces;
using Domain.Services;
using Infrastructure.Data;
using Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IAppLogger, ConsoleLogger>();

var connectionString = builder.Configuration.GetConnectionString("Sql");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Falta la cadena de conexion 'Sql'. Configura User Secrets o variables de entorno.");
}

builder.Services.AddScoped<IOrderRepository>(_ => new SqlOrderRepository(connectionString));
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<CreateOrderHandler>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

app.Use(async (ctx, next) =>
{
    try
    {
        await next();
    }
    catch (Exception)
    {
        ctx.Response.StatusCode = 500;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(new
        {
            error = "InternalServerError",
            message = "Ocurrio un error inesperado."
        });
    }
});

app.MapGet("/health", (IAppLogger logger) =>
{
    logger.Log("health ping");
    return Results.Ok(new { status = "ok" });
});

app.MapPost("/orders", (HttpContext http, CreateOrderHandler handler) =>
{
    using var reader = new StreamReader(http.Request.Body);
    var body = reader.ReadToEnd();
    var parts = (body ?? "").Split(',');

    var customer = parts.Length > 0 ? parts[0] : "anon";
    var product = parts.Length > 1 ? parts[1] : "unknown";
    var qty = parts.Length > 2 && int.TryParse(parts[2], out var parsedQty) ? parsedQty : 1;
    var price = parts.Length > 3 && decimal.TryParse(parts[3], out var parsedPrice) ? parsedPrice : 0.99m;

    var order = handler.Execute(customer, product, qty, price);
    return Results.Ok(order);
});

app.MapGet("/orders/last", (IOrderRepository repository) => repository.GetLastOrders());

await app.RunAsync();