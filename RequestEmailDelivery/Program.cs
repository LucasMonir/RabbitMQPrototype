using RabbitMQ.Client;
using System.Text;
using static Utils.LogTypes;

namespace RequestEmailDelivery
{
	internal class Program
	{
		// Mensagens pré-montadas de email
		private static readonly List<string> EmailMessages = new() {
			"Hello! New order stablished", 
			"Critical Error! The manager authorized deploying on a friday!", 
			"Update: The sistem is now working properly" };
		
		// Mensagens pré-montadas de logging
		private static readonly List<string> InfoMessagges = new() { 
			"Program crashed", 
			"The program initialized", 
			"Service unavailable" };

		private static void Main(string[] args)
		{
			var factory = new ConnectionFactory { HostName = "localhost" };
			using var connection = factory.CreateConnection();
			using var channel = connection.CreateModel();
			var exchangeName = "log_types";
			channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

			var type = GetType(args);
			var message = GetRandomMessage(type);

			var body = Encoding.UTF8.GetBytes(message);

			Console.WriteLine("Type anything to send a random message!");
			Console.ReadLine();

			channel.BasicPublish(exchange: exchangeName,
					 routingKey: type,
					 basicProperties: null,
					 body: body);


			Console.WriteLine($" [x] Message successfully sent! '{type}':'{message}'");
			Console.WriteLine(" Press [enter] to exit.");
			Console.ReadLine();
		}

		// Procurar tipo de mensagem
		private static string GetType(string[] args)
		{
			var correspondingEnum = string.Empty;
			if (args.Length > 0)
			{
				var enums = GetEnums();
				correspondingEnum = enums.Where(x => x.ToString().ToLower() == args[0].ToLower()).First().ToString();
			}

			return !string.IsNullOrEmpty(correspondingEnum) ? correspondingEnum: TYPES.info.ToString();
		}

		// Buscar mensagem aleatória
		private static string GetRandomMessage(string type)
		{
			var random = new Random();
			return type == TYPES.email.ToString() ? EmailMessages[random.Next(EmailMessages.Count)] : InfoMessagges[random.Next(InfoMessagges.Count)];
		}
	}
}