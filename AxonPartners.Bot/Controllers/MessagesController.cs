using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using AxonPartners.DAL;

namespace AxonPartners.Bot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                string msg = activity.Text.ToLower().Trim();
                if (msg == Settings.Instance.GetSettingValue("StartCmd"))
                {
                    //This is where the conversation gets reset!
                    activity.GetStateClient().BotState.DeleteStateForUser(activity.ChannelId, activity.From.Id);
                    await Conversation.SendAsync(activity, () => new RootDialog());
                }
                else if (msg == Settings.Instance.GetSettingValue("HelpCmd"))
                {
                    var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    Activity reply = activity.CreateReply(Settings.Instance.GetTextBySettingValue("HelpTxt"));
                    await connector.Conversations.SendToConversationAsync(reply);
                }
                else if(msg == Settings.Instance.GetSettingValue("FinishCmd"))
                {
                    var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    Activity reply = activity.CreateReply(Settings.Instance.GetTextBySettingValue("HelpTxt"));
                    await connector.Conversations.SendToConversationAsync(reply);

                    activity.GetStateClient().BotState.DeleteStateForUser(activity.ChannelId, activity.From.Id);
                }
                else
                {
                    try
                    {
                        await Conversation.SendAsync(activity, () => new RootDialog());
                    }
                    catch
                    {
                        var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                        Activity reply = activity.CreateReply(Settings.Instance.GetTextBySettingValue("HelpTxt"));
                        await connector.Conversations.SendToConversationAsync(reply);
                    }
                }
            }
            else
            {
                await HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
        private async Task<Activity> HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                if (message is IConversationUpdateActivity iConversationUpdated)
                {
                    ConnectorClient connector = new ConnectorClient(new System.Uri(message.ServiceUrl));

                    foreach (var member in iConversationUpdated.MembersAdded ?? System.Array.Empty<ChannelAccount>())
                    {
                        // if the bot is added, then
                        if (member.Id == iConversationUpdated.Recipient.Id)
                        {
                            await SqlServerProvider.AddUser(message.From.Id, message.From.Name);

                            var reply = ((Activity)iConversationUpdated).CreateReply(Settings.Instance.GetTextBySettingValue("GreeringTxt"));
                            await connector.Conversations.ReplyToActivityAsync(reply);
                            await Conversation.SendAsync(message, () => new RootDialog());
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
                if(message.Action == "add")
                {
                    ConnectorClient connector = new ConnectorClient(new System.Uri(message.ServiceUrl));

                    await SqlServerProvider.AddUser(message.From.Id, message.From.Name);
                    var reply = message.CreateReply(Settings.Instance.GetTextBySettingValue("GreeringTxt"));
                    await connector.Conversations.ReplyToActivityAsync(reply);

                    message.GetStateClient().BotState.DeleteStateForUser(message.ChannelId, message.From.Id);
                    await Conversation.SendAsync(message, () => new RootDialog());
                }
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}