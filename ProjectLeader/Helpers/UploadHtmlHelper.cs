using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace ProjectLeader.Helpers
{


    
    public static class UploadHtmlHelper
    {
        public static string Upload(this HtmlHelper helper, string name)
        {
            string result = string.Format("<input type=\"file\" id=\"{0}\" name=\"{0}\" />", name);
            return result;
        }

        public static string Upload(this HtmlHelper helper, string name, object htmlAttributes)
        {
            string attributes;
            if (htmlAttributes is IEnumerable<KeyValuePair<string, object>>)
                attributes = (htmlAttributes as IEnumerable<KeyValuePair<string, object>>).ToAttributeString();
            else
                attributes = (new RouteValueDictionary(htmlAttributes)).ToAttributeString();

            string result = string.Format("<input type=\"file\" name=\"{0}\" {1} />", name, attributes);
            return result;
        }

        public static string ToAttributeString(this IEnumerable<KeyValuePair<string, object>> instance)
        {
            return string.Empty;
        }

        public static string ToAttributeString(this IDictionary<string, object> instance)
        {
            return string.Empty;
        }
    }
}