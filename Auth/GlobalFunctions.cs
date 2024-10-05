using Newtonsoft.Json.Linq;

namespace RuokalistaServer.Auth
{
    public class GlobalFunctions
    {
        private static string root;
        public static string GetRoot()
        {
            if (root == null)
            {
                var config = File.ReadAllText("appsettings.json");
                //parse config json and get root
                var parsedConfig = JObject.Parse(config);
                root = parsedConfig?["rootUser"]?.ToString() ?? "";
            }
            return root;



        }
    }
}
