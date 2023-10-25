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
config.json file should be in the root folder. server will fail to initialize if the config.json file doesn't exist.
config.json should be configured as the below example.

```
{
  "ServerConfig": {
    "PollingRate": 50000,
    "CoreServices": [
      {
        "IP": "10.10.208.254",
        "Port": 135,
        "Name": "Active Directory Domain Service"
      },
      {
        "IP": "10.10.208.254",
        "Port": 67,
        "Name": "DHCP and PXE Service"
      },
      {
        "IP": "10.10.208.251",
        "Port": 53,
        "Name": "DNS Service"
      }
    ],
    "Services": [
      {
        "Name": "C1",
        "SubName": "Korea Central",
        "Url": "c1.foo.com",
        "isMaintenance": false 
      },
      {
        "Name": "C2",
        "SubName": "East US",
        "Url": "c2.foo.com",
        "isMaintenance": false 
      },
      {
        "Name": "C3",
        "SubName": "West US",
        "Url": "c3.foo.com",
        "isMaintenance": true 
      }
    ] 
  } 
}
```
**PollingRate** - how often the server gets the services' status and is a unit of calculating the service quality index (SLA Percent).  
**CoreService** - Services that need to be managed individually. it is monitored by a TCP port. (not calculating SLA Percent.)  
**Services** - Server or region that needs to be managed by one IP or URL. it is monitored by ICMP ping or HTTP response.  



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

**Type** - type of the notification: warn, info  
**Content** - notification content.
