using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestApp
{
    class Program
    {
        private static string document1 = @"{
    'operation' : 'RemoteManagment',
    'command' : 'UpdateToSpecificFirmware',
    'firmwareImageFile' : 'demo.img',
    'firmwareImageLocation' : 'http://demo.server.de/src/'}";

        private static string document2 = @"{'volumePercent' : 10.5}";

        private static string document3 = @"{
    'frameRate' : 50,
    'resolution' : '1920x1080'}";
        static void Main(string[] args)
        {
            var service = new VTV.OpsConsole.RemoteManagement.Services.CommandManagementService();
            var resp = service.CreateCommandDocument(
                new VTV.OpsConsole.RemoteManagement.Models.CreateCommandDocumentRequest()
                {
                    Command = "UpdateToSpecificFirmware",
                    Body = document1,
                    TTL = 45,
                    CheckRequired = true,
                    CheckTypes = true
                });
            Console.WriteLine(resp.Success);
            Console.WriteLine(resp.ErrorText);
            Console.WriteLine(resp.Document);
            Console.WriteLine("--------------");
            resp = service.CreateCommandDocument(
                new VTV.OpsConsole.RemoteManagement.Models.CreateCommandDocumentRequest()
                {
                    Command = "SetAudioVolume",
                    Body = document2,
                    TTL = 45,
                    CheckRequired = true,
                    CheckTypes = false
                });
            Console.WriteLine(resp.Success);
            Console.WriteLine(resp.ErrorText);
            Console.WriteLine(resp.Document);
            Console.WriteLine("--------------");
            resp = service.CreateCommandDocument(
                new VTV.OpsConsole.RemoteManagement.Models.CreateCommandDocumentRequest()
                {
                    Command = "SetHDMIResolution",
                    Body = document3,
                    TTL = 45,
                    CheckRequired = true,
                    CheckTypes = true
                });
            Console.WriteLine(resp.Success);
            Console.WriteLine(resp.ErrorText);
            Console.WriteLine(resp.Document);
            Console.WriteLine("--------------");
        }
    }
}
