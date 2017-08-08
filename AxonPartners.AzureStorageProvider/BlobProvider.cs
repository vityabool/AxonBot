using AxonPartners.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace AxonPartners.DAL
{
    public class StorageProvider
    {
        CloudStorageAccount account = null;

        public StorageProvider()
        {
            account = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
        }

        public StorageProvider(string connectionString)
        {
            account = CloudStorageAccount.Parse(connectionString);
            createArtifacts();
        }

        string containerName = "files";
        public string uploadToStorage(byte[] bytes)
        {
            string blobName = Guid.NewGuid().ToString() + ".docx";
            

            CloudBlockBlob blob = 
                account.CreateCloudBlobClient().
                GetContainerReference(containerName).
                GetBlockBlobReference(blobName);

            blob.UploadFromByteArray(bytes, 0, bytes.Length);

            return blob.Uri.ToString();
        }
        public bool logMessage(DbMessageEntity entity)
        {
            try
            {
                DynamicTableEntity dte = new DynamicTableEntity($"{entity.ConversationId}_{entity.UserId}", $"{entity.MessageId}");
                dte.Properties = convertToDictionary<DbMessageEntity>(entity);

                TableOperation insertOperation = TableOperation.Insert(dte, false);
                account.CreateCloudTableClient().
                    GetTableReference("messages").
                    Execute(insertOperation);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public UserDialogState getDialogState(string ChannelId, string userId)
        {
            try
            {
                TableOperation retreiveOperation = TableOperation.Retrieve(ChannelId, userId);

                var result = account.CreateCloudTableClient().
                        GetTableReference("states").
                        Execute(retreiveOperation);

                if (result == null || result.Result == null)
                {
                    return null;
                }
                else
                {
                    return convertFromDictionary<UserDialogState>((Dictionary<string, EntityProperty>)((DynamicTableEntity)result.Result).Properties);
                }
            }
            catch
            {
                return null;
            }
        }
        public bool updateDialogState(string ChannelId, string userId, bool? IsFinished = null, int? lastQuestionId = null, string DialogId = null)
        {
            return updateDialogState(getDialogState(ChannelId, userId), IsFinished, lastQuestionId, DialogId);
        }
        public bool updateDialogState(UserDialogState userDialogState, bool? IsFinished = null, int? lastQuestionId = null, string DialogId = null)
        {
            if(userDialogState != null)
            {
                if (IsFinished.HasValue) userDialogState.IsFinished = IsFinished.Value;
                if (lastQuestionId.HasValue) userDialogState.LastAskedQuestionId = lastQuestionId.Value;
                if (!string.IsNullOrEmpty(DialogId)) userDialogState.DialogId = DialogId;
                userDialogState.sysUpdateDateUtc = DateTime.UtcNow;
                addDialogState(userDialogState);

                return true;
            }
            else
            {
                return false;
            }
        }
        public bool addDialogState(UserDialogState userDialogState)
        {
            try
            {
                DynamicTableEntity dte = new DynamicTableEntity(userDialogState.ChannelId, userDialogState.UserId);
                dte.Properties = convertToDictionary<UserDialogState>(userDialogState);

                TableOperation insertOperation = TableOperation.InsertOrReplace(dte);
                account.CreateCloudTableClient().
                    GetTableReference("states").
                    Execute(insertOperation);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void createArtifacts()
        {
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            blobClient.GetContainerReference(containerName).CreateIfNotExists();

            CloudTableClient tableClient = account.CreateCloudTableClient();
            tableClient.GetTableReference("messages").CreateIfNotExists();
            tableClient.GetTableReference("states").CreateIfNotExists();
        }

        Dictionary<string, EntityProperty> convertToDictionary<T>(object instance)
        {
            try
            {
                Dictionary<string, EntityProperty> dte = new Dictionary<string, EntityProperty>();

                foreach (PropertyInfo pi in typeof(T).GetProperties())
                {
                    if (pi.PropertyType == typeof(int)) dte.Add(pi.Name, new EntityProperty((int?)pi.GetValue(instance)));
                    else if (pi.PropertyType == typeof(byte[])) dte.Add(pi.Name, new EntityProperty((byte[])pi.GetValue(instance)));
                    else if (pi.PropertyType == typeof(string)) dte.Add(pi.Name, new EntityProperty((string)pi.GetValue(instance)));
                    else if (pi.PropertyType == typeof(bool)) dte.Add(pi.Name, new EntityProperty((bool?)pi.GetValue(instance)));
                    else if (pi.PropertyType == typeof(DateTimeOffset)) dte.Add(pi.Name, new EntityProperty((DateTimeOffset?)pi.GetValue(instance)));
                    else if (pi.PropertyType == typeof(DateTime)) dte.Add(pi.Name, new EntityProperty((DateTime?)pi.GetValue(instance)));
                    else if (pi.PropertyType == typeof(double)) dte.Add(pi.Name, new EntityProperty((double?)pi.GetValue(instance)));
                    else if (pi.PropertyType == typeof(Guid)) dte.Add(pi.Name, new EntityProperty((Guid?)pi.GetValue(instance)));
                    else if (pi.PropertyType == typeof(long)) dte.Add(pi.Name, new EntityProperty((long?)pi.GetValue(instance)));
                }

                return dte;
            }
            catch { return null; }
        }
        T convertFromDictionary<T>(Dictionary<string, EntityProperty> props)
        {
            object entity = Activator.CreateInstance(typeof(T));

            PropertyInfo[] pia = typeof(T).GetProperties();

            foreach (KeyValuePair<string, EntityProperty> ep in props)
            {
                foreach (PropertyInfo pi in pia)
                {
                    if (ep.Key == pi.Name) pi.SetValue(entity, ep.Value.PropertyAsObject);
                }
            }

            return (T)Convert.ChangeType(entity, typeof(T));
        }
    }
}
