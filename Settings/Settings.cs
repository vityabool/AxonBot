using AxonPartners.DAL;
using AxonPartners.Models;
using Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxonPartners
{
    /// <summary>
    /// Singleton class for all settings shared across backend
    /// http://www.csharpstar.com/singleton-design-pattern-csharp/
    /// </summary>
    public sealed class Settings
    {
        #region Singleton Initialization
        private static readonly Lazy<Settings> _Settings = new Lazy<Settings>(() => new Settings());
        public static Settings Instance { get { return _Settings.Value; } }
        private Settings() { }
        #endregion

        private List<SettingItem> _SettingItems;
        private List<TextItem> _TextItems;
        private List<Question> _Questions;
        public List<SettingItem> SettingItems { get { return _SettingItems; } }
        public List<TextItem> TextItems { get { return _TextItems; } }
        public List<Question> Questions { get { return _Questions; } }

        //Extended properties for Document Generator
        public IEnumerable<Question> QuestionsParameters
        {
            get
            {
                for (int i = 0; i < _Questions.Count; i++)
                {
                    if(_Questions[i].MessageType == MessageTypes.Text && _Questions[i].DocGenRelation != null && _Questions[i].DocGenRelation.ParamName != null)
                    {
                        yield return _Questions[i];
                    }
                }
            }
        }
        public IEnumerable<Question> QuestionsParagraphs
        {
            get
            {
                for (int i = 0; i < _Questions.Count; i++)
                {
                    if (((_Questions[i].MessageType == MessageTypes.YesNo && _Questions[i].YesNoOption == YesNoOptions.Logic) ||
                        (_Questions[i].MessageType == MessageTypes.YesNo && _Questions[i].YesNoOption == YesNoOptions.Question))
                         && _Questions[i].DocGenRelation != null && _Questions[i].DocGenRelation.ParamName != null)
                    {
                        yield return _Questions[i];
                    }
                }
            }
        }
        public Question QuestionHeader
        {
            get
            {
                return _Questions.Single(x => x.Id == 0);
            }
        }
        public Question QuestionFooter
        {
            get
            {
                return _Questions.Single(x => x.MessageType == MessageTypes.Final);
            }
        }

        public void Load()
        {
            DataTable DTs = SqlServerProvider.LoadSettings().Result;
            DataTable DTt = SqlServerProvider.LoadTexts().Result;
            DataTable DTq = SqlServerProvider.LoadDialogPipeLine().Result;

            if (DTs.Rows.Count > 0)
            {
                _SettingItems = new List<SettingItem>();
                foreach (DataRow row in DTs.Rows)
                {
                    SettingItem si = new SettingItem()
                    {
                        Id = Helpers.GetColumnValue<int>(row, "Id"),
                        Parameter = Helpers.GetColumnValue<string>(row, "Parameter").Trim(),
                        Value = Helpers.GetColumnValue<string>(row, "Value").Trim()
                    };
                    _SettingItems.Add(si);
                }
            }

            if (DTt.Rows.Count > 0)
            {
                _TextItems = new List<TextItem>();
                foreach (DataRow row in DTt.Rows)
                {
                    TextItem ti = new TextItem()
                    {
                        Id = Helpers.GetColumnValue<int>(row, "Id"),
                        Lang = Helpers.GetColumnValue<string>(row, "Lang").Trim(),
                        Text = Helpers.ReplaceWithParams(Helpers.GetColumnValue<string>(row, "Text"), SettingItems)
                    };
                    _TextItems.Add(ti);
                }
            }

            if (DTq.Rows.Count > 0)
            {
                _Questions = new List<Question>();

                foreach (DataRow row in DTq.Rows)
                {
                    Question q = new Question()
                    {
                        Pid = Helpers.GetColumnValue<int>(row, "Pid"),
                        Lang = Helpers.GetColumnValue<string>(row, "Lang"),
                        Id = Helpers.GetColumnValue<int>(row, "Id"),
                        NextId = Helpers.GetColumnValue<int>(row, "NextId"),
                        MessageType = Helpers.GetColumnValue<MessageTypes>(row, "MessageType"),
                        YesNoOption = Helpers.GetColumnValue<YesNoOptions>(row, "YesNoOption"),
                        QuestionText = Helpers.GetColumnValue<string>(row, "QuestionText"),
                        DocGenRelation = new DocGenRelationFld()
                        {
                            ParamName = Helpers.GetColumnValue<string>(row, "ParamName"),
                            IfYesTextId = Helpers.GetColumnValue<int>(row, "IfYesTextId"),
                            IfNoTextId = Helpers.GetColumnValue<int>(row, "IfNoTextId")
                        }
                    };
                    if (q.MessageType == MessageTypes.YesNo)
                    {
                        if (q.YesNoOption == YesNoOptions.Exit)
                        {
                            q.ExitParameter = new ExitParameterFld()
                            {
                                ExitAnswer = Helpers.GetColumnValue<bool>(row, "epExitAnswer"),
                                ExitMessage = Helpers.GetColumnValue<string>(row, "epExitMessage")
                            };
                        }
                        if (q.YesNoOption == YesNoOptions.Logic)
                        {
                            q.LogicParameter = new LogicParameterFld()
                            {
                                IfYesGoToId = Helpers.GetColumnValue<int>(row, "lpIfYesGoToId"),
                                IfNoGoToId = Helpers.GetColumnValue<int>(row, "lpIfNoGoToId")
                            };
                        }
                    }

                    _Questions.Add(q);
                }
            }
        }

        public string GetSettingValue(string name)
        {
            for (int i = 0; i < SettingItems.Count; i++)
            {
                if (SettingItems[i].Parameter.Trim().ToLower() == name.Trim().ToLower()) return SettingItems[i].Value;
            }
            return null;
        }
        public string GetSettingValue(int Id)
        {
            for (int i = 0; i < SettingItems.Count; i++)
            {
                if (SettingItems[i].Id == Id) return SettingItems[i].Value;
            }
            return null;
        }
        public string GetTextBySettingValue(string name, string lang = null)
        {
            if (lang == null) lang = GetSettingValue("DefaultLang").Trim().ToLower();

            for (int i = 0; i < SettingItems.Count; i++)
            {
                if (SettingItems[i].Parameter.Trim().ToLower() == name.Trim().ToLower())
                {
                    int siVal = int.Parse(SettingItems[i].Value.Trim());
                    for (int j = 0; j < TextItems.Count; j++)
                    {
                        if (TextItems[j].Id == siVal && TextItems[j].Lang.ToLower().Trim() == lang) return TextItems[j].Text;
                    }
                }
            }
            return null;
        }
        public string GetTextBySettingValue(int Id, string lang = null)
        {
            if (lang == null) lang = GetSettingValue("DefaultLang").Trim().ToLower();

            for (int i = 0; i < SettingItems.Count; i++)
            {
                if (SettingItems[i].Id == Id)
                {
                    int siVal = int.Parse(SettingItems[i].Value.Trim());
                    for (int j = 0; j < TextItems.Count; j++)
                    {
                        if (TextItems[j].Id == siVal && TextItems[j].Lang.ToLower().Trim() == lang) return TextItems[j].Text;
                    }
                }
            }
            return null;
        }
        public string GetTextById(int id, string lang = null)
        {
            if (lang == null) lang = GetSettingValue("DefaultLang").Trim().ToLower();

            for (int j = 0; j < TextItems.Count; j++)
            {
                if (TextItems[j].Id == id && TextItems[j].Lang.ToLower().Trim() == lang) return TextItems[j].Text;
            }

            return null;
        }
        public Question GetQuestionById(int id, string lang = null, int pid = 1)
        {
            if (lang == null) lang = GetSettingValue("DefaultLang").Trim().ToLower();

            for (int i = 0; i < Questions.Count; i++)
            {
                if (Questions[i].Id == id)
                {
                    return Questions[i];
                }
            }

            return null;
        }
        public Question GetQuestionByParameter(string parameter, string lang = null, int pid = 1)
        {
            if (lang == null) lang = GetSettingValue("DefaultLang").Trim().ToLower();

            for (int i = 0; i < Questions.Count; i++)
            {
                if (Questions[i].DocGenRelation.ParamName == parameter)
                {
                    return Questions[i];
                }
            }

            return null;
        }
    }
}
