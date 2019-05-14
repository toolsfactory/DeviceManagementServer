using Newtonsoft.Json.Linq;

namespace VTV.OpsConsole.RemoteManagement.Models
{
    public class CreateCommandDocumentResponse
    {
        public bool Success { get; set; }
        public JObject Document { get; set; }
        public string ErrorText { get; set; }
    }
}