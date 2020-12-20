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

        public delegate void stopperStartDelegate();
        public event stopperStartDelegate stopperStartEvent;
        protected void fireStopperStartEvent() => stopperStartEvent();
        
        public delegate void stopperStopDelegate(int stopMs);
        public event stopperStopDelegate stopperStopEvent;
        protected void fireStopperStopEvent(int stopMs) => stopperStopEvent(stopMs);

        public delegate void technicalScoreRecievedDelegate(TechnicalScoreDto techScore);
        public event technicalScoreRecievedDelegate technicalScoreRecievedEvent;
        protected void fireTechnicalScoreRecievedEvent(TechnicalScoreDto techScore) =>
            technicalScoreRecievedEvent(techScore);

        public delegate void safetyCarEventRecievedDelegate(SafetyCarEventDto safetyCarEvent);
        public event safetyCarEventRecievedDelegate safetyCarEventRecievedEvent;
        protected void fireSafetyCarUpdateRecievedEvent(SafetyCarEventDto safetyCarEvent) =>
            safetyCarEventRecievedEvent(safetyCarEvent);

        public delegate void teamResultRecievedDelegate(TeamResultDto teamResult);
        public event teamResultRecievedDelegate teamResultRecievedEvent;
        protected void fireTeamResultRecievedEvent(TeamResultDto teamResult) =>
            teamResultRecievedEvent(teamResult);

        public abstract void setYear(int year);

        public abstract void connect(string serverUrl);

        public abstract void disconnect();

        public abstract void requestTeamDataUpdate();
    }
}