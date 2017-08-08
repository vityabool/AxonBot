using System;

namespace AxonPartners.Models
{
    [Serializable]
    public class LogicParameterFld
    {
        public int IfYesGoToId { get; set; }
        public int IfNoGoToId { get; set; }
    }
}
