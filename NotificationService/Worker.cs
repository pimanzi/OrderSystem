using Confluent.Kafka;
using Shared.Events;
using System.Text.Json;

namespace NotificationService;

public class NotificationWorker : BackgroundService
{
    private readonly ILogger<NotificationWorker> _logger;
    private readonly IConsumer<string, string> _consumer;

    public NotificationWorker(
        ILogger<NotificationWorker> logger,
        IConfiguration configuration)
    {
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = "notification-service-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _consumer = new ConsumerBuilder<string, string>(config)
            .Build();
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _consumer.Subscribe("order-events");

        _logger.LogInformation(
            "NotificationWorker started, " +
            "listening to order-events topic...");

        await Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);

                    if (result?.Message?.Value is null)
                        continue;

                    HandleMessage(result.Message.Value);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error consuming message");
                }
            }
        }, stoppingToken);

        _consumer.Close();
        _logger.LogInformation("NotificationWorker stopped!");
    }

    private void HandleMessage(string messageValue)
    {
        var envelope = JsonSerializer
            .Deserialize<JsonElement>(messageValue);

        var eventType = envelope
            .GetProperty("EventType")
            .GetString();

        switch (eventType)
        {
            case "OrderPlaced":
                var placed = JsonSerializer
                    .Deserialize<OrderPlacedEvent>(messageValue);
                _logger.LogInformation(
                    "NOTIFICATION: Order {OrderId} placed " +
                    "by {CustomerName} - Total: {Total}",
                    placed!.OrderId,
                    placed.CustomerName,
                    placed.TotalAmount);
                break;

            case "OrderConfirmed":
                var confirmed = JsonSerializer
                    .Deserialize<OrderConfirmedEvent>(messageValue);
                _logger.LogInformation(
                    "NOTIFICATION: Order {OrderId} confirmed " +
                    "for {CustomerName}!",
                    confirmed!.OrderId,
                    confirmed.CustomerName);
                break;

            case "OrderShipped":
                var shipped = JsonSerializer
                    .Deserialize<OrderShippedEvent>(messageValue);
                _logger.LogInformation(
                    "NOTIFICATION: Order {OrderId} shipped " +
                    "to {CustomerName}! " +
                    "Tracking: {TrackingNumber}",
                    shipped!.OrderId,
                    shipped.CustomerName,
                    shipped.TrackingNumber);
                break;

            default:
                _logger.LogWarning(
                    "Unknown event type: {EventType}",
                    eventType);
                break;
        }
    }
}