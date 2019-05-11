namespace VTV.OpsConsole.RemoteManagement.Models
{
    public class CommandSendResult
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public string ErrorDescription { get; set; }
        public string JobId { get; set; }
        public string JobUrl { get; set; }
    }

}
