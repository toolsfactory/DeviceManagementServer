namespace VTV.OpsConsole.RemoteManagement.Models
{
    public class CommandParsingResult
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public Command Command { get; set; }
    }
}
