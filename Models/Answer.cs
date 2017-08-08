using System;

namespace AxonPartners.Models
{
    [Serializable]
    public class Answer
    {
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
    }
}
