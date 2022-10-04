// See https://aka.ms/new-console-template for more information
using ClientSample;
using Opc.Ua;
using Opc.Ua.Client;




string url = "opc.tcp://aci-contoso-a4h4m6w-plc1.centraluseuap.azurecontainer.io:50000";
var config = ConnectionConfig.Create(url);
string[] filter = "OpcPlc/Telemetry/Basic".Split('/');


EndpointDescription selectedEndpoint = CoreClientUtils.SelectEndpoint(url, false, (int)TimeSpan.FromSeconds(30).TotalMilliseconds);
EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(config);
ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);

Session session = await Session.Create(config, endpoint, false, "TestClient", (uint)TimeSpan.FromSeconds(60).TotalMilliseconds,
    new UserIdentity(new AnonymousIdentityToken()), null);


SessionBrowser sb = new SessionBrowser(session);

List<ExpandedNodeId> varsToWatch = new List<ExpandedNodeId>();

var references = sb.GetChildren();
foreach (var rd in references)
{ 
    if (rd.DisplayName.Text == filter[0])
    {
        Console.WriteLine(" {0}, {1}, {2} ", rd.DisplayName, rd.NodeId.ToString(), rd.NodeClass);
        ReferenceDescriptionCollection nextRefs = sb.GetChildren(rd.NodeId.ToString());
        foreach (var nextRd in nextRefs)
        {
            if (nextRd.DisplayName.Text == filter[1])
            {
                Console.WriteLine("   + {0}, {1}, {2} next", nextRd.DisplayName, nextRd.NodeId.ToString(), nextRd.NodeClass);
                ReferenceDescriptionCollection nextRefs2 = sb.GetChildren(nextRd.NodeId.ToString());

                foreach (var rrd in nextRefs2)
                {
                    if (rrd.DisplayName.Text == filter[2])
                    {
                        Console.WriteLine("   +  + {0}, {1}, {2} next", rrd.DisplayName, rrd.NodeId.ToString(), rrd.NodeClass);
                        ReferenceDescriptionCollection nr3 = sb.GetChildren(rrd.NodeId.ToString(), NodeClass.Variable);
                        foreach (var rrdd in nr3)
                        {
                            Console.WriteLine("   +  + {0}, {1}, {2} next", rrdd.DisplayName, rrdd.NodeId.ToString(), rrdd.NodeClass);
                            varsToWatch.Add(rrdd.NodeId);
                        }
                    }
                }
            }
        }
    }
}


foreach (ExpandedNodeId item in varsToWatch)
{
    Subscription subscription = new Subscription(session.DefaultSubscription) { PublishingInterval = (int)TimeSpan.FromSeconds(5).TotalMilliseconds };
    MonitoredItem monitoredNode = new MonitoredItem(subscription.DefaultItem) {
        DisplayName = item.Identifier.ToString(), // Program.MonitoredNodeId.Identifier.ToString(),
        StartNodeId = item.ToString()// Program.MonitoredNodeId,
    };
    monitoredNode.Notification += (item,e) => {
        foreach (var value in item.DequeueValues())
        {
            Console.WriteLine("{0}: {1}, {2}, {3}", item.DisplayName, value.Value, value.SourceTimestamp, value.StatusCode);
            //MonitoringClient.LatestValue = (int)value.Value;
        }
    };
    subscription.AddItem(monitoredNode);
    session.AddSubscription(subscription);
    subscription.Create();
}
Console.WriteLine("Subscribed to Values");
Console.ReadLine();
