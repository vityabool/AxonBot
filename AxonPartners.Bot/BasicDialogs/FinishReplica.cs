using AxonPartners.DAL;
using AxonPartners.Models;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;

namespace AxonPartners.Bot.Dialogs
{
    [Serializable]
    public class FinishReplica : IDialog<string>
    {
        private string userId;
        private DialogProcessor dialogProcessor;
        private string finalMessage;
        public FinishReplica() { userId = string.Empty; }
        public FinishReplica(string _userId, DialogProcessor _dialogProcessor, string _finalMessage)
        {
            userId = _userId;
            dialogProcessor = _dialogProcessor;
            finalMessage = _finalMessage;
        }
        public async Task StartAsync(IDialogContext context)
        {
            try
            {
                if (userId != null)
                {
                    Answer a = dialogProcessor.GetAnswerByQuestionParameter("Email");
                    if (a != null && !string.IsNullOrWhiteSpace(a.AnswerText))
                    {
                        await SqlServerProvider.UpdateEmail(userId, a.AnswerText.Trim().ToLower());
                    }
                }


                if (finalMessage == null)
                {
                    await context.PostAsync(Settings.Instance.GetTextBySettingValue("FinalDialMsgTxt"));
                }
                else
                {
                    await context.PostAsync(finalMessage);
                }

                DocGenerator dg = new DocGenerator();
                byte[] docBytes = dg.GeneratePrivacyPolicy(dialogProcessor.Answers);

                StorageProvider blobProvider = new StorageProvider();
                string URL = blobProvider.uploadToStorage(docBytes);

                await context.PostAsync(Settings.Instance.GetTextBySettingValue("DocLinkTxt") + URL);

                await context.PostAsync(Settings.Instance.GetTextBySettingValue("ByeByeMsgTxt"));

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