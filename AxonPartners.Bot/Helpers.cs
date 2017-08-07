using AxonPartners.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AxonPartners
{
    public static class Helpers
    {
        public static T GetColumnValue<T>(DataRow row, string columnName)
        {
            T value = default(T);

            try
            {
                if (row.Table.Columns.Contains(columnName) && row[columnName] != null && !String.IsNullOrWhiteSpace(row[columnName].ToString()))
                {
                    if (typeof(Enum) == typeof(T).BaseType)
                    {
                        value = (T)Enum.Parse(typeof(T), row[columnName].ToString(), true);
                    }
                    else
                    {
                        value = (T)Convert.ChangeType(row[columnName].ToString(), typeof(T));
                    }
                }
            }
            catch { }

            return value;
        }

        public static string ReplaceWithParams(string inpText, List<SettingItem> settings)
        {
            if (!string.IsNullOrEmpty(inpText))
            {
                foreach (SettingItem settingItem in settings)
                {
                    inpText = inpText.Replace("{" + settingItem.Parameter + "}", settingItem.Value);
                }
            }

            return inpText;
        }
    }
}