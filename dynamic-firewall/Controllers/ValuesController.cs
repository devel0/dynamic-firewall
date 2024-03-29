﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace dynamic_firewall.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        Global global { get { return Global.Instance; } }

        Config config { get { return global.Config; } }

        static Dictionary<string, Thread> tokenThreads = new Dictionary<string, Thread>();

        [HttpGet("{token}")]
        public ContentResult enable(string token)
        {
            try
            {
                var q = HttpContext.Request.Headers["X-Real-IP"];
                var url = "";
                if (q.Count > 0) url = q.First();
                
                var qt = config.ValidTokens.FirstOrDefault(r => r.Token == token);
                if (qt == null) return Content($"<h1>invalid access</h1>ip:{url}", "text/html");                
                
                System.Diagnostics.Process.Start("/sbin/ipset", $"add {qt.IPSetName} {url}");
                System.Console.WriteLine($"added [{url}] to ipset [{qt.IPSetName}]");

                global.RestartIpset(qt, url);   

                return Content($"<html><h1>dynamic firewall</h1>" +
                    $"from url=[{url}]<br/>" +
                    ((qt.ExpireMinutes == 0) ?
                    $"{qt.IPSetName} access never expire<br/></html>" :
                    $"{qt.IPSetName} access expire in {qt.ExpireMinutes} minutes<br/></html>"), "text/html");

            }
            catch (Exception ex)
            {
                return Content($"error", "text/html");
            }
        }

        [HttpGet("{token}")]
        public ContentResult disable(string token)
        {
            try
            {
                var qt = config.ValidTokens.FirstOrDefault(r => r.Token == token);
                if (qt == null) return Content($"<h1>invalid access</h1>", "text/html");

                var q = HttpContext.Request.Headers["X-Real-IP"];
                var url = "";
                if (q.Count > 0) url = q.First();

                System.Diagnostics.Process.Start("/sbin/ipset", $"del {qt.IPSetName} {url}");
                System.Console.WriteLine($"removed [{url}] from ipset [{qt.IPSetName}]");

                return Content($"<html><h1>dynamic firewall</h1>" +
                    $"from url=[{url}]<br/>" +
                    $"{qt.IPSetName} access disabled", "text/html");

            }
            catch (Exception ex)
            {
                return Content($"error", "text/html");
            }
        }
    }
}
