using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RobonautCasparClient_v2.DO;
using RobonautCasparClient_v2.DO.communication;
using RobonautCasparClient_v2.modules.interfaces;
using WebSocketSharp;

namespace RobonautCasparClient_v2.modules
{
    public class WebSocketDataInteractor: IDataServerInteractor
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

        public override void setYear(int year)
        {
            Year = year;
        }

        public override void connect(string serverUrl)
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
                fireConnectionFailedEvent();
            }
        }

        public override void disconnect()
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
                    fireConnectedEvent();
                    break;
                case "teams":
                    List<TeamData> teamDatas =
                        JsonConvert.DeserializeObject<List<TeamData>>(obj.GetValue("teams").ToString());
                    teamDataService.Teams = teamDatas;
                    fireTeamDatasRecievedEvent(teamDatas);
                    break;
                case "teamData":
                    TeamData teamData = JsonConvert.DeserializeObject<TeamData>(obj.GetValue("team").ToString());
                    teamDataService.updateTeam(teamData);
                    fireTeamDataRecievedEvent(teamData);
                    break;
                case "dataClientConnected":
                    fireDataProviderClientConnectedEvent();
                    break;
                case "technicalTimerStart":
                    fireTechTimerStartEvent(Int32.Parse(obj.GetValue("startTime").ToString()));
                    break;
                case "technicalTimerStop":
                    fireTechTimerStopEvent(Int32.Parse(obj.GetValue("stopTime").ToString()));
                    break;
                case "speedTimerStart":
                    fireStopperStartEvent();
                    break;
                case "speedTimerStop":
                    fireStopperStopEvent(Int32.Parse(obj.GetValue("stopTime").ToString()));
                    break;
                case "technicalScoreUpdate":
                    TechnicalScoreDto techScore =
                        JsonConvert.DeserializeObject<TechnicalScoreDto>(e.Data);
                    fireTechnicalScoreRecievedEvent(techScore);
                    break;
                case "teamResult":
                    TeamResultDto teamResult =
                        JsonConvert.DeserializeObject<TeamResultDto>(e.Data);
                    fireTeamResultRecievedEvent(teamResult);
                    break;
            }
        }

        public override void requestTeamDataUpdate()
        {
            ws.Send("{\"type\": \"getTeams\",\"year\": " + Year + "}");
        }

        private void Ws_OnClose(object sender, CloseEventArgs e)
        {
            fireDisconnectedEvent();
        }
    }
}