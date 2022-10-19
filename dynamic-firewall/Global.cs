using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace dynamic_firewall
{

    public class Global
    {

        static Global _Instance;
        public static Global Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Global();

                return _Instance;
            }
        }

        public Config Config { get; private set; }

        internal CancellationTokenSource ctsExpiredTokenManager = new CancellationTokenSource();
        internal AutoResetEvent are = new AutoResetEvent(false);

        List<(ValidToken validToken, string ipaddress, DateTime dtStart)> tokenStarted =
            new List<(ValidToken validToken, string ipaddress, DateTime dtStart)>();
        object lckTokenStarted = new object();

        public void RestartIpset(ValidToken validToken, string ipaddress)
        {
            if (validToken.ExpireMinutes == 0) return;

            lock (lckTokenStarted)
            {
                var q = tokenStarted.Where(w => w.validToken.IPSetName == validToken.IPSetName && w.ipaddress == ipaddress).ToList();
                if (q.Count > 0)
                {
                    foreach (var x in q) tokenStarted.Remove(x);
                }
                tokenStarted.Add((validToken, ipaddress, DateTime.Now));
            }
        }

        public Global()
        {
            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Config.Pathfilename));

            Task.Run(async () =>
            {        
                System.Console.WriteLine($"Expired token manager started");        
                while (true)
                {
                    try
                    {
                        if (ctsExpiredTokenManager.IsCancellationRequested) 
                        {
                            System.Console.WriteLine($"stop dynfw requested");
                            break;
                        }
                        var dtnow = DateTime.Now;
                        lock (lckTokenStarted)
                        {
                            var q = tokenStarted.Where(r => (dtnow - r.dtStart).TotalMinutes >= r.validToken.ExpireMinutes).ToList();
                            foreach (var x in q)
                            {
                                System.Console.WriteLine($"removing ipaddr [{x.ipaddress}] from set [{x.validToken.IPSetName}]");
                                System.Diagnostics.Process.Start("/sbin/ipset", $"del {x.validToken.IPSetName} {x.ipaddress}");
                                tokenStarted.Remove(x);
                            }
                        }
                        await Task.Delay(5000, ctsExpiredTokenManager.Token);
                    } 
                    catch (TaskCanceledException)
                    {                    
                        // cancel during Task.Delay happened    
                    }

                }

                are.Set(); // notify ApplicationStopping to continue shutdown
            }, ctsExpiredTokenManager.Token);            
        }

    }

}
