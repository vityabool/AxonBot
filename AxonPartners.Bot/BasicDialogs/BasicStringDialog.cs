using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AxonPartners.Bot.Dialogs
{
    [Serializable]
    public class BasicStringDialog : IDialog<string>
    {
        private string _question;
        public BasicStringDialog() { _question = string.Empty; }
        public BasicStringDialog(string question)
        {
            _question = question;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(_question);

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            context.Done(message.Text);
        }
    }
}