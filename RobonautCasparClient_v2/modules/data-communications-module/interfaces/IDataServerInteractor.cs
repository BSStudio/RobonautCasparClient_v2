using System.Collections.Generic;
using RobonautCasparClient_v2.DO;
using RobonautCasparClient_v2.DO.communication;

namespace RobonautCasparClient_v2.modules.interfaces
{
    public abstract class IDataServerInteractor
    {
        public delegate void connectedDelegate();
        public event connectedDelegate connectedEvent;
        protected void fireConnectedEvent() => connectedEvent();

        public delegate void connectionFailedDelegate();
        public event connectionFailedDelegate connectionFailedEvent;
        protected void fireConnectionFailedEvent() => connectionFailedEvent();

        public delegate void disconnectedDelegate();
        public event disconnectedDelegate disconnectedEvent;
        protected void fireDisconnectedEvent() => disconnectedEvent();

        public delegate void dataProviderClientConnectedDelegate();
        public event dataProviderClientConnectedDelegate dataProviderClientConnectedEvent;
        protected void fireDataProviderClientConnectedEvent() => 
            dataProviderClientConnectedEvent();
        
        public delegate void dataProviderClientDisconnectedDelegate();
        public event dataProviderClientDisconnectedDelegate dataProviderClientDisconnectedEvent;
        protected void fireDataProviderClientDisconnectedEvent() => 
            dataProviderClientDisconnectedEvent();
        

        public delegate void teamDatasRecievedDelegate(List<TeamData> newTeamDatas);
        public event teamDatasRecievedDelegate teamDatasRecievedEvent;
        protected void fireTeamDatasRecievedEvent(List<TeamData> teamDatas) =>
            teamDatasRecievedEvent(teamDatas);
        
        public delegate void teamDataRecievedDelegate(TeamData newTeamData);
        public event teamDataRecievedDelegate teamDataRecievedEvent;
        protected void fireTeamDataRecievedEvent(TeamData teamData) =>
            teamDataRecievedEvent(teamData);

        public delegate void techTimerStartDelegate(int startMs);
        public event techTimerStartDelegate techTimerStartEvent;
        protected void fireTechTimerStartEvent(int startMs) =>
            techTimerStartEvent(startMs);

        public delegate void techTimerStopDelegate(int stopMs);
        public event techTimerStopDelegate techTimerStopEvent;
        protected void fireTechTimerStopEvent(int stopMs) =>
            techTimerStopEvent(stopMs);

        public delegate void stopperStartDelegate(int startMs);
        public event stopperStartDelegate stopperStartEvent;
        protected void fireStopperStartEvent(int startMs) => stopperStartEvent(startMs);
        
        public delegate void stopperStopDelegate(int stopMs);
        public event stopperStopDelegate stopperStopEvent;
        protected void fireStopperStopEvent(int stopMs) => stopperStopEvent(stopMs);

        public delegate void gateInfoRecievedDelegate(GateInformation gateInformation);
        public event gateInfoRecievedDelegate gateInfoRecievedEvent;
        protected void fireGateInfoRecievedEvent(GateInformation gateInformation) =>
            gateInfoRecievedEvent(gateInformation);

        public delegate void safetyCarOvertakeRecievedDelegate(SafetyCarOvertakeInformation overtakeInformation);
        public event safetyCarOvertakeRecievedDelegate safetyCarOvertakeRecievedEvent;
        protected void fireSafetyCarOvertakeRecievedEvent(SafetyCarOvertakeInformation overtakeInformation) =>
            safetyCarOvertakeRecievedEvent(overtakeInformation);

        public delegate void safetyCarFollowRecievedDelegate(SafetyCarFollowInformation followInformation);
        public event safetyCarFollowRecievedDelegate safetyCarFollowRecievedEvent;
        protected void fireSafetyCarFollowRecievedEvent(SafetyCarFollowInformation followInformation) =>
            safetyCarFollowRecievedEvent(followInformation);

        public delegate void speedRaceScoreRecievedDelegate(SpeedRaceScore speedRaceScore);
        public event speedRaceScoreRecievedDelegate speedRaceScoreRecievedEvent;
        protected void fireSpeedRaceScoreRecievedEvent(SpeedRaceScore speedRaceScore) =>
            speedRaceScoreRecievedEvent(speedRaceScore);

        public abstract void setYear(int year);

        public abstract void connect(string serverUrl);

        public abstract void disconnect();

        public abstract void requestTeamDataUpdate();
    }
}