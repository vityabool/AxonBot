using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;

namespace AxonPartners.Bot.BasicObjects
{
    [Serializable]
    public class BasicBooleanDialog : IDialog<string>
    {
        private string _question;
        private bool _backOption;

        public BasicBooleanDialog() { _question = string.Empty; }

        public BasicBooleanDialog(string question, bool backOption = true)
        {
            _question = question;
            _backOption = backOption;
        }

        public async Task StartAsync(IDialogContext context)
        {
            string[] PromptOptions = null;
            if(_backOption) PromptOptions = new string[] { "Yes", "No", "Back" };
            else PromptOptions = new string[] { "Yes", "No" };

            PromptDialog.Choice(
            context,
            AfterResetAsync,
            PromptOptions,
            _question, promptStyle: PromptStyle.Auto
            );
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<string> argument)
        {
            string res = await argument;

            context.Done(res);
        }
    }
}