using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;

namespace ClientSample
{
    internal class SessionBrowser
    {
        Session _session;
        public SessionBrowser(Session session)
        {
            _session = session;
        }
        public ReferenceDescriptionCollection GetChildren(string nodeId = "", NodeClass nodeClass = NodeClass.Object)
        {
            NodeId startNode;
            if (string.IsNullOrEmpty(nodeId))
            {
                startNode = ObjectIds.ObjectsFolder;
            }
            else
            {
                startNode = ExpandedNodeId.ToNodeId(nodeId, _session.NamespaceUris);
            }
            ReferenceDescriptionCollection nextRefs;
            byte[] nextCp;
            _session.Browse(
                null,
                null,
                startNode,
                0u,
                BrowseDirection.Forward,
                ReferenceTypeIds.HierarchicalReferences,
                true,
                (uint)nodeClass,
                //(uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                out nextCp,
                out nextRefs);
            return nextRefs;
        }
    }
}
