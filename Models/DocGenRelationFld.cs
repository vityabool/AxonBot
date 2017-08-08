using System;

namespace AxonPartners.Models
{
    [Serializable]
    public class DocGenRelationFld
    {
        public string ParamName { get; set; }
        public int IfYesTextId { get; set; }
        public int IfNoTextId { get; set; }
    }
}
