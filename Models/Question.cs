using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxonPartners.Models
{
    public enum MessageTypes { YesNo, Text, Final, Exit }
    public enum YesNoOptions { Question, Logic, Exit }

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

    [Serializable]
    public class Question : QuestionBase
    {
        public LogicParameterFld LogicParameter { get; set; }
        public ExitParameterFld ExitParameter { get; set; }
        public DocGenRelationFld DocGenRelation { get; set; }
    }

    [Serializable]
    public class Answer
    {
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
    }

    [Serializable]
    public class LogicParameterFld
    {
        public int IfYesGoToId { get; set; }
        public int IfNoGoToId { get; set; }
    }
    [Serializable]
    public class ExitParameterFld
    {
        public bool ExitAnswer { get; set; }
        public string ExitMessage { get; set; }
    }
    [Serializable]
    public class DocGenRelationFld
    {
        public string ParamName { get; set; }
        public int IfYesTextId { get; set; }
        public int IfNoTextId { get; set; }
    }
    public class SettingItem
    {
        public int Id { get; set; }
        public string Parameter { get; set; }
        public string Value { get; set; }
    }
    public class TextItem
    {
        public int Id { get; set; }
        public string Lang { get; set; }
        public string Text { get; set; }
    }
}
