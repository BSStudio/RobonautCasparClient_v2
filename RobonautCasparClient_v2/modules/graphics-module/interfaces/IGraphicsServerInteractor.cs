using System.Collections.Generic;
using RobonautCasparClient_v2.DO;

namespace RobonautCasparClient_v2.Modules.interfaces
{
    public abstract class IGraphicsServerInteractor
    {
        public delegate void casparConnectedDelegate();
        public event casparConnectedDelegate casparConnected;

        public delegate void casparDisconnectedDelegate();
        public event casparDisconnectedDelegate casparDisconnected;

        protected void fireCasparConnected()
        {
         casparConnected();
        }

        protected void fireCasparDisconnected()
        {
         casparDisconnected();
        }

        /*
         * Csatlakozik a grafikus kijátszó szerverhez a megadott elérhetőség alapján
        */
        public abstract void connect(string connectionUrl);

        /*
         * Lecsatlakozik a szerverről
         */
        public abstract void disconnect();

        public abstract bool getConnectionToServer();

        /*
         * Megjelenít egy névinzertet
         */
        public abstract void showNameInsert(string name, string title);

        /*
         * Elrejti a névinzertet
         */
        public abstract void hideNameInsert();

        /*
         * Megjeleníti egy csapat és a tagjainak nevét
         */
        public abstract void showTeamNameWithMembers(TeamData teamData);

        /*
         * Elrejt minden csapatokkal kapcsolatos inzertet
         */
        public abstract void stopTeamDataGraphics();

        /*
         * Elrejt minden grafikát
         */
        public abstract void stopAllGraphics();

        /*
         * Ügyességi versenyrész alatt látható inzert
         * Elemei:
         *     - csapat neve
         *     - szerzett pontok
         */
        public abstract void showTeamTechnicalContestDisplay(TeamData teamData);

        public abstract void updateTeamTechnicalContestDisplay(TeamData teamData);

        /*
         * Gyorsasági versenyrész alatt látható inzert
         * Elemei:
         *     - ügyességi részen szerzett pont
         *     - köridők
         */
        public abstract void showTeamSpeedContestDisplay(TeamData teamData);

        public abstract void updateTeamSpeedContestDisplay(TeamData teamData);

        /*
         * Safety carral való interakciók (követés, előzés) kijezéséhez való inzert megjelenítése
         */
        public abstract void showSafetyCarInfoDisplay(TeamData teamData);
        
        public abstract void updateSafetyCarInfoDisplay(TeamData teamData);

        public abstract void hideSafetyCarInfoDisplay();

         /*
         * A meghatározott irányba számláló inzert megjelenítése
         * Ha már meg van jelenítve az inzert, akkor csak frissíti a tartalmat
         */
         public abstract void showTimer(int startMs, TimerDirection dir);

        /*
         * Elrejti a számlálót
         */
        public abstract void hideTimer();

        /*
         * Megjelenít egy csapathoz tartozó minden információt felvonultató inzertet
         */
        public abstract void showTeamAllStats(TeamData teamData, TeamType rankType);

        /*
         * Teljes képernyős tabellák megjelenítése megadott típussal, megadott csapat adatokkal
         *
         * Ha egy oldalra nem férnek ki a csapatok egy oldalra, akkor magától többre bontja és lépteti
         */
        public abstract void showFullscreenGraphics(FullScreenTableType type, List<TeamData> teamDatas);
         
        /*
         * visszater azzal, hogy meg van-e jelenitve a grafika a tablazat utan
         * true = meg van jelenitve
         * false = nem volt mar uj adat, eltunt
         */
        public abstract bool stepFullScreenGraphics(List<TeamData> teamDatas);
        
        /*
         * elrejti a teljes képernyős grafikát
         */
        public abstract void hideFullscreenGraphics();
    }
}