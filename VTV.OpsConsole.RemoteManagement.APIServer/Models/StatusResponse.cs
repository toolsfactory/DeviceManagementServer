namespace VTV.OpsConsole.RemoteManagement.APIServer.Controllers
{
    public class StatusResponse
    {
        public string Environment { get; set; }
        public string APIAccessKeyInfo { get; set; }
        public string ServerBaseUrl { get; set; }
        public string CommandsTemplateSource { get; set; }
        public string CommandsTemplateAuthor { get; set; }
        public string CommandsTemplateVersion { get; set; }
        public bool CommandTemplatesParsed { get; set; }
        public bool CommandTemplatesLoaded { get; set; }
        public string CommandTemplatesParseAndLoadError { get; set; }
    }
}