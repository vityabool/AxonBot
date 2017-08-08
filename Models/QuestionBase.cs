using System;

namespace AxonPartners.Models
{
    [Serializable]
    public class QuestionBase
    {
        public int Pid { get; set; }
        public string Lang { get; set; }
        public int Id { get; set; }
        public int NextId { get; set; }
        public MessageTypes MessageType { get; set; }
        public YesNoOptions YesNoOption { get; set; }
        public string QuestionText { get; set; }
    }
}
