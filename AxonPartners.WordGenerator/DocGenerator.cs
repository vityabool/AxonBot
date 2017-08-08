using AxonPartners.Models;
using Novacode;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AxonPartners
{
    public class DocGenerator
    {
        public byte[] GeneratePrivacyPolicy(List<Answer> Answers)
        {
            byte[] data = null;

            using (MemoryStream MS = new MemoryStream())
            {
                using (DocX doc = DocX.Create(MS, DocumentTypes.Document))
                {
                    //Adding header (if exists)
                    Question qh = Settings.Instance.QuestionHeader;

                    if (qh != null && qh.DocGenRelation != null && qh.DocGenRelation.IfYesTextId != default(int))
                    {
                        TextItem ti = Settings.Instance.TextItems.Single(x => x.Id == qh.DocGenRelation.IfYesTextId && x.Lang == qh.Lang);
                        if(ti != null && !string.IsNullOrWhiteSpace(ti.Text))
                        {
                            doc.InsertParagraph(ReplaceVariables(ti.Text, Answers));
                        }
                    }

                    foreach (Question q in Settings.Instance.QuestionsParagraphs)
                    {
                        //Finding answer for question
                        Answer a = Answers.Single(x => x.QuestionId == q.Id);

                        if(a != null)
                        {
                            bool answer = bool.Parse(a.AnswerText.Trim());

                            if (answer)
                            {
                                if (q.DocGenRelation.IfYesTextId != default(int))
                                {
                                    TextItem ti = Settings.Instance.TextItems.Single(x => x.Id == q.DocGenRelation.IfYesTextId && x.Lang == q.Lang);

                                    if (ti != null && !string.IsNullOrWhiteSpace(ti.Text))
                                    {
                                        doc.InsertParagraph(ReplaceVariables(ti.Text, Answers));
                                    }
                                }

                            }
                            else
                            {
                                if (q.DocGenRelation.IfNoTextId != default(int))
                                {
                                    TextItem ti = Settings.Instance.TextItems.Single(x => x.Id == q.DocGenRelation.IfNoTextId && x.Lang == q.Lang);

                                    if (ti != null && !string.IsNullOrWhiteSpace(ti.Text))
                                    {
                                        doc.InsertParagraph(ReplaceVariables(ti.Text, Answers));
                                    }
                                }
                            }
                        }
                        
                    }

                    //Adding footer (if exists)
                    Question qf = Settings.Instance.QuestionFooter;

                    if (qf != null && qf.DocGenRelation != null && qf.DocGenRelation.IfYesTextId != default(int))
                    {
                        TextItem ti = Settings.Instance.TextItems.Single(x => x.Id == qf.DocGenRelation.IfYesTextId && x.Lang == qf.Lang);
                        if (ti != null && !string.IsNullOrWhiteSpace(ti.Text))
                        {
                            doc.InsertParagraph(ReplaceVariables(ti.Text, Answers));
                        }
                    }

                    doc.Save();
                }

                data = MS.ToArray();
            }

            return data;
        }

        public string ReplaceVariables(string text, List<Answer> Answers)
        {
            foreach (Question q in Settings.Instance.QuestionsParameters)
            {
                Answer a = Answers.SingleOrDefault(x => x.QuestionId == q.Id);

                if(a != null)
                {
                    text = text.Replace($"*{q.DocGenRelation.ParamName}*", a.AnswerText);
                }
                
            }

            return text;
        }
    }
}
