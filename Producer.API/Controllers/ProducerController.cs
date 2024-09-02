using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Producer.API.Dtos;
using RabbitMQ.Client;
using System.Text;

namespace Producer.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProducerController : ControllerBase
    {
        [HttpPost]
        public ActionResult<string> Send(MessageDto messageDto)
        {

            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                UserName = "gest",
                Password = "gest"
            };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = messageDto.Message;
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "hello",
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine($" [x] Sent {message}");

            Console.WriteLine(" Press [enter] to exit.");
            return Ok("done");
        }
    }
}
