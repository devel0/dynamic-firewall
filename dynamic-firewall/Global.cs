using System.IO;
using Newtonsoft.Json;

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

        public Global()
        {
            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Config.Pathfilename));
        }

    }

}
