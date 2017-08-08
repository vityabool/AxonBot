using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;

namespace AxonPartners.Bot.Dialogs
{
    [Serializable]
    public class ExitReplica : IDialog<string>
    {
        private string finalMessage;
        public ExitReplica() { finalMessage = string.Empty; }
        public ExitReplica(string _finalMessage) { finalMessage = _finalMessage; }
        public async Task StartAsync(IDialogContext context)
        {
            try
            {
                await context.PostAsync(finalMessage);

                context.Done("OK");
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync(Settings.Instance.GetTextBySettingValue("DialRespErrorTxt"));
                context.Done("ERROR");
            }
            catch (TaskCanceledException ex)
            {
                await context.PostAsync(ex.Message);
                context.Done("ERROR");
            }
        }
    }
}