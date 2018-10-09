using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSharpUnilever.Controllers.Util
{
    public class Util
    {
        /// <summary>
        /// Injects a piece of html content inside a standard bsharp header and layout to be sent by email
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        public static string TranscludeInBSharpEmailTemplate(string htmlContent)
        {
            return
$@"<div style='font-family: BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif, ""Apple Color Emoji"", ""Segoe UI Emoji"", ""Segoe UI Symbol"", ""Noto Color Emoji""'>
    <center>
        <div style=""background: #343a40;padding: 15px"">
            <span style=""color: white; font-size:22px"">BSharp<strong style=""color:#17a2b8"">ERP</strong></span>
        </div>
    </center>
    <div>
        <center>
            {htmlContent}
        </center>
    </div>
</div>";
        }
    }
}
