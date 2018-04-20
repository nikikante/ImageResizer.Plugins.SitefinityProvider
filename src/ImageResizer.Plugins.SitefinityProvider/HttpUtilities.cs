using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ImageResizer.Plugins.SitefinityProvider
{
    static class HttpUtilities
    {
        #region helper methods
        public static string QueryStringSerialize(NameValueCollection nvm)
        {
            if (nvm == null)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            foreach (string key in nvm.Keys)
            {
                sb.Append(HttpUtility.UrlEncode(key));
                sb.Append("=");
                var values = nvm.GetValues(key);
                if (values.Length == 1 && values[0] != null)
                {
                    sb.Append(HttpUtility.UrlEncode(values[0]));
                }
                else
                {
                    sb.Append(HttpUtility.UrlEncode(string.Join(",", values)));
                }
            }
            return sb.ToString();
        }
        #endregion
    }
}
