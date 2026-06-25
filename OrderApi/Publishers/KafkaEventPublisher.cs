using Confluent.Kafka;
using OrderApi.Publishers.Interfaces;
using System.Text.Json;

namespace OrderApi.Publishers;

public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventPublisher> _logger;

    public KafkaEventPublisher(
        ILogger<KafkaEventPublisher> logger)
    {
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
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