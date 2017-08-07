using AxonPartners.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace AxonPartners.Bot.Models
{
    public class ConversationStarter : ConversationStarterBase
    {
        public override async Task Resume(string channelId, string text, string conversationId = null, string locale = "en-Us")
        {
            var userAccount = new ChannelAccount(ToId, ToName);
            var botAccount = new ChannelAccount(FromId, FromName);
            var connector = new ConnectorClient(new Uri(ServiceUrl));

            IMessageActivity message = Activity.CreateMessageActivity();
            if (!string.IsNullOrEmpty(conversationId) && !string.IsNullOrEmpty(channelId))
            {
                message.ChannelId = channelId;
            }
            else
            {
                conversationId = (await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount)).Id;
            }
            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(id: conversationId);
            message.Text = text;
            message.Locale = locale;
            await connector.Conversations.SendToConversationAsync((Activity)message);
        }

        public static ConversationStarter FromJSON(string json)
        {
            return JsonConvert.DeserializeObject<ConversationStarter>(json);
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}