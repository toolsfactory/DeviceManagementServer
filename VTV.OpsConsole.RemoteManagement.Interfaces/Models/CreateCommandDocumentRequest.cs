using Newtonsoft.Json.Linq;

namespace VTV.OpsConsole.RemoteManagement.Models
{
    public class CreateCommandDocumentRequest
    {
        public string Command { get; set; }
        public JObject Body { get; set; }
        public int TTL { get; set; }
        public bool CheckTypes { get; set; }
        public bool CheckRequired { get; set; }
        public bool CheckValues { get; set; }
    }
}