using AxonPartners.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AxonPartners.DAL
{
    public class StorageProvider
    {
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=axonpartners;AccountKey=FSQ8v/b3U1KNqXtv/t9LdEaCFJPNm7leigGjJ+upAXy6+TXxT0DxEvT/3nzeHkyevnJiicteUFaIjhBDzVV/Lw==;EndpointSuffix=core.windows.net";
        string containerName = "files";
        public string uploadToStorage(byte[] bytes)
        {
            string blobName = Guid.NewGuid().ToString() + ".docx";
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists();

            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            blob.UploadFromByteArray(bytes, 0, bytes.Length);

            return blob.Uri.ToString();
        }

        public bool logMessage(DbMessageEntity entity)
        {
            try
            {
                CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
                CloudTableClient tableClient = account.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("messages");

                table.CreateIfNotExists();

                DynamicTableEntity dte = new DynamicTableEntity($"{entity.ConversationId}_{entity.UserId}", $"{entity.MessageId}");

                foreach (PropertyInfo pi in typeof(DbMessageEntity).GetProperties())
                {
                    if (pi.PropertyType == typeof(int?)) dte.Properties.Add(pi.Name, new EntityProperty((int?)pi.GetValue(entity)));
                    else if (pi.PropertyType == typeof(byte[])) dte.Properties.Add(pi.Name, new EntityProperty((byte[])pi.GetValue(entity)));
                    else if (pi.PropertyType == typeof(string)) dte.Properties.Add(pi.Name, new EntityProperty((string)pi.GetValue(entity)));
                    else if (pi.PropertyType == typeof(bool?)) dte.Properties.Add(pi.Name, new EntityProperty((bool?)pi.GetValue(entity)));
                    else if (pi.PropertyType == typeof(DateTimeOffset?)) dte.Properties.Add(pi.Name, new EntityProperty((DateTimeOffset?)pi.GetValue(entity)));
                    else if (pi.PropertyType == typeof(DateTime?)) dte.Properties.Add(pi.Name, new EntityProperty((DateTime?)pi.GetValue(entity)));
                    else if (pi.PropertyType == typeof(double?)) dte.Properties.Add(pi.Name, new EntityProperty((double?)pi.GetValue(entity)));
                    else if (pi.PropertyType == typeof(Guid?)) dte.Properties.Add(pi.Name, new EntityProperty((Guid?)pi.GetValue(entity)));
                    else if (pi.PropertyType == typeof(long?)) dte.Properties.Add(pi.Name, new EntityProperty((long?)pi.GetValue(entity)));
                }

                TableOperation insertOperation = TableOperation.Insert(dte, false);
                table.Execute(insertOperation);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
