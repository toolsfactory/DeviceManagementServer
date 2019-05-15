using Newtonsoft.Json.Linq;
using NUnit.Framework;
using VTV.OpsConsole.RemoteManagement.Services;

namespace VTV.OpsConsole.RemoteManagement.Tests
{
    public class CommandManagementServiceTests
    {
        private readonly CommandManagementService _service = new CommandManagementService();
        private readonly string _attributeMissingText = "Required attribute missing '{0}'.";
        private readonly string _attributeWrongTypeText = "Invalid type for attribute '{0}'. '{1}' instead of '{2}'";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PositiveSimpleCase()
        {
            var document = @"{ 'parameters': {
    'firmwareImageFile' : 'demo.img',
    'firmwareImageLocation' : 'http://demo.server.de/src/'}}";

            var resp = _service.CreateCommandDocument(
                new VTV.OpsConsole.RemoteManagement.Models.CreateCommandDocumentRequest()
                {
                    Command = "UpdateToSpecificFirmware",
                    Body = JObject.Parse(document),
                    TTL = 45,
                    CheckRequired = true,
                    CheckTypes = true
                });
            Assert.IsTrue(resp.Success);
        }
    }
}