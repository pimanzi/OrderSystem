using Confluent.Kafka;
using OrderApi.Publishers.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OrderApi.Settings;

namespace OrderApi.Publishers;

public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventPublisher> _logger;
    private readonly KafkaSettings _settings;

    public KafkaEventPublisher(
        ILogger<KafkaEventPublisher> logger, IOptions<KafkaSettings> settings
    )
    {
        _logger = logger;
        _settings = settings.Value;

        var config = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            Acks = Acks.All
        };

        _producer = new ProducerBuilder<string, string>(config)
            .Build();
    }

    public async Task PublishAsync<T>(
        string topic, T eventData) where T : class
    {
        var message = JsonSerializer.Serialize(eventData);

        await _producer.ProduceAsync(topic,
            new Message<string, string>
            {
                Key = GetKey(eventData),
                Value = message
            });

        _logger.LogInformation(
            "Published {EventType} to {Topic}",
            eventData.GetType().Name,
            topic);
    }

    private string GetKey<T>(T eventData)
    {
        var property = typeof(T)
            .GetProperty("OrderId");

        return property?.GetValue(eventData)
            ?.ToString() ?? Guid.NewGuid().ToString();
    }
}