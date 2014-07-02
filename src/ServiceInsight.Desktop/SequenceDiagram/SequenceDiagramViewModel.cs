using System.Collections.Generic;

namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    public class SequenceDiagramViewModel
    {
        public SequenceDiagramViewModel()
        {
            Endpoints = new List<string> { "ECommerce", "Sales", "CustomerRelations", "ContentManagement", "Operations" };
        }

        public List<string> Endpoints { get; set; }
    }
}