using System.Net;
using System.Web;

namespace ePortal.Shared
{
    public class CommonMethods
    {
        public static string TextToHtml(string text)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            text = HttpUtility.HtmlEncode(text);
            text = WebUtility.HtmlEncode(text);
            //text = text.Replace("\r\n", "\r");
            //text = text.Replace("\n", "\r");
            //text = text.Replace("\r", "<br>\r\n");
            //text = text.Replace("  ", " &nbsp;");
            ////text = text.Replace("<", "&lt;");
            ////text = text.Replace(">", "&gt;");
            ////text = text.Replace("&", "&amp;");
            ////text = text.Replace("”", "&rdquo;");
            ////text = text.Replace("“", "&ldquo;");
            ////text = text.Replace("–", "&ndash;");
            ////text = text.Replace("‘", "&lsquo;");
            ////text = text.Replace("’", "rsquo;");

            text = text.Replace("\r\n", "\r");
            text = text.Replace("\n", "\r");
            text = text.Replace("\r", "<br>\r\n");
            //text = text.Replace(" ", "&nbsp;");
            //text = text.Replace("  ", " &nbsp;");
            text = text.Replace("<", "&lt;");
            text = text.Replace(">", "&gt;");
            text = text.Replace("&", "&amp;");
            text = text.Replace("”", "&rdquo;");
            text = text.Replace("“", "&ldquo;");
            text = text.Replace("–", "&ndash;");
            text = text.Replace("‘", "&lsquo;");
            text = text.Replace("’", "rsquo;");
            text = text.Replace("'", "&apos;");
            text = text.Replace("\"", "&quot;");
            foreach (char c in text)
            {
                if (c > 127) // special chars
                    sb.Append(String.Format("&#{0};", (int)c));
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }


        public static string HtmlToText(string text)
        {
            text = HttpUtility.HtmlDecode(text);
            text = WebUtility.HtmlDecode(text);
            //text = text.Replace(" &nbsp;", "  ");
            //text = text.Replace("&nbsp;", " ");
            //text = text.Replace("<br>\r\n", "\r");
            //text = text.Replace("\n<br>\n", "<br>");
            //text = text.Replace("\n<br>", "<br>");
            //text = text.Replace("<br>\n", "<br>");
            //text = text.Replace("\n<br/>\n", "<br/>");
            //text = text.Replace("\n<br/>", "<br/>");
            //text = text.Replace("<br/>\n", "<br/>");
            //text = text.Replace("<br>", "\r");
            //text = text.Replace("\r", "\n");
            //text = text.Replace("<br/>", "\n");
            ////text = text.Replace("&lt;", "<");
            ////text = text.Replace("&gt;", ">");
            ////text = text.Replace("&amp;", "&");
            ////text = text.Replace("&rdquo;", "”");
            ////text = text.Replace("&ldquo;", "“");
            ////text = text.Replace("&ndash;", "–");
            ////text = text.Replace("&lsquo;", "‘");
            ////text = text.Replace("rsquo;", "’");

            //text = text.Replace(" &nbsp;", "  ");
            //text = text.Replace("&nbsp;", " ");
            text = text.Replace("<br>\r\n", "\r");
            text = text.Replace("\n<br>\n", "<br>");
            text = text.Replace("\n<br>", "<br>");
            text = text.Replace("<br>\n", "<br>");
            text = text.Replace("\n<br/>\n", "<br/>");
            text = text.Replace("\n<br/>", "<br/>");
            text = text.Replace("<br/>\n", "<br/>");
            text = text.Replace("<br>", "\r");
            text = text.Replace("\r", "\n");
            text = text.Replace("<br/>", "\n");
            text = text.Replace("&lt;", "<");
            text = text.Replace("&gt;", ">");
            text = text.Replace("&amp;", "&");
            text = text.Replace("&rdquo;", "”");
            text = text.Replace("&ldquo;", "“");
            text = text.Replace("&ndash;", "–");
            text = text.Replace("&lsquo;", "‘");
            text = text.Replace("rsquo;", "’");
            text = text.Replace("&apos;", "'");
            text = text.Replace("&#39;", "'");
            text = text.Replace("&quot;", "\"");
            return text;
        }
    }
}
