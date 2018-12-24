using System.Collections.Generic;

namespace dynamic_firewall
{

    public class ValidToken
    {
        public string Token { get; set; }

        /// <summary>
        /// iptables ipset name
        /// </summary>        
        public string IPSetName { get; set; }

        /// <summary>
        /// 0 never expire
        /// </summary>    
        public double ExpireMinutes { get; set; }
    }

    public class Config
    {
        public static string Pathfilename
        {
            get
            {
                return "/security/dynamic-firewall.json"; 
            }
        }
        public List<ValidToken> ValidTokens { get; set; } = new List<ValidToken>();
    }

}