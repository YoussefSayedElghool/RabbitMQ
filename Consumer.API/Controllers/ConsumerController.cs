using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsumerController : ControllerBase
    {

        [HttpGet(Name = "get")]
        public ActionResult<string> Get()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                UserName = "gest",
                Password = "gest"
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            string message = "";
            var messageReceived = new ManualResetEventSlim();

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received {message}");
                messageReceived.Set();
            };

            channel.BasicConsume(queue: "hello",
                                 autoAck: true,
                                 consumer: consumer);

            messageReceived.Wait();

            return Ok(new { mesg = message });
        }

    }
}

