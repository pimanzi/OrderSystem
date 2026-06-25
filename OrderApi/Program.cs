using OrderApi.Publishers;
using OrderApi.Publishers.Interfaces;
using OrderApi.Repository;
using OrderApi.Services;
using OrderApi.Services.Interfaces;
using OrderApi.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<OrderStore>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("Kafka"));

builder.Services
    .AddSingleton<IEventPublisher,
        KafkaEventPublisher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();