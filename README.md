# cloudinteractive-statuspage

cloudinteractive-statuspage is a simple Microsoft ASP.NET Core MVC-based service status and incident communication website.

### Table of contents
* [Features](#features)
* [Dependencies](#dependencies)
* [Configuration](#configuration)

## Features
* Display the status of specific port responsivity.
* Display the status of web servers.
* Display some custom alerts or banners.
* Responsive UI. (Support mobile devices)

## Dependencies
* [Microsoft.Extensions.Configuration.Binder](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Binder/7.0.4?_src=template) - MIT License
* [Microsoft.Extensions.Configuration.Json](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json/7.0.0?_src=template) - MIT License

## Configuration



### config.json
config.json file should be in the root folder.

config.json should be configured as the below example.

```
{
  "ServerConfig": {
    "ObserverTimeWindow": 1000,
    "TimeoutThreshold": 1000,
    "ShowAutoSummary": true,
    "CoreServices": [
      {
        "IP": "10.10.20.10",
        "Port": 135,
        "Name": "AD Domain Service"
      },
      {
        "IP": "10.10.20.10",
        "Port": 53,
        "Name": "Internal DNS Service"
      }
    ],
    "Services": [
      {
        "Name": "A1",
        "SubName": "East US",
        "Url": "a1.foo.net",
        "isMaintenance": false
      },
      {
        "Name": "B2",
        "SubName": "Korea Central",
        "Url": "b2.foo.net",
        "isMaintenance": false
      },
      {
        "Name": "C3",
        "SubName": "Korea South",
        "Url": "c3.foo.net",
        "isMaintenance": false
      }
    ]
  }
}
```
-  **ObserverTimeWindow:** Frequency at which the server checks the status of the services. It serves as the unit used to calculate the service quality index.
-  **TimeoutThreshold:** Threshold at which the server determines service is offline.
-  **ShowAutoSummary:** Show the auto-summary of the service status.
-  **CoreService:** These are essential services that need individual management and are monitored via a TCP port. They do not have a service quality index associated with them.
-  **Services:** Server or region that needs to be managed by one IP or URL. it is monitored by ICMP ping or HTTP response.



### notify.json
notify.json file contains the content of the main page's top banner notifications.   
notify.json file is optional and can be configured as in the below example.  

```
{
  "NotifyConfig": {
    "Notices": [
      {
        "Type": "Info",
        "Content": "Meet our new West-US C3 Region. Check out Management Portal for more information."
      },
      {
        "Type": "Warn",
        "Content": "C1 Region is experiencing a connection issue due to extreme weather conditions."
      }
    ] 
  } 
}
```

-  **Type:** `Warn`, `Info` 
-  **Content:** Content of notification.
