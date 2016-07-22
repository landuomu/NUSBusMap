# NUSBusMap

### Introduction
An iOS mobile application for NUS bus information
Developed using Xamarin (C#), a cross-platform mobile application development software.

### Features
* Track and display location of buses (both NUS and public) on map
* Display arrival timings of buses by clicking on either bus stops or NUS buses on map
* Set alerts to notify user when selected bus service(s) is/are approaching selected bus stop(s)
* Basic user customization on Settings page

### Limitations
* Unable to get real-time data of NUS buses -- NUS buses movement are currently simulated
* Unable to implement push notification for alerts as require Apple developer license
* Unable to save current device state due to Apple's security restrictions

### Usage (For future implementation of kiosk at bus terminal)
_To add bus on road (When bus operator wants to start service):_
```
// BusHelper.AddBusOnRoad(vehiclePlate, routeName);
// example:
BusHelper.AddBusOnRoad("PC1221C", "A1");
```

_To remove bus on road (When bus operator finishes service):_
```
// BusHelper.RemoveBusOnRoad(vehiclePlate);
// example:
BusHelper.RemoveBusOnRoad("PC1221C");
```

### Future features
* Real-time data for NUS buses when it is available
* Local/remote notifications for alerts
* Display/post announcements for events e.g. bus service disruptions
* Allow user to report a problem or post feedback
