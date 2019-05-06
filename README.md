# DeviceManagementServer
Sample AWS IoT Device Management WebAPI Server
Using dome basic API's to create nd manage jobs.

### AWS Credentials
As this is only my personal IoT training project, it has no functionalities built in to securely load credentials in a production environment. I'm only making use of the Secrets Manager in VF2019.
The following entries are required for the solution to work.
```avascript
{
  "AWS:ApiAccessKey": "<YOURAPIKEYHERE>",
  "AWS:ApiSecretKey": "<YOURAPISECRETKEYHERE>",
  "AWS:AccountId": "<YOURACCOUNTIDHERE>",
  "AWS:Region": "<YOURAWSREGIONHERE>",
  "AWS:IoTEndpoint": "<YOURAWSIOTENDPOINTHERE>"
}
```