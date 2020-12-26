using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RobonautCasparClient_v2.DO;
using RobonautCasparClient_v2.DO.communication;
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

        private const string TEAM_DATA_REQUEST_QUEUE_KEY = "general.teamData";
        private const string TEAM_DATA_QUEUE_KEY = "team.teamData";
        private const string SKILL_TIMER_QUEUE_KEY = "skill.timer";
        private const string SKILL_GATE_QUEUE_KEY = "skill.gate";
        private const string SAFETY_CAR_FOLLOW_QUEUE_KEY = "speed.safetyCar.follow";
        private const string SAFETY_CAR_OVERTAKE_QUEUE_KEY = "speed.safetyCar.overtake";
        private const string SPEED_TIMER_QUEUE_KEY = "speed.timer";
        private const string SPEED_LAP_QUEUE_KEY = "speed.lap";

        private TeamDataService teamDataService = TeamDataService.Instance;

        public int Year { get; set; }

        private ConnectionFactory factory;
        private IConnection connection;
        private IModel contestConnection;

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
            factory = new ConnectionFactory() {HostName = serverUrl, ClientProvidedName = "Robonaut Graphics Client"};

            try
            {
                connection = factory.CreateConnection();
            }
            catch (Exception e)
            {
                fireConnectionFailedEvent();
                return;
            }

            contestConnection = connection.CreateModel();

            subscribeToQueue(TEAM_DATA_QUEUE_KEY, teamDataRecieved);
            subscribeToQueue(SKILL_TIMER_QUEUE_KEY, skillTimerDataRecieved);
            subscribeToQueue(SKILL_GATE_QUEUE_KEY, skillGateDataRecieved);
            subscribeToQueue(SAFETY_CAR_FOLLOW_QUEUE_KEY, safetyCarFollowInformationRecieved);
            subscribeToQueue(SAFETY_CAR_OVERTAKE_QUEUE_KEY, safetyCarOvertakeInformationRecieved);
            subscribeToQueue(SPEED_TIMER_QUEUE_KEY, speedTimerDataRecieved);
            subscribeToQueue(SPEED_LAP_QUEUE_KEY, speedLapDataRecieved);

            connection.ConnectionShutdown += disconnected;

            if (IsConnected)
            {
                fireConnectedEvent();
                requestTeamDataUpdate();
            }
            else
                fireConnectionFailedEvent();
        }

        private void disconnected(object sender, ShutdownEventArgs ea)
        {
            fireDisconnectedEvent();
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
            if (IsConnected && contestConnection.IsOpen)
            {
                string message = "{\"requesterName\": \"GraphicsClient\" }";
                var body = Encoding.UTF8.GetBytes(message);

                contestConnection.BasicPublish(exchange: "",
                    routingKey: TEAM_DATA_REQUEST_QUEUE_KEY,
                    basicProperties: null,
                    body: body);
            }
        }

        private void teamDataRecieved(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = JObject.Parse(Encoding.UTF8.GetString(body));

            var teamData = JsonConvert.DeserializeObject<TeamData>(Encoding.UTF8.GetString(body));

            teamDataService.updateTeam(teamData);
            fireTeamDataRecievedEvent(teamData);
        }

        private void skillTimerDataRecieved(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var skillTimer = JsonConvert.DeserializeObject<SkillTimer>(Encoding.UTF8.GetString(body));

            if (skillTimer.TimerAction == TimerAction.START)
                fireTechTimerStartEvent(skillTimer.TimerAt);

            if (skillTimer.TimerAction == TimerAction.STOP)
                fireTechTimerStopEvent(skillTimer.TimerAt);
        }

        private void skillGateDataRecieved(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var gateInformation = JsonConvert.DeserializeObject<GateInformation>(Encoding.UTF8.GetString(body));

            fireGateInfoRecievedEvent(gateInformation);
        }

        private void safetyCarOvertakeInformationRecieved(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var overtakeInformation = JsonConvert.DeserializeObject<SafetyCarOvertakeInformation>(Encoding.UTF8.GetString(body));
            
            fireSafetyCarOvertakeRecievedEvent(overtakeInformation);
        }

        private void safetyCarFollowInformationRecieved(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var followInformation = JsonConvert.DeserializeObject<SafetyCarFollowInformation>(Encoding.UTF8.GetString(body));

            fireSafetyCarFollowRecievedEvent(followInformation);
        }

        private void speedTimerDataRecieved(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var speedTimer = JsonConvert.DeserializeObject<SpeedTimer>(Encoding.UTF8.GetString(body));

            if (speedTimer.TimerAction == TimerAction.START)
                fireStopperStartEvent(speedTimer.TimerAt);

            if (speedTimer.TimerAction == TimerAction.STOP)
                fireStopperStopEvent(speedTimer.TimerAt);
        }

        private void speedLapDataRecieved(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var speedRaceScore = JsonConvert.DeserializeObject<SpeedRaceScore>(Encoding.UTF8.GetString(body));
            
            fireSpeedRaceScoreRecievedEvent(speedRaceScore);
        }
        
        private void subscribeToQueue(string queueKey, EventHandler<BasicDeliverEventArgs> handleFunction)
        {
            if (IsConnected && contestConnection.IsOpen)
            {
                var consumer = new EventingBasicConsumer(contestConnection);
                consumer.Received += handleFunction;

                contestConnection.BasicConsume(queue: queueKey,
                    autoAck: true,
                    consumer: consumer);
            }
        }
    }
}