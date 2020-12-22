using System.Data.SqlTypes;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RobonautCasparClient_v2.DO;
using RobonautCasparClient_v2.modules.interfaces;

namespace RobonautCasparClient_v2.modules
{
    public class RabbitMqDataInteractor : IDataServerInteractor
    {
        #region Singleton implementation

        private static RabbitMqDataInteractor instance;

        public static RabbitMqDataInteractor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RabbitMqDataInteractor();
                }

                return instance;
            }
        }

        #endregion

        private const string CONTEST_EVENTS_QUEUE_NAME = "contestEvents";

        private TeamDataService teamDataService = TeamDataService.Instance;

        public int Year { get; set; }

        private ConnectionFactory factory;
        private IConnection connection;
        private IModel contestEventsChannel;

        public override void setYear(int year)
        {
            Year = year;
        }

        private bool IsConnected
        {
            get
            {
                if (connection != null)
                {
                    return connection.IsOpen;
                }

                return false;
            }
        }

        public override void connect(string serverUrl)
        {
            factory = new ConnectionFactory() {HostName = serverUrl, ClientProvidedName = "GraphicsClient"};

            connection = factory.CreateConnection();
            contestEventsChannel = connection.CreateModel();

            contestEventsChannel.QueueDeclare(queue: CONTEST_EVENTS_QUEUE_NAME,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(contestEventsChannel);
            consumer.Received += eventRecieved;

            contestEventsChannel.BasicConsume(queue: CONTEST_EVENTS_QUEUE_NAME,
                autoAck: true,
                consumer: consumer);

            connection.ConnectionShutdown += disconnected;

            if (IsConnected)
                fireConnectedEvent();
            else
                fireConnectionFailedEvent();
        }

        private void disconnected(object sender, ShutdownEventArgs ea)
        {
            fireDisconnectedEvent();
        }

        private void eventRecieved(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = JObject.Parse(Encoding.UTF8.GetString(body));
        }

        public override void disconnect()
        {
            if (IsConnected)
            {
                connection.Close();
                fireDisconnectedEvent();
            }
        }

        public override void requestTeamDataUpdate()
        {
            if (IsConnected && contestEventsChannel.IsOpen)
            {
                string message = "{\"type\": \"getTeams\",\"year\": " + Year + "}";
                var body = Encoding.UTF8.GetBytes(message);

                contestEventsChannel.BasicPublish(exchange: "",
                    routingKey: CONTEST_EVENTS_QUEUE_NAME,
                    basicProperties: null,
                    body: body);
            }
        }
    }
}