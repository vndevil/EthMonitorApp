namespace EthMonitorApp
{
    public class StringHelper
    {
        private static readonly string[] VietnameseSigns =
        {
            "aAeEoOuUiIdDyYzZlsSc",
            "áàạảãâấầậẩẫăắằặẳẵą",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "òóọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ",
            "ż",
            "Ż",
            "ł",
            "ś",
            "Ś",
            "ć"
        };

        public static string RemoveSign4VietnameseString(string str, bool hasSpace)
        {
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    for (var i = 1; i < VietnameseSigns.Length; i++)
                    {
                        for (var j = 0; j < VietnameseSigns[i].Length; j++)
                            str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
                    }

                    if (hasSpace)
                    {
                        str = str.Replace(" ", "-");
                        str = str.Replace("&nbsp;", "-");
                    }

                    str = str.Replace("ngòi", "ngoi");
                    str = str.Replace("ò", "o");
                    str = str.Replace("‘", string.Empty);
                    str = str.Replace("’", string.Empty);
                    str = str.Replace("tròng", "trong");
                    str = str.Replace("&", "-");
                    str = str.Replace("&amp;", "-");
                    str = str.Replace("'", string.Empty);
                    str = str.Replace("\"", "-");
                    str = str.Replace("/", "-");
                    str = str.Replace(":", "-");
                    str = str.Replace("“", string.Empty);
                    str = str.Replace("”", string.Empty);
                    str = str.Replace("|", "-");
                    str = str.Replace(".", string.Empty);
                    str = str.Replace("(", string.Empty);
                    str = str.Replace("\"", string.Empty);
                    str = str.Replace("''", string.Empty);
                    str = str.Replace("``", string.Empty);
                    str = str.Replace(")", string.Empty);
                    str = str.Replace("-,", string.Empty);
                    str = str.Replace(",", string.Empty);
                    str = str.Replace(">", string.Empty);
                    str = str.Replace("&quot;", string.Empty);
                    str = str.Replace("&ldquo;", string.Empty);
                    str = str.Replace("&rdquo;", string.Empty);
                    str = str.Replace("&lsquo;", string.Empty);
                    str = str.Replace("&rsquo;", string.Empty);
                    str = str.Replace("&nbsp;", string.Empty);
                    str = str.Replace("&gt;", string.Empty);
                    str = str.Replace("<", string.Empty);
                    str = str.Replace("&lt;", string.Empty);
                    str = str.Replace("-?", string.Empty);
                    str = str.Replace("?", string.Empty);
                    str = str.Replace("-!", string.Empty);
                    str = str.Replace("-@", string.Empty);
                    str = str.Replace("@", string.Empty);
                    str = str.Replace("!", string.Empty);
                    str = str.Replace("%", string.Empty);
                    str = str.Replace("#", string.Empty);
                    str = str.Replace("^", string.Empty);
                    str = str.Replace("*", string.Empty);
                    str = str.Replace("+", string.Empty);
                    str = str.Replace("&mdash;", string.Empty);
                    str = str.Replace("™", string.Empty);
                    str = str.Replace("&trade;", string.Empty);
                    str = str.Replace("---", "-");
                    str = str.Replace("--", "-");
                    str = str.Replace("®", string.Empty);
                    str = str.Replace("™", string.Empty);
                    str = str.Replace("°C", string.Empty);
                    str = str.Replace("° C", string.Empty);
                    str = str.Replace("°", string.Empty);
                    str = str.Replace("ę", "e");
                    str = str.Replace("ż", "z");
                    str = str.Replace("ó", "o");
                    str = str.Replace("ł", "l");
                    str = str.Replace("ń", "n");
                    str = str.Replace("ü", "u");
                    str = str.Replace("ä", "a");
                    str = str.Replace("ö", "o");
                    str = str.Replace("ß", "b");
                    str = str.Replace(("ß").ToLower(), "b");
                    str = str.Replace(("lát"), "lat");
                    str = str.Replace(("gạch"), "gach");
                    str = str.Replace(("óp"), "op");

                    if (str.EndsWith("-")) str = str.Remove(str.Length - 1, 1);

                    return str.ToLower();
                }

                return string.Empty;
            }
            catch
            {
                return str;
            }
        }

        /// <summary>
        /// Thay thế, lọc dấu
        /// </summary>
        /// <param name="str"></param>
        /// <returns> chuỗi được thay thế </returns>
        public static string RemoveSign4VietnameseString(string str)
        {
            return RemoveSign4VietnameseString(str, true);
        }
    }
}
