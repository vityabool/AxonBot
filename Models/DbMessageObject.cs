using Microsoft.Bot.Connector;
using System;
using System.Reflection;

namespace AxonPartners.Models
{
    public class DbMessageEntity
    {
        public string UserId { get; set; }
        public string ConversationId { get; set; }
        public string MessageId { get; set; }
        public string ChannelId { get; set; }
        public string ServiceUrl { get; set; }
        public string FromId { get; set; }
        public string FromName { get; set; }
        public string RecipientId { get; set; }
        public string RecipientName { get; set; }
        public int QuestionId { get; set; }
        public string QuestionLang { get; set; }
        public string QuestionText { get; set; }
        public string QuestionAnswer { get; set; }
        public DateTime QuestionAskTimeStamp { get; set; }
        public DateTime QuestionAnswerTimeStamp { get; set; }
        public double TimeElapsedSec
        {
            get
            {
                double totalSec = (QuestionAnswerTimeStamp - QuestionAskTimeStamp).TotalSeconds;
                if (totalSec > int.MaxValue) return int.MaxValue;
                else return Convert.ToInt32(totalSec);
            }
        }
        public string UserLocale { get; set; }
        public DateTime UserTimeStamp { get; set; }
        public string ActivityId { get; set; }

        public string ToSqlInsert(string tableName = "[dbo].[Messages]")
        {
            PropertyInfo[] properties = this.GetType().GetProperties();

            //prepare INTO predicate
            string intoPredicate = null;
            for (int i = 0; i < properties.Length; i++)
            {
                intoPredicate += $"[{properties[i].Name}]" + (i < properties.Length - 1 ? "," : "");
            }

            //prepare VALUES predicate
            string values = null;
            for (int i = 0; i < properties.Length; i++)
            {
                if(properties[i].PropertyType == typeof(DateTime))
                {
                    DateTime DT = (DateTime)properties[i].GetValue(this);
                    if(DT == default(DateTime))
                    {
                        values += "NULL" + (i < properties.Length - 1 ? "," : "");
                    }
                    else
                    {
                        values += $"CAST('{DT.ToString("yyyy-MM-dd HH:mm:ss.fffffff")}' AS datetime2(7))" + (i < properties.Length - 1 ? "," : "");
                    }
                    
                }
                else if(properties[i].PropertyType == typeof(int))
                {
                    values += properties[i].GetValue(this).ToString() + (i < properties.Length - 1 ? "," : "");
                }
                else
                {
                    object val = properties[i].GetValue(this);
                    if (val == null || (val is string && string.IsNullOrEmpty((string)val)))
                    {
                        values += "NULL" + (i < properties.Length - 1 ? "," : "");
                    }
                    else
                    {
                        values += $"N'{val.ToString().Replace("'", "''")}'" + (i < properties.Length - 1 ? "," : "");
                    }
                    
                }
            }

            return $"INSERT INTO {tableName} ({intoPredicate}) VALUES ({values});";
        }

        public static implicit operator DbMessageEntity(Activity activity)
        {
            return new DbMessageEntity()
            {
                ChannelId = activity.ChannelId,
                ConversationId = activity.Conversation.Id,
                FromId = activity.From.Id,
                FromName = activity.From.Name,
                MessageId = activity.Id,
                RecipientId = activity.Recipient.Id,
                RecipientName = activity.Recipient.Name,
                ServiceUrl = activity.ServiceUrl,
                UserId = activity.From.Id,
                ActivityId = activity.Id,
                UserTimeStamp = activity.LocalTimestamp.GetValueOrDefault().DateTime,
                UserLocale = activity is IMessageActivity ? ((IMessageActivity)activity).Locale : null
            };
        }
    }
}
