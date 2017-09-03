using System;

namespace VBot
{
    public class ConvertHelper
    {
        public static string ToString(string str)
        {
            try
            {
                return Convert.ToString(str);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
