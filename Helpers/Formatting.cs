namespace Bot.Helpers;

public static class Formatting
{
    public static string ToYesNo(this bool b)
    {
        return (b ? "Yes" : "No");
    }

    public static TimeSpan? ParseTime(string strTime)
    {
        TimeSpan? time = null;

        var numStr = strTime.Substring(0, strTime.Length - 1);
        if (int.TryParse(numStr, out var num))
        {
            time = new TimeSpan();
            
            int seconds = 0;
            int minutes = 0;
            int hours = 0;
            int days = 0;
            
            var lastChar = strTime[strTime.Length - 1];
            switch (lastChar)
            {
                case 's':
                {
                    seconds = num;
                    break;
                }
                case 'm':
                {                    
                    minutes = num;
                    break;
                }
                case 'h':
                {
                    hours = num;
                    break;
                }
                case 'd':
                {
                    days = num;
                    break;
                }
                case 'w':
                {
                    days = num * 7;
                    break;
                }
                default:
                {
                    time = null;
                    break;
                }
            }            
        }

        return time;
    }
}