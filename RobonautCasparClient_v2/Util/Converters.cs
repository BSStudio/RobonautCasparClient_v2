using System;

namespace RobonautCasparClient_v2.DO
{
    public class Converters
    {
        public static string TimeToString(int speedTime)
        {
            if (speedTime == int.MaxValue)
                return "-";

            int min = (int) Math.Floor((double) (speedTime / 60000));
            int sec = (int) Math.Floor((double) (speedTime - min * 60000) / 1000);
            int ms = (int) Math.Floor((double) speedTime - 1000 * sec - 60000 * min);

            return min.ToString("00") + ":" + sec.ToString("00") + "." + ms.ToString("00");
        }

        public static string DirectionToString(TimerDirection direction)
        {
            switch (direction)
            {
                case TimerDirection.UP:
                    return "up";
                case TimerDirection.DOWN:
                    return "down";
                case TimerDirection.STOP:
                    return "stop";
                default:
                    return "";
            }
        }
    }
}