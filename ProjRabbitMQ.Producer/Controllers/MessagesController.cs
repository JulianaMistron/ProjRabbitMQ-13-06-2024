using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjRabbitMQ.Models;
using ProjRabbitMQ.Producer.Data;
using RabbitMQ.Client;

namespace ProjRabbitMQ.Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ConnectionFactory _factory;
        private const string QUEUE_NAME = "message";


        public MessagesController(ConnectionFactory factory)
        {
            _factory = factory;
        }

        [HttpPost]
        public IActionResult PostMQMessage([FromBody] Message message)//from body:forma de representar q esta recebendo informação
        {
            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {

                    channel.QueueDeclare(
                        queue: QUEUE_NAME,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                        );

                    var stringfieldMessage = JsonConvert.SerializeObject(message);//transofrma em arquivo json
                    var bytesMessage = Encoding.UTF8.GetBytes(stringfieldMessage);//transforma em bytes

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: QUEUE_NAME,
                        basicProperties: null,
                        body: bytesMessage
                        );
                }
            }
            return Accepted();
        }
    }
}
