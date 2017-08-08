using AxonPartners.DAL;
using AxonPartners;
using AxonPartners.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Settings;
using Models;

namespace TestDocGenApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //StorageProvider SP = new StorageProvider("DefaultEndpointsProtocol=https;AccountName=axonpartners;AccountKey=FSQ8v/b3U1KNqXtv/t9LdEaCFJPNm7leigGjJ+upAXy6+TXxT0DxEvT/3nzeHkyevnJiicteUFaIjhBDzVV/Lw==;EndpointSuffix=core.windows.net");
            //UserDialogState uds = new UserDialogState
            //{
            //    ChannelId = "skype",
            //    ConversationId = "123456",
            //    DialogId = Guid.NewGuid().ToString(),
            //    IsFinished = false,
            //    LastAskedQuestionId = 0,
            //    UserId = "spopla",
            //    sysUpdateDateUtc = DateTime.UtcNow
            //};

            //SP.addDialogState(uds);
            //UserDialogState uds = SP.getDialogState("skype", "spopla");

            //SP.updateDialogState(uds, false, 1, null);
            //SP.updateDialogState("skype", "spopla", true, 16, null);

            //AxonPartners.Settings.Instance.Load();
            //DataTable DT = SqlServerProvider.LoadConversation("default-user", "7gjeklfcia1b").Result;
            //List<Answer> answers = null;
            //if (DT.Rows.Count > 0)
            //{
            //    answers = new List<Answer>();
            //    foreach (DataRow row in DT.Rows)
            //    {
            //        Answer a = new Answer
            //        {
            //            QuestionId = Helpers.GetColumnValue<int>(row, "QuestionId"),
            //            AnswerText = Helpers.GetColumnValue<string>(row, "QuestionAnswer")
            //        };
            //        answers.Add(a);
            //    }
            //}

            //DocGenerator DG = new DocGenerator();
            //DG.GeneratePrivacyPolicy(answers);

            //DbMessageEntity DMO = new DbMessageEntity
            //{
            //    ChannelId = "X",
            //    ConversationId = "Y",
            //    FromId = "Z",
            //    MessageId = "A",
            //    QuestionId = 0,
            //    RecipientId = "B",
            //    ServiceUrl = "vortex.com",
            //    UserId = "C",
            //    FromName = "Sergiy Poplavskiy",
            //    RecipientName = "Ivan Ivanov",
            //    QuestionText = "How are you?",
            //    QuestionAnswer = "Fine, Thanks!",
            //    QuestionAnswerTimeStamp = DateTime.UtcNow,
            //    QuestionAskTimeStamp = DateTime.UtcNow.AddSeconds(-10),
            //    QuestionLang = "en",
            //};

            //StorageProvider SP = new StorageProvider();
            //SP.logMessage(DMO);

            //string sql = DMO.ToSqlInsert();

            //Console.WriteLine(sql);


            //DateTime format Experiments
            //Console.WriteLine("Type a date format and press enter");
            //while (true)
            //{
            //    Console.WriteLine(DateTime.Now.ToString(Console.ReadLine()));
            //}

            //PreloadFromDb();
            //DbManagement.AddUser("tx001", "Сергій Поплавський f").Wait();
            //DbManagement.AddUser("tx001", "Сергій Поплавський f").Wait();

            //DbManagement.UpdateEmail("tx001", "spopla@microsoft.com").Wait();
            //DbManagement.UpdateEmail("tx002", "spopla@microsoft.com").Wait();
            //PrivacyPolicyModel PPM = new PrivacyPolicyModel();
            //PPM.orgName = "Microsoft";
            //PPM.address = "Kyiv, Zhylianska 75, 4th floor";
            //PPM.projectName = "Azon Bot";
            //PPM.isToUExist = true;
            //PPM.linkToToU = "https://www.microsoft.com/en-us/legal/intellectualproperty/copyright";
            //PPM.hasUserRegistration = true;
            //PPM.hasContactForm = true;
            //PPM.linkToContactForm = "https://support.microsoft.com/en-us";
            //PPM.hasCookies = true;
            //PPM.useCameras = true;
            //PPM.sendMarketingEmails = true;
            //PPM.shareDataTo3dParties = true;
            //PPM.canMakePayments = true;
            //PPM.isExternalLinks = true;
            //PPM.email = "support@microsoft.com";


            //DocGenerator dg = new DocGenerator();
            //byte[] docBytes = dg.GeneratePrivacyPolicy(PPM);

            //BlobProvider blobProvider = new BlobProvider();
            //string URL = blobProvider.uploadToStorage(docBytes);

            //Console.WriteLine(URL);

            //Console.ReadLine();
        }

        //static List<Question> _Questions { get; set; }
        //static void PreloadFromDb()
        //{
        //    DataTable DT = DbManagement.LoadDialogPipeLine().Result;

        //    if (DT.Rows.Count > 0)
        //    {
        //        _Questions = new List<Question>();

        //        foreach (DataRow row in DT.Rows)
        //        {
        //            Question q = new Question()
        //            {
        //                Id = GetColumnValue<int>(row, "id"),
        //                NextId = GetColumnValue<int>(row, "NextId"),
        //                MessageType = GetColumnValue<MessageTypes>(row, "MessageType"),
        //                YesNoOption = GetColumnValue<YesNoOptions>(row, "YesNoOption"),
        //                QuestionText = GetColumnValue<string>(row, "QuestionText")
        //            };
        //            if (q.MessageType == MessageTypes.YesNo)
        //            {
        //                if (q.YesNoOption == YesNoOptions.Exit)
        //                {
        //                    q.ExitParameter = new Question.ExitParameterFld()
        //                    {
        //                        ExitAnswer = GetColumnValue<bool>(row, "epExitAnswer"),
        //                        ExitMessage = GetColumnValue<string>(row, "epExitMessage")
        //                    };
        //                }
        //                if (q.YesNoOption == YesNoOptions.Logic)
        //                {
        //                    q.LogicParameter = new Question.LogicParameterFld()
        //                    {
        //                        IfYesGoToId = GetColumnValue<int>(row, "lpIfYesGoToId"),
        //                        IfNoGoToId = GetColumnValue<int>(row, "lpIfNoGoToId")
        //                    };
        //                }
        //            }

        //            _Questions.Add(q);
        //        }
        //    }
        //}

        //public static T GetColumnValue<T>(DataRow row, string columnName)
        //{
        //    T value = default(T);

        //    if (row.Table.Columns.Contains(columnName) && row[columnName] != null && !String.IsNullOrWhiteSpace(row[columnName].ToString()))
        //    {
        //        if (typeof(Enum) == typeof(T).BaseType)
        //        {
        //            value = (T)Enum.Parse(typeof(T), row[columnName].ToString(), true);
        //        }
        //        else
        //        {
        //            value = (T)Convert.ChangeType(row[columnName].ToString(), typeof(T));
        //        }
        //    }

        //    return value;
        //}


        //public enum MessageTypes { YesNo, Text, Final, Exit }
        //public enum YesNoOptions { Question, Logic, Exit }
        //[Serializable]
        //public class Question
        //{
        //    public int Id { get; set; }
        //    public int NextId { get; set; }
        //    public MessageTypes MessageType { get; set; }
        //    public YesNoOptions YesNoOption { get; set; }
        //    public string QuestionText { get; set; }

        //    public LogicParameterFld LogicParameter { get; set; }
        //    public ExitParameterFld ExitParameter { get; set; }

        //    [Serializable]
        //    public class LogicParameterFld
        //    {
        //        public int IfYesGoToId { get; set; }
        //        public int IfNoGoToId { get; set; }
        //    }
        //    [Serializable]
        //    public class ExitParameterFld
        //    {
        //        public bool ExitAnswer { get; set; }
        //        public string ExitMessage { get; set; }
        //    }
        //}
    }
}
