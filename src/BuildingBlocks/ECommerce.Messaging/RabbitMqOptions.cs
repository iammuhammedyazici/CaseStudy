namespace ECommerce.Messaging;

public class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string ExchangeName { get; set; } = "ecommerce.events";
    public int RetryTtlSeconds { get; set; } = 10;
    public int MaxRetries { get; set; } = 5;
}
