using AxonPartners.Bot.BasicObjects;
using AxonPartners.Bot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using AxonPartners.Models;
using System;
using System.Threading.Tasks;

namespace AxonPartners.Bot
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        //private PrivacyPolicyModel privacyPolicyModel = new PrivacyPolicyModel();
        private DialogProcessor dialogProcessor = new DialogProcessor();
        private string userId = null;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            userId = message.From.Id;

            ProcessMessage(dialogProcessor.GetNextQuestion((Activity)context.Activity, message.Text), context);
        }

        #region DialogStack
        private async Task StringDialogMessageReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                ProcessMessage(dialogProcessor.GetNextQuestion((Activity)context.Activity, await result), context);
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync(Settings.Instance.GetTextBySettingValue("DialRespErrorTxt"));
            }
        }
        private async Task BoolDialogMessageReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string normalizedResponce = await result;

                if (normalizedResponce.Trim().ToLower() == "yes") normalizedResponce = "True";
                else if (normalizedResponce.Trim().ToLower() == "no") normalizedResponce = "False";
                else if (normalizedResponce.Trim().ToLower() == "back") normalizedResponce = Settings.Instance.GetSettingValue("PrevCmd");

                ProcessMessage(dialogProcessor.GetNextQuestion((Activity)context.Activity, normalizedResponce), context);
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync(Settings.Instance.GetTextBySettingValue("DialRespErrorTxt"));
            }
        }
        private async Task EOFConversation(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                dialogProcessor.ResetDialog();
                context.Done(1);
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync(Settings.Instance.GetTextBySettingValue("DialRespErrorTxt"));
            }
        }
        public void ProcessMessage(Tuple<string, MessageTypes> q, IDialogContext context)
        {
            switch (q.Item2)
            {
                case MessageTypes.YesNo:
                    context.Call(new BasicBooleanDialog(q.Item1, dialogProcessor.LastQuestion.Id != 0), this.BoolDialogMessageReceivedAsync);
                    break;
                case MessageTypes.Text:
                    context.Call(new BasicStringDialog(q.Item1), this.StringDialogMessageReceivedAsync);
                    break;
                case MessageTypes.Final:
                    context.Call(new FinishReplica(userId, dialogProcessor, q.Item1), this.EOFConversation);
                    break;
                case MessageTypes.Exit:
                    context.Call(new ExitReplica(q.Item1), this.EOFConversation);
                    break;
                default:
                    context.Call(new ExitReplica(q.Item1), this.EOFConversation);
                    break;
            }
        }
        #endregion
    }
}