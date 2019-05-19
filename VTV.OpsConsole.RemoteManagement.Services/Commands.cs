namespace VTV.OpsConsole.RemoteManagement.Services
{
    internal static class Commands
    {

        public static string Text = @"
{
  'commands': [
    {
      'name': 'updateFirmware',
      'description': 'Triggers the standard firmware update process of the device',
      'parameters': null
    },
    {
      'name': 'updateToSpecificFirmware',
      'description': 'Allows to force a software upate to specific versions',
      'parameters': [
        {
          'name': 'firmwareImageUrl',
          'required': true,
          'acceptedValues': '',
          'description': 'Name of the firmware image file',
          'parameterType': 8
        },
        {
          'name': 'uiImageUrl',
          'required': false,
          'acceptedValues': '',
          'description': 'Url of the server and directory to download the image from. if not provided, default server is used.',
          'parameterType': 8
        },
        {
          'name': 'loaderImageUrl',
          'required': false,
          'acceptedValues': '',
          'description': 'is the stb forced to replace newer versions=',
          'parameterType': 8
        }
      ]
    },
    {
      'name': 'reboot',
      'description': '',
      'parameters': null
    },
    {
      'name': 'gotoPowerState',
      'description': '',
      'parameters': [
        {
          'name': 'powerState',
          'required': true,
          'acceptedValues': 'on,off',
          'description': 'Status to send the device to. on, off, standby',
          'parameterType': 8
        }
      ]
    },
    {
      'name': 'setAudioVolume',
      'description': '',
      'parameters': [
        {
          'name': 'mute',
          'required': false,
          'acceptedValues': '',
          'description': 'muted or unmuted',
          'parameterType': 9
        },
        {
          'name': 'value',
          'required': false,
          'acceptedValues': '0,5,10,15,20,25,30,35,40,45,50,55,60,65,70,75,80,85,90,95,100',
          'description': 'The audio volume. Range 0-100 in 5 percent steps',
          'parameterType': 6
        }
      ]
    },
    {
      'name': 'runDesasterRecovery',
      'description': '',
      'parameters': null
    },
    {
      'name': 'doFactoryReset',
      'description': '',
      'parameters': null
    },
    {
      'name': 'doPartialReset',
      'description': '',
      'parameters': [
        {
          'name': 'recordings',
          'required': true,
          'acceptedValues': '',
          'description': '',
          'parameterType': 9
        },
        {
          'name': 'drm',
          'required': true,
          'acceptedValues': '',
          'description': '',
          'parameterType': 9
        },
        {
          'name': 'network',
          'required': true,
          'acceptedValues': '',
          'description': '',
          'parameterType': 9
        },
        {
          'name': 'bluetooth',
          'required': true,
          'acceptedValues': '',
          'description': '',
          'parameterType': 9
        },
        {
          'name': 'filesystem',
          'required': false,
          'acceptedValues': '',
          'description': '',
          'parameterType': 9
        }
      ]
    },
    {
      'name': 'runChannelScan',
      'description': '',
      'parameters': null
    },
    {
      'name': 'runRegionDiscovery',
      'description': '',
      'parameters': null
    },
    {
      'name': 'changeChannelVisibility',
      'description': '',
      'parameters': [
        {
          'name': 'unsubscribed',
          'required': true,
          'acceptedValues': '',
          'description': '',
          'parameterType': 9
        },
        {
          'name': 'CPFiltering',
          'required': true,
          'acceptedValues': '',
          'description': '',
          'parameterType': 8
        }
      ]
    },
    {
      'name': 'runSpeedTest',
      'description': '',
      'parameters': null
    },
    {
      'name': 'deleteLocalRecording',
      'description': '',
      'parameters': [
        {
          'name': 'recID',
          'required': true,
          'acceptedValues': '',
          'description': 'Id of the planned recording',
          'parameterType': 8
        }
      ]
    },
    {
      'name': 'deleteLocalPlannedRecording',
      'description': '',
      'parameters': [
        {
          'name': 'recID',
          'required': true,
          'acceptedValues': '',
          'description': 'Id of the planned recording',
          'parameterType': 8
        }
      ]
    },
    {
      'name': 'setStandbyMode',
      'description': '',
      'parameters': [
        {
          'name': 'mode',
          'required': true,
          'acceptedValues': 'active,passive',
          'description': 'active standby or passive standby',
          'parameterType': 8
        }
      ]
    },
    {
      'name': 'setHDMIMode',
      'description': '',
      'parameters': [
        {
          'name': 'mode',
          'required': true,
          'acceptedValues': 'auto,720p,1080i,1080p,4k',
          'description': 'Mode to be set',
          'parameterType': 8
        }
      ]
    },
    {
      'name': 'setHDMIAudioMode',
      'description': '',
      'parameters': [
        {
          'name': 'mode',
          'required': true,
          'acceptedValues': 'auto,pcm,dolby',
          'description': 'Mode to be set',
          'parameterType': 8
        }
      ]
    },
    {
      'name': 'setHearingDisabilityMode',
      'description': '',
      'parameters': [
        {
          'name': 'active',
          'required': true,
          'acceptedValues': '',
          'description': null,
          'parameterType': 9
        }
      ]
    },
    {
      'name': 'setUILanguage',
      'description': '',
      'parameters': [
        {
          'name': 'language',
          'required': true,
          'acceptedValues': '',
          'description': 'language as 5 character countrycode. de-DE',
          'parameterType': 8
        }
      ]
    },
    {
      'name': 'setPereferredSubtitleLanguage',
      'description': '',
      'parameters': [
        {
          'name': 'language',
          'required': true,
          'acceptedValues': '',
          'description': 'language as 5 character countrycode. de-DE',
          'parameterType': 8
        }
      ]
    },
    {
      'name': 'setPereferreAudioLanguage',
      'description': '',
      'parameters': [
        {
          'name': 'language',
          'required': true,
          'acceptedValues': '',
          'description': 'language as 5 character countrycode. de-DE',
          'parameterType': 8
        }
      ]
    },
    {
      'name': 'setSubtitleMode',
      'description': '',
      'parameters': [
        {
          'name': 'mode',
          'required': true,
          'acceptedValues': 'auto,on,off',
          'description': '',
          'parameterType': 8
        }
      ]
    },
    {
      'name': 'setActiveStandbyTimer',
      'description': '',
      'parameters': [
        {
          'name': 'durationMinutes',
          'required': true,
          'acceptedValues': '60,120,180,240,300',
          'description': 'duration of the timer in minutes',
          'parameterType': 6
        }
      ]
    },
    {
      'name': 'setParentalMode',
      'description': '',
      'parameters': [
        {
          'name': 'visibility',
          'required': true,
          'acceptedValues': 'show,hide,pin',
          'description': '',
          'parameterType': 8
        }
      ]
    },
    {
      'name': 'flushBatchEvents',
      'description': '',
      'parameters': null
    }
  ]
}";
    }
}