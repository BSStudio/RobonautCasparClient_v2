using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RobonautCasparClient_v2.DO;
using RobonautCasparClient_v2.DO.communication;
using WebSocketSharp;

namespace RobonautCasparClient_v2.modules
{
    public class WebSocketDataInteractor
    {
        #region Singleton implementation

        private static WebSocketDataInteractor instance;

        public static WebSocketDataInteractor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WebSocketDataInteractor();
                }

                return instance;
            }
        }

        #endregion

        private WebSocket ws;
        private TeamDataService teamDataService = TeamDataService.Instance;

        public int Year { get; set; }

        public delegate void connectedDelegate();

        public event connectedDelegate connectedEvent;

        public delegate void connectionFailedDelegate();

        public event connectionFailedDelegate connectionFailedEvent;

        public delegate void disconnectedDelegate();

        public event disconnectedDelegate disconnectedEvent;

        public delegate void dataProviderClientConnectedDelegate();

        public event dataProviderClientConnectedDelegate dataProviderClientConnectedEvent;

        public delegate void dataProviderClientDisconnectedDelegate();

        public event dataProviderClientDisconnectedDelegate dataProviderClientDisconnectedEvent;


        public delegate void teamDatasRecievedDelegate(List<TeamData> newTeamDatas);

        public event teamDatasRecievedDelegate teamDatasRecievedEvent;

        public delegate void teamDataRecievedDelegate(TeamData newTeamData);

        public event teamDataRecievedDelegate teamDataRecievedEvent;

        public delegate void techTimerStartDelegate(int startMs);

        public event techTimerStartDelegate techTimerStartEvent;

        public delegate void techTimerStopDelegate(int stopTime);

        public event techTimerStopDelegate techTimerStopEvent;

        public delegate void stopperStartDelegate();

        public event stopperStartDelegate stopperStartEvent;

        public delegate void stopperStopDelegate(int stopTime);

        public event stopperStopDelegate stopperStopEvent;

        public delegate void technicalScoreRecievedDelegate(TechnicalScoreDto techScore);

        public event technicalScoreRecievedDelegate technicalScoreRecievedEvent;

        public delegate void speedScoreRecievedDelegate(SpeedScoreDto speedScore);

        public event speedScoreRecievedDelegate speedScoreRecievedEvent;

        public delegate void teamResultRecievedDelegate(TeamResultDto teamResult);

        public event teamResultRecievedDelegate teamResultRecievedEvent;


        public void connect(string serverUrl)
        {
            try
            {
                ws = new WebSocket(serverUrl);
                ws.OnMessage += Ws_OnMessage;
                ws.OnClose += Ws_OnClose;
                ws.Connect();
                requestTeamDataUpdate();
            }
            catch (Exception e)
            {
                connectionFailedEvent();
            }
        }

        public void disconnect()
        {
            ws.Close();
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            JObject obj = JObject.Parse(e.Data);
            Console.WriteLine(obj);

            switch (obj.GetValue("type").ToString())
            {
                case "connected":
                    connectedEvent();
                    break;
                case "teams":
                    List<TeamData> teamDatas =
                        JsonConvert.DeserializeObject<List<TeamData>>(obj.GetValue("teams").ToString());
                    teamDataService.Teams = teamDatas;
                    teamDatasRecievedEvent(teamDatas);
                    break;
                case "teamData":
                    TeamData teamData = JsonConvert.DeserializeObject<TeamData>(obj.GetValue("team").ToString());
                    teamDataService.updateTeam(teamData);
                    teamDataRecievedEvent(teamData);
                    break;
                case "dataClientConnected":
                    dataProviderClientConnectedEvent();
                    break;
                case "technicalTimerStart":
                    techTimerStartEvent(Int32.Parse(obj.GetValue("startTime").ToString()));
                    break;
                case "technicalTimerStop":
                    techTimerStopEvent(Int32.Parse(obj.GetValue("stopTime").ToString()));
                    break;
                case "speedTimerStart":
                    stopperStartEvent();
                    break;
                case "speedTimerStop":
                    stopperStopEvent(Int32.Parse(obj.GetValue("stopTime").ToString()));
                    break;
                case "technicalScoreUpdate":
                    TechnicalScoreDto techScore =
                        JsonConvert.DeserializeObject<TechnicalScoreDto>(e.Data);
                    technicalScoreRecievedEvent(techScore);
                    break;
                case "speedScoreUpdate":
                    SpeedScoreDto speedScore =
                        JsonConvert.DeserializeObject<SpeedScoreDto>(e.Data);
                    speedScoreRecievedEvent(speedScore);
                    break;
                case "teamResult":
                    TeamResultDto teamResult =
                        JsonConvert.DeserializeObject<TeamResultDto>(e.Data);
                    teamResultRecievedEvent(teamResult);
                    break;
            }
        }

        public void requestTeamDataUpdate()
        {
            ws.Send("{\"type\": \"getTeams\",\"year\": " + Year + "}");
        }

        private void Ws_OnClose(object sender, CloseEventArgs e)
        {
            disconnectedEvent();
        }
    }
}