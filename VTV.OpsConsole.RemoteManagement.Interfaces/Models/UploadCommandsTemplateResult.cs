namespace VTV.OpsConsole.RemoteManagement.Models
{
    public class UploadCommandsTemplateResult
    {
        public bool Success { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public int CommandsCount { get; set; }
        public string ErrorText { get; set; }
        public int ErrorCode { get; set; }
    }
}