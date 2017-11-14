using System;
using System.Text;
using NLog;
using NLog.LayoutRenderers;
using NLog.Web.LayoutRenderers;
using System.Linq;

namespace PTV.Framework.Logging
{
    [LayoutRenderer("aspnet-request-ip")]
    public class AspNetRequestIpLayoutRenderer : AspNetLayoutRendererBase
    {
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            string ip = GetHeaderValueAs<string>("X-Forwarded-For").SplitCsv().FirstOrDefault();
            if (ip.IsNullOrWhitespace() && HttpContextAccessor.HttpContext?.Connection?.RemoteIpAddress != null)
                ip = HttpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            if (ip.IsNullOrWhitespace())
                ip = GetHeaderValueAs<string>("REMOTE_ADDR");

            ip = (ip ?? string.Empty).Trim();
            builder.Append(ip);
        }

        private T GetHeaderValueAs<T>(string headerName)
        {
            if (HttpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue(headerName, out var values) ?? false)
            {
                string rawValues = values.ToString();

                if (!rawValues.IsNullOrEmpty())
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default(T);
        }
    }
}
