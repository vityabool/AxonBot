using System;

namespace AxonPartners.Models
{
    [Serializable]
    public class Question : QuestionBase
    {
        public LogicParameterFld LogicParameter { get; set; }
        public ExitParameterFld ExitParameter { get; set; }
        public DocGenRelationFld DocGenRelation { get; set; }
    }
}
