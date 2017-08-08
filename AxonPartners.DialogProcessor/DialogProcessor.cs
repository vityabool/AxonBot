using AxonPartners.Models;
using AxonPartners.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using Models;

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

        private UserDialogState userDialogState { get; set; }

        public Tuple<string, MessageTypes> GetNextQuestion(DbMessageEntity dbMessageEntity, object _answer = null, string lang = "en")
        {
            string answer = _answer?.ToString();
            int pid = 1;
            int.TryParse(Settings.Instance.GetSettingValue("ActivePid"), out pid);

            //Handle first message in pipeline
            if (answer == null)
            {
                LastQuestion = Settings.Instance.GetQuestionById(0, lang, pid);
                LastQuestionAskTime = DateTime.UtcNow;
            }
            else
            {
                //Handle commands messages
                if (answer.ToLower() == Settings.Instance.GetSettingValue("StartCmd").ToLower())
                {
                    LastQuestion = Settings.Instance.GetQuestionById(1, lang, pid);
                    LastQuestionAskTime = DateTime.UtcNow;
                }
                //Handle restarting dialog
                else if(LastQuestion == null)
                {
                    LastQuestion = Settings.Instance.GetQuestionById(0, lang, pid);
                    LastQuestionAskTime = DateTime.UtcNow;
                }
                //Handle go to prevoius message
                else if (answer.ToLower() == Settings.Instance.GetSettingValue("PrevCmd").ToLower())
                {
                    if(Answers != null && Answers.Count > 1)
                    {
                        LastQuestion = Settings.Instance.GetQuestionById(Answers[Answers.Count - 1].QuestionId);
                        LastQuestionAskTime = DateTime.UtcNow;
                        Answers.RemoveAt(Answers.Count - 1);
                    }
                    else
                    {
                        LastQuestion = Settings.Instance.GetQuestionById(0, lang, pid);
                        LastQuestionAskTime = DateTime.UtcNow;
                        Answers = null;
                    }
                }
                //Execute normal scenario
                else
                {
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
                            //return new Tuple<string, MessageTypes>(LastQuestion.QuestionText, MessageTypes.Final);
                            break;
                        default:
                            LastQuestion = Settings.Instance.GetQuestionById(LastQuestion.NextId, lang, pid);
                            LastQuestionAskTime = DateTime.UtcNow;
                            break;
                    }
                }
            }

            return new Tuple<string, MessageTypes>(LastQuestion.QuestionText, LastQuestion.MessageType);
        }
        public Answer GetAnswerByQuestionParameter(string parameter, string lang = "en", int pid = 1)
        {
            int qId = Settings.Instance.GetQuestionByParameter(parameter, lang, pid).Id;
            return Answers.SingleOrDefault(x => x.QuestionId == qId);
        }
        public void ResetDialog()
        {
            LastQuestion = null;
            LastAnswer = null;
            Answers = null;
            userDialogState = null;
        }


        void LogMessage(DbMessageEntity entity, Question q, Answer a)
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
        void preloadState(DbMessageEntity entity)
        {
            if(userDialogState == null)
            {
                StorageProvider SP = new StorageProvider();
                userDialogState = SP.getDialogState(entity.ChannelId, entity.UserId);

                if (userDialogState != null)
                {
                    userDialogState = new UserDialogState
                    {
                        ChannelId = entity.ChannelId,
                        ConversationId = entity.ConversationId,
                        DialogId = Guid.NewGuid().ToString(),
                        IsFinished = false,
                        LastAskedQuestionId = 0,
                        sysUpdateDateUtc = DateTime.UtcNow,
                        UserId = entity.UserId
                    };

                    SP.addDialogState(userDialogState);
                }
            }
            
        }
        void saveState()
        {
            StorageProvider SP = new StorageProvider();
            SP.addDialogState(userDialogState);
        }
    }
}
