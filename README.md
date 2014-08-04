Deployment
==========

Deployment source code from client to server.

Put your source machine configuration here ClientSettings.xml
```
<ClientList>
  <Client>
    <ProjectName>app1</ProjectName>
    <Path>C:\Users\myuser\Desktop\app1</Path>
  </Client>
</ClientList>
```

Put your target machine configuration here ServerSettings.xml
```
<ServerList>
  <Server>
    <ClientName>app1</ClientName>
    <ProjectName>app1 - machine1</ProjectName>
    <Path>\\myserver\Application\app1</Path>
  </Server>
  <Server>
    <ClientName>app1</ClientName>
    <ProjectName>app1 - machine2</ProjectName>
    <Path>\\myserver\Application\app2</Path>
  </Server>
  <Server>
    <ClientName>app1</ClientName>
    <ProjectName>app1 - machine3</ProjectName>
    <Path>\\myserver\Application\app3</Path>
  </Server>
</ServerList>
```

Status
-----------
[![Build status](https://ci.appveyor.com/api/projects/status/9hscl0mc3h0dp6k5)](https://ci.appveyor.com/project/dinobenz/deployment)
