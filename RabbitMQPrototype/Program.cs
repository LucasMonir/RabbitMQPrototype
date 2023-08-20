using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using static Utils.LogTypes;

namespace RabbitMQPrototype
{
	public class RabbitReceiver
	{
		public static void Main(string[] args)
		{
			var exchangeName = "log_types";
			var factory = new ConnectionFactory { HostName = "localhost" };
			using var connection = factory.CreateConnection();
			using var channel = connection.CreateModel();

			channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);
			var queueName = channel.QueueDeclare().QueueName;

			if (args.Length < 1)
			{
				Console.WriteLine("Please run the program declaring at least one topic: program [topic]");
				Console.ReadLine();
				Environment.ExitCode = 1;
				return;
			}

			foreach (var key in args)
			{
				channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: key);
			}

			Console.WriteLine("All set up! Waiting for messages...");

			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += (model, ea) => {
				Console.WriteLine("Received!");

				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				var key = ea.RoutingKey;

				Console.WriteLine($"Request received: {key}: {message}");

				if (key == TYPES.email.ToString())
					SendEmail(message);
				else
					LogRecording(message);
			};

			channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

			Console.WriteLine("CTL+C to quit...");
			Console.ReadLine();
		}

		// Abstração de serviço de Logging
		private static void LogRecording(string message)
		{
			Thread.Sleep(2500);
			Console.WriteLine($"Message stored: {message}");
		}

		// Abstração de serviço de enviod e Email
		private static void SendEmail(string email)
		{
			Thread.Sleep(1500);
			Console.WriteLine($"Email Sent to {email}");
		}
	}
}