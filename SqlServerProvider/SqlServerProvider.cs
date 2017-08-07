using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AxonPartners.DAL
{
    public static class SqlServerProvider
    {
        private const string SqlDbConnectionString = "Server=tcp:axon-srv.database.windows.net,1433;Initial Catalog=axonDb;Persist Security Info=False;User ID=axonadmin;Password=Qwerty#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static async Task<bool> AddUser(string id, string name)
        {
            bool result = false;
            using (SqlConnection connection = new SqlConnection(SqlDbConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand($"SELECT 1 as [Result] FROM [dbo].[Users] WHERE [Id] = '{id}';", connection);
                object execResult = command.ExecuteScalar();
                if (execResult == null)
                {
                    command = new SqlCommand($"INSERT INTO [dbo].[Users] ([Id], [Name]) VALUES ('{id}', N'{name}');", connection);
                    int rows = command.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }
        public static async Task<bool> UpdateEmail(string id, string email)
        {
            bool result = false;
            using (SqlConnection connection = new SqlConnection(SqlDbConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand($"SELECT 1 as [Result] FROM [dbo].[Users] WHERE [Id] = '{id}';", connection);
                object execResult = command.ExecuteScalar();
                if (execResult != null)
                {
                    command = new SqlCommand($"UPDATE [dbo].[Users] SET [Email] = '{email}' WHERE [Id] = '{id}';", connection);
                    int rows = command.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }
        public static async Task<bool> UpdateLang(string id, string lang)
        {
            bool result = false;
            using (SqlConnection connection = new SqlConnection(SqlDbConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand($"SELECT 1 as [Result] FROM [dbo].[Users] WHERE [Id] = '{id}';", connection);
                object execResult = command.ExecuteScalar();
                if (execResult != null)
                {
                    command = new SqlCommand($"UPDATE [dbo].[Users] SET [Lang] = '{lang}' WHERE [Id] = '{id}';", connection);
                    int rows = command.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        public static async Task<DataTable> LoadDialogPipeLine()
        {
            DataTable result = null;

            using (SqlConnection connection = new SqlConnection(SqlDbConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand($"SELECT [Pid], [Lang], [Id], [NextId], [MessageType], [YesNoOption], [QuestionText], [lpIfYesGoToId], [lpIfNoGoToId], [epExitAnswer], [epExitMessage], [ParamName], [IfYesTextId], [IfNoTextId] FROM [dbo].[DialogPipeline] ORDER BY [Pid], [Lang], [Id];", connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader != null && reader.HasRows)
                {
                    result = new DataTable();
                    result.Load(reader);
                }
                else
                {
                    result = null;
                }
            }

            return result;
        }
        public static async Task<DataTable> LoadSettings()
        {
            DataTable result = null;

            using (SqlConnection connection = new SqlConnection(SqlDbConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand($"SELECT [Id], [Parameter], [Value] FROM [dbo].[Settings] ORDER BY [Id];", connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader != null && reader.HasRows)
                {
                    result = new DataTable();
                    result.Load(reader);
                }
                else
                {
                    result = null;
                }
            }

            return result;
        }
        public static async Task<DataTable> LoadTexts()
        {
            DataTable result = null;

            using (SqlConnection connection = new SqlConnection(SqlDbConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand($"SELECT [Id], [Lang], [Text] FROM [dbo].[Texts] ORDER BY [Id];", connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader != null && reader.HasRows)
                {
                    result = new DataTable();
                    result.Load(reader);
                }
                else
                {
                    result = null;
                }
            }

            return result;
        }

        #region nonProd
        /// <summary>
        /// METHOD FOR TEST PURPOSE ONLY!
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<DataTable> LoadConversation(string user, string id)
        {
            DataTable result = null;

            using (SqlConnection connection = new SqlConnection(SqlDbConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand($"SELECT * FROM [dbo].[Messages] WHERE [UserId] = '{user}' AND [ConversationId] = '{id}' ORDER BY [QuestionId];", connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader != null && reader.HasRows)
                {
                    result = new DataTable();
                    result.Load(reader);
                }
                else
                {
                    result = null;
                }
            }

            return result;
        }
        /// <summary>
        /// NOT RECOMMENDED TO USE IN PRODUCTION AS OF RISK OF SQL INJECTION!
        /// </summary>
        /// <param name="sqlStmt"></param>
        /// <returns></returns>
        public static async Task<bool> RawInsert(string sqlStmt)
        {
            bool result = false;
            using (SqlConnection connection = new SqlConnection(SqlDbConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlStmt, connection);
                int rows = command.ExecuteNonQuery();
                if (rows > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }
        #endregion
    }
}
