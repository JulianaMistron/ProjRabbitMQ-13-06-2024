using Newtonsoft.Json;
using ProjRabbitMQ.Consumer.Services;
using ProjRabbitMQ.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

const string QUEUE_NAME = "message";//consome a fila  gerou no consumer

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())//abre a conexao com rabbit
{
    using (var channel = connection.CreateModel())
    {
        channel.QueueDeclare(queue: QUEUE_NAME,//declara a fila pra poder usar
                      durable: false,
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);

        while (true)//
        {
            var consumer = new EventingBasicConsumer(channel);//pega a fila inteira
            consumer.Received += (model, ea) =>//fica iterando o loop
            {
                var body = ea.Body.ToArray();//recepciona a informação, capturar a mensagem, pega um por um
                var returnMessage = Encoding.UTF8.GetString(body);//pega a informação em string no formAto json
                var message = JsonConvert.DeserializeObject<Message>(returnMessage);//desserializa jogando num objeto message
                Message msg = new MessageService().PostMessage(message).Result;
                Console.WriteLine("Message: " + msg.Description);
                
            };

            channel.BasicConsume(queue: QUEUE_NAME,
                                 autoAck: true,
                                 consumer: consumer);

            Thread.Sleep(2000);
        }
    }
}
