using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AxonPartners.Bot.BasicObjects
{
    [Serializable]
    public class BasicBooleanDialog : IDialog<bool>
    {
        private string _question;

        public BasicBooleanDialog() { _question = string.Empty; }

        public BasicBooleanDialog(string question)
        {
            _question = question;
        }

        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Confirm(
            context,
            AfterResetAsync,
            _question,
            Settings.Instance.GetTextBySettingValue("BoolInputErrorTxt") + _question,
            promptStyle: PromptStyle.Auto);
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            bool confirm = await argument;

            context.Done(confirm);
        }
    }
}