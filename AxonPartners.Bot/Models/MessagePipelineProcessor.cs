using AxonPartners.SqlServerProvider;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using AxonPartners.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AxonPartners.Bot.Models
{
    [Serializable]
    public class MessagePipelineProcessor
    {
        public List<Answer> Answers { get; set; }
        public Question LastQuestion { get; set; }
        public DateTime LastQuestionAskTime { get; set; }
        public Answer LastAnswer { get; set; }
        public DateTime LastAnswerReceivedTime { get; set; }

        public Tuple<string, MessageTypes> GetNextQuestion(IDialogContext context, object _answer = null, string lang = "en", int pid = 1)
        {
            string answer = _answer?.ToString();

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

                LastAnswerReceivedTime = DateTime.UtcNow;

                if (Answers == null) Answers = new List<Answer>();
                Answers.Add(new Answer { QuestionId = LastQuestion.Id, AnswerText = answer });

                //Logging last question and answer
                try { LogMessage(context, LastQuestion, Answers.Last<Answer>()); } catch { }

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

        public void LogMessage(IDialogContext context, Question q, Answer a)
        {
            DbMessageEntity DMO = new DbMessageEntity()
            {
                ChannelId = context.Activity.ChannelId,
                ConversationId = context.Activity.Conversation.Id,
                FromId = context.Activity.From.Id,
                FromName = context.Activity.From.Name,
                MessageId = context.Activity.Id,
                RecipientId = context.Activity.Recipient.Id,
                RecipientName = context.Activity.Recipient.Name,
                ServiceUrl = context.Activity.ServiceUrl,
                UserId = context.Activity.From.Id,
                ActivityId = context.Activity.Id,
                UserTimeStamp = context.Activity.LocalTimestamp.GetValueOrDefault().DateTime,
                UserLocale = context.Activity is IMessageActivity ? ((IMessageActivity)context.Activity).Locale : null,
                QuestionId = q.Id,
                QuestionAskTimeStamp = LastQuestionAskTime,
                QuestionAnswerTimeStamp = LastAnswerReceivedTime,
                QuestionAnswer = a.AnswerText,
                QuestionLang = q.Lang,
                QuestionText = q.QuestionText
            };

            bool res = DbManagement.RawInsert(DMO.ToSqlInsert()).Result;
        }
    }
}


//static List<Question> SampleMessagesPipeline()
//{
//    List<Question> pipeline = new List<Question>
//    {
//        new Question
//        {
//            Id = 0,
//            NextId = 1,
//            QuestionText = "QiD: 0. Would you like to get started?",
//            MessageType = MessageTypes.YesNo,
//            YesNoOption = YesNoOptions.Exit,
//            ExitParameter = new ExitParameterFld
//            {
//                ExitAnswer = false,
//                ExitMessage = "OK, see you next time!"
//            }
//        },

//        new Question
//        {
//            Id = 1,
//            NextId = 2,
//            QuestionText = "QiD: 1. OK, now we will go thrue sample message pipeline. Type of answer is Text. Type some text and press enter...",
//            MessageType = MessageTypes.Text
//        },

//        new Question
//        {
//            Id = 2,
//            NextId = 3,
//            QuestionText = "QiD: 2. Another 'Text' answer type. Type some text and press enter...",
//            MessageType = MessageTypes.Text
//        },

//        new Question
//        {
//            Id = 3,
//            NextId = 4,
//            QuestionText = "QiD: 3. This is a simple 'YesNo' question. Choose any answer.",
//            MessageType = MessageTypes.YesNo,
//            YesNoOption = YesNoOptions.Question
//        },

//        new Question
//        {
//            Id = 4,
//            NextId = 5,
//            QuestionText = "QiD: 4. And one more time :)",
//            MessageType = MessageTypes.YesNo,
//            YesNoOption = YesNoOptions.Question
//        },

//        new Question
//        {
//            Id = 5,
//            NextId = 7,
//            QuestionText = "QiD: 5. This is a chooser. Choose 'Yes' to go to QiD 6 or 'No' to go to QiD 7",
//            MessageType = MessageTypes.YesNo,
//            YesNoOption = YesNoOptions.Logic,
//            LogicParameter = new LogicParameterFld
//            {
//                IfYesGoToId = 6,
//                IfNoGoToId = 7
//            }
//        },

//        new Question
//        {
//            Id = 6,
//            NextId = 7,
//            QuestionText = "QiD: 6. You have choosed 'Yes'. Welcome here :) Type any text and press enter.",
//            MessageType = MessageTypes.Text
//        },

//        new Question
//        {
//            Id = 7,
//            NextId = 8,
//            QuestionText = "QiD: 7. You have choosed 'No'. Welcome here :) Type any text and press enter.",
//            MessageType = MessageTypes.Text
//        },

//        new Question
//        {
//            Id = 8,
//            NextId = 9,
//            QuestionText = "QiD: 8. This is exit question. Choose 'Yes' to exit now or 'No' to go to final question",
//            MessageType = MessageTypes.YesNo,
//            YesNoOption = YesNoOptions.Exit,
//            ExitParameter = new ExitParameterFld
//            {
//                ExitAnswer = true,
//                ExitMessage = "Thank you! See you next Time"
//            }
//        },

//        new Question
//        {
//            Id = 9,
//            NextId = -1,
//            QuestionText = "QiD: 9. This is final question. Type any text to continue.",
//            MessageType = MessageTypes.Final
//        }
//    };
//    return pipeline;
//}