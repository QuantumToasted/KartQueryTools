# KartQueryTools
A small set of tools for querying [SRB2Kart](https://wiki.srb2.org/wiki/SRB2Kart) servers as well as the Master Server which hosts them.
Simple to use with little to no setup required, as there are not that many methods.

## Example usage
```cs
using KartQueryTools;

// Queries an SRB2Kart server via hostname.
var server = KartQuery.QueryServer("kart.kuro.mu");
Console.WriteLine(server.Name);

// Queries a list of all SRB2Kart servers currently on the Master Server.
// It's recommended to cache these results instead of querying every time you need servers.
var allServers = KartQuery.QueryMasterServer();
Console.WriteLine(allServers[0].Name);
```
