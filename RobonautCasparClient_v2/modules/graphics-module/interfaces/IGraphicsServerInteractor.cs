using System.Collections.Generic;
using RobonautCasparClient_v2.DO;

namespace RobonautCasparClient_v2.Modules.interfaces
{
    public interface IGraphicsServerInteractor
    {

        /*
         * Csatlakozik a grafikus kijátszó szerverhez a megadott elérhetőség alapján
        */
        void connect(string connectionUrl);

        /*
         * Lecsatlakozik a szerverről
         */
        void disconnect();

        /*
         * Megjelenít egy névinzertet
         */
        void showNameInsert(string name, string title);

        /*
         * Elrejti a névinzertet
         */
        void hideNameInsert();

        /*
         * Megjeleníti egy csapat és a tagjainak nevét
         */
        void showTeamNameWithMembers(TeamData teamData);

        /*
         * Elrejt minden csapatokkal kapcsolatos inzertet
         */
        void stopTeamDataGraphics();

        /*
         * Elrejt minden grafikát
         */
        void stopAllGraphics();

        /*
         * Ügyességi versenyrész alatt látható inzert
         * Elemei:
         *     - csapat neve
         *     - szerzett pontok
         */
        void showTeamTechnicalContestDisplay(TeamData teamData);

        void updateTeamTechnicalContestDisplay(TeamData teamData);

        /*
         * Gyorsasági versenyrész alatt látható inzert
         * Elemei:
         *     - ügyességi részen szerzett pont
         *     - köridők
         */
        void showTeamSpeedContestDisplay(TeamData teamData);

        void updateTeamSpeedContestDisplay(TeamData teamData);

         /*
         * A meghatározott irányba számláló inzert megjelenítése
         * Ha már meg van jelenítve az inzert, akkor csak frissíti a tartalmat
         */
        void showTimer(int startMs, TimerDirection dir);

        /*
         * Elrejti a számlálót
         */
        void hideTimer();

        /*
         * Teljes képernyős tabellák megjelenítése megadott típussal, megadott csapat adatokkal
         *
         * Ha egy oldalra nem férnek ki a csapatok egy oldalra, akkor magától többre bontja és lépteti
         */
        void showFullscreenGraphics(FullScreenTableType type, List<TeamData> teamDatas);

        void hideFullscreenGraphics();

        /*
         * Megjelenít egy csapathoz tartozó minden információt felvonultató inzertet
         */
        void showTeamAllStats(TeamData teamData, TeamType rankType);
    }
}