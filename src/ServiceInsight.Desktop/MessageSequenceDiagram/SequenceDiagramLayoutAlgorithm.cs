using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    using DevExpress.Xpf.Editors.Helpers;
    using DevExpress.XtraEditors.Filtering;
    using Mindscape.WpfDiagramming;
    using Mindscape.WpfDiagramming.Foundation;
    using System.Windows;

    public class SequenceDiagramLayoutAlgorithm : LayoutAlgorithmBase
    {
        // defines the amount of pixels per time part for the time scale
        private readonly double TimePart = 20.0; // for future use
        // the amount of pixels between 2 RoleNodes
        private readonly double SpaceBetweenRoleNodes = 20.0;

        readonly double maxNodeWidth = RoleNode.nodeHeaderWidth;

        IList<IDiagramNode> startNodes;

        public override void Run(IDiagramModel model)
        {
            startNodes = GetStartNodes(model);    

            for (int i = 0; i < startNodes.Count; i++)
            {
                var node = startNodes[i];
                if (IsIncluded(node))
                {
                    double x = (node.Bounds.Width + SpaceBetweenRoleNodes) * i;
                    node.Bounds = new Rect(x, 0, node.Bounds.Width, node.Bounds.Height);
                }
            }

            foreach (IDiagramNode node in model.Nodes)
            {
                if (IsIncluded(node) && ! startNodes.Contains(node))
                {
                    RoleNode topLevelNode = null;
                    int levelsDeep = 0;

                    GetRoleNodeAndLevelsDeep(node, topLevelNode, levelsDeep);

                    if (topLevelNode != null)
                    {
                        double x = (startNodes.IndexOf(topLevelNode) * maxNodeWidth) + (topLevelNode.Bounds.Width - node.Bounds.Width) / 2;
                        double y = topLevelNode.Bounds.Y + topLevelNode.Bounds.Height + TimePart;

                        node.Bounds = new Rect(x, y, node.Bounds.Width, node.Bounds.Height);
                    }
                }
                

            }
        }

        private IDiagramNode GetParent(IDiagramNode node)
        {
            IList<IDiagramNode> parents = new List<IDiagramNode>();
            List<IDiagramConnectionPoint> points = new List<IDiagramConnectionPoint>();
            foreach (IDiagramConnectionPoint point in node.ConnectionPoints)
            {
                points.Add(point);
            }
            foreach (IDiagramConnectionPoint point in points)
            {
                if (point.Edge == Edge.Top)
                {
                    foreach (IDiagramConnection connection in point.Connections)
                    {
                        if (connection.GetType() != typeof(EventConnection) && connection.GetType() != typeof(MessageConnection))
                        {
                            if (connection.FromConnectionPoint.Connectable is IDiagramNode)
                            {
                                IDiagramNode parent = connection.FromConnectionPoint.Connectable as IDiagramNode;
                                if (parent != null && IsIncluded(parent) && parent != node)
                                {
                                    parents.Add(parent);
                                }
                            }    
                        }
                    }
                }
            }
            if (parents.Count > 0)
            {
                return parents.First();
            }
            else
            {
                return null;
            }
        }

        private void GetRoleNodeAndLevelsDeep(IDiagramNode node, RoleNode roleNode, int levelsDeep)
        {
            var parent = GetParent(node);
            if (parent.GetType() != typeof(RoleNode))
            {
                levelsDeep++;
                GetRoleNodeAndLevelsDeep(parent, roleNode, levelsDeep);
            }
        }
    }
}
