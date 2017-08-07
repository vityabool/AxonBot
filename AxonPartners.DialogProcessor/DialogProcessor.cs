using AxonPartners.Models;
using AxonPartners.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxonPartners
{
    [Serializable]
    public class DialogProcessor
    {
        public List<Answer> Answers { get; set; }
        public Question LastQuestion { get; set; }
        public DateTime LastQuestionAskTime { get; set; }
        public Answer LastAnswer { get; set; }
        public DateTime LastAnswerReceivedTime { get; set; }

        public Tuple<string, MessageTypes> GetNextQuestion(DbMessageEntity dbMessageEntity, object _answer = null, string lang = "en", int pid = 1)
        {
            string answer = _answer?.ToString();

            //Handle first message in pipeline
            if (answer == null)
            {
                LastQuestion = Settings.Instance.GetQuestionById(0, lang, pid);
                LastQuestionAskTime = DateTime.UtcNow;
                return new Tuple<string, MessageTypes>(LastQuestion.QuestionText, LastQuestion.MessageType);
            }
            else
            {
                //Handle commands messages
                if (answer.ToLower() == Settings.Instance.GetSettingValue("StartCmd").ToLower())
                {
                    LastQuestion = Settings.Instance.GetQuestionById(1, lang, pid);
                    LastQuestionAskTime = DateTime.UtcNow;
                    return new Tuple<string, MessageTypes>(LastQuestion.QuestionText, LastQuestion.MessageType);
                }

                //Handle restarting dialog
                if(LastQuestion == null)
                {
                    LastQuestion = Settings.Instance.GetQuestionById(0, lang, pid);
                    LastQuestionAskTime = DateTime.UtcNow;
                    return new Tuple<string, MessageTypes>(LastQuestion.QuestionText, LastQuestion.MessageType);
                }

                LastAnswerReceivedTime = DateTime.UtcNow;

                if (Answers == null) Answers = new List<Answer>();
                Answers.Add(new Answer { QuestionId = LastQuestion.Id, AnswerText = answer });

                //Logging last question and answer
                try { LogMessage(dbMessageEntity, LastQuestion, Answers.Last<Answer>()); } catch { }

                switch (LastQuestion.MessageType)
                {
                    case MessageTypes.YesNo:
                        switch (LastQuestion.YesNoOption)
                        {
                            case YesNoOptions.Question:
                                LastQuestion = Settings.Instance.GetQuestionById(LastQuestion.NextId, lang, pid);
                                LastQuestionAskTime = DateTime.UtcNow;
                                break;
                            case YesNoOptions.Logic:
                                if (bool.Parse(answer))
                                {
                                    LastQuestion = Settings.Instance.GetQuestionById(LastQuestion.LogicParameter.IfYesGoToId, lang, pid);
                                    LastQuestionAskTime = DateTime.UtcNow;
                                }
                                else
                                {
                                    LastQuestion = Settings.Instance.GetQuestionById(LastQuestion.LogicParameter.IfNoGoToId, lang, pid);
                                    LastQuestionAskTime = DateTime.UtcNow;
                                }
                                break;
                            case YesNoOptions.Exit:
                                if (bool.Parse(answer) == LastQuestion.ExitParameter.ExitAnswer)
                                {
                                    return new Tuple<string, MessageTypes>(LastQuestion.ExitParameter.ExitMessage, MessageTypes.Exit);
                                }
                                else
                                {
                                    LastQuestion = Settings.Instance.GetQuestionById(LastQuestion.NextId, lang, pid);
                                    LastQuestionAskTime = DateTime.UtcNow;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case MessageTypes.Text:
                        LastQuestion = Settings.Instance.GetQuestionById(LastQuestion.NextId, lang, pid);
                        LastQuestionAskTime = DateTime.UtcNow;
                        break;
                    case MessageTypes.Final:
                        return new Tuple<string, MessageTypes>(LastQuestion.QuestionText, MessageTypes.Final);
                    default:
                        LastQuestion = Settings.Instance.GetQuestionById(LastQuestion.NextId, lang, pid);
                        LastQuestionAskTime = DateTime.UtcNow;
                        break;
                }

                return new Tuple<string, MessageTypes>(LastQuestion.QuestionText, LastQuestion.MessageType);
            }
        }
        public Answer GetAnswerByQuestionParameter(string parameter, string lang = "en", int pid = 1)
        {
            int qId = Settings.Instance.GetQuestionByParameter(parameter, lang, pid).Id;
            return Answers.SingleOrDefault(x => x.QuestionId == qId);
        }
        public void LogMessage(DbMessageEntity entity, Question q, Answer a)
        {

            entity.QuestionId = q.Id;
            entity.QuestionAskTimeStamp = LastQuestionAskTime;
            entity.QuestionAnswerTimeStamp = LastAnswerReceivedTime;
            entity.QuestionAnswer = a.AnswerText;
            entity.QuestionLang = q.Lang;
            entity.QuestionText = q.QuestionText;

            //Dont care about result :)
            new StorageProvider().logMessage(entity);
        }
    }
}
