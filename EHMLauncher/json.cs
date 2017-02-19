using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EHMLauncher
{
    public class json
    {
        public static void WriteJson(List<fmMain.Server> list)
        {
            string json = null;
            foreach(fmMain.Server srv in list)
            {
                json += JsonConvert.SerializeObject(srv) + "\n";
            }
            File.WriteAllText(fmMain.JSON_FILE, json);
        }        
    }
}
