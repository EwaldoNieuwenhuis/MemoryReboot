using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BusinessMemory;
using System.Data.Common;
using System.Drawing;

namespace MemoryDataAccess
{
    public class MemoryRecordRepository : IMemoryItemRepository<MemoryRecord>
    {
        public string ConnectionString { get; set; }

        private const string DefaultConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Ewaldo;Integrated Security=True;Connect Timeout=30;Encrypt=False;";

        public MemoryRecordRepository()
        {
            ConnectionString = DefaultConnectionString;
        }

        public MemoryRecordRepository(string connectionstring)
        {
            ConnectionString = connectionstring;
        }

        public ICollection<MemoryRecord> ReadAll()
        {
            var records = new List<MemoryRecord>();
            string sql = "SELECT * FROM MemoryRecords;";

            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string playerName = reader.GetString(1);
                        int amountOfCards = reader.GetInt32(2);
                        int seconds = reader.GetInt32(3);
                        int attempts = reader.GetInt32(4);
                        DateTime dateTime = reader.GetDateTime(5);
                        var record = new MemoryRecord(amountOfCards, seconds, attempts, playerName, dateTime);
                        record.Id = id;
                        records.Add(record);
                    }
                    connection.Close();
                }
            }
            return records;
        }

        public void Create(MemoryRecord? memoryRecord, out int rowsAffected, out MemoryRecord? newRecord)
        {
            string sql = "INSERT INTO MemoryRecords (PlayerName, AmountOfCards, Seconds, Attempts) VALUES (@PlayerName, @AmountOfCards, @Seconds, @Attempts);";

            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("PlayerName", memoryRecord.PlayerName);
                    command.Parameters.AddWithValue("AmountOfCards", memoryRecord.AmountOfCards);
                    command.Parameters.AddWithValue("Seconds", memoryRecord.Seconds);
                    command.Parameters.AddWithValue("Attempts", memoryRecord.Attempts);
                    connection.Open();
                    // Execute the command to insert the record and retrieve the generated Id
                    object result = command.ExecuteScalar();
                    // Check if the result is not null and convert it to the generated Id
                    if (result != null && result != DBNull.Value)
                    {
                        newRecord = memoryRecord;
                        newRecord.Id = Convert.ToInt32(result);
                        rowsAffected = 1;
                    }
                    else
                    {
                        newRecord = null;
                        rowsAffected = 0;
                    }
                    connection.Close();
                }
            }

        }

        public void DeleteAll(out int rowsAffected)
        {
            string sql = "DELETE FROM MemoryRecords;";
            rowsAffected = 0;

            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

        }

        public void Update(int id, MemoryRecord record, out int rowsAffected, out MemoryRecord updatedRecord)
        {
            string sql = "UPDATE MemoryRecords SET PlayerName = @PlayerName, AmountOfCards = @AmountOfCards, Seconds = @Seconds, Attempts = @Attempts WHERE Id = @Id;";

            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PlayerName", record.PlayerName);
                    command.Parameters.AddWithValue("@AmountOfCards", record.AmountOfCards);
                    command.Parameters.AddWithValue("@Seconds", record.Seconds);
                    command.Parameters.AddWithValue("@Attempts", record.Attempts);
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    rowsAffected = command.ExecuteNonQuery();
                    updatedRecord = record;
                    updatedRecord.Id = id;
                    connection.Close();
                }
            }


        }

        public void Delete(int id, out int rowsAffected)
        {
            string sql = "DELETE FROM MemoryRecords WHERE Id = @Id;";
            rowsAffected = 0;

            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

        }

        public ICollection<MemoryRecord> ReadAllWhere(Dictionary<string, object> criteria)
        {
            // Bouw de WHERE-clausule op basis van de criteria
            string whereClause = string.Join(" AND ", criteria.Select(c => $"{c.Key} = @{c.Key}"));

            string sql = $"SELECT * FROM MemoryRecords WHERE {whereClause};";
            var records = new List<MemoryRecord>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    // Voeg parameters toe voor de criteria
                    foreach (var criterion in criteria)
                    {
                        command.Parameters.AddWithValue($"@{criterion.Key}", criterion.Value);
                    }

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string playerName = reader.GetString(1);
                            int amountOfCards = reader.GetInt32(2);
                            int seconds = reader.GetInt32(3);
                            int attempts = reader.GetInt32(4);
                            DateTime dateTime = reader.GetDateTime(5);
                            MemoryRecord memoryRecord = new MemoryRecord(amountOfCards, seconds, attempts, playerName, dateTime);
                            memoryRecord.Id = id;
                            records.Add(memoryRecord);

                        }
                    }
                    connection.Close();
                }
            }

            return records;
        }

        /// <summary>
        /// Checks if an item exists based on the playername, if not it creates
        /// if it does exist it will look if the score is higher
        /// if its higher it will update if not it will just return the record
        /// </summary>
        public MemoryRecord CreateReadOrUpdate(MemoryRecord? record)
        {
            Dictionary<string, object> WhereClause = new Dictionary<string, object>();
            WhereClause.Add("PlayerName", record.PlayerName);
            //find record on name
            ICollection<MemoryRecord> memoryRecordsWithName = ReadAllWhere(WhereClause);
            if (memoryRecordsWithName.Count > 0)
            {
                foreach (var memoryRecord in memoryRecordsWithName)
                {
                    //check for a new record
                    if (memoryRecord.CalculatePoints() < record.CalculatePoints())
                    {
                        //update
                        int rows;
                        MemoryRecord updatedRecord;
                        Update(memoryRecord.Id, record, out rows, out updatedRecord);
                        return updatedRecord;

                    }
                }

                return record;
            }
            else
            {
                //create
                int rows = 0;
                Create(record, out rows, out record);
                return record;
            }



        }

        public MemoryRecord? Read(int id)
        {
            string sql = "SELECT * FROM MemoryRecords WHERE Id = @Id;";
            MemoryRecord? memoryRecord = null;

            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int idRecord = reader.GetInt32(0);
                            string playerName = reader.GetString(1);
                            int amountOfCards = reader.GetInt32(2);
                            int seconds = reader.GetInt32(3);
                            int attempts = reader.GetInt32(4);
                            DateTime dateTime = reader.GetDateTime(5);
                            memoryRecord = new MemoryRecord(amountOfCards, seconds, attempts, playerName, dateTime);
                            memoryRecord.Id = idRecord;
                        }
                    }
                    connection.Close();
                }
            }

            return memoryRecord;
        }

        public Dictionary<string, List<object>> ReadAllWithColumns(string query = "SELECT * FROM MemoryRecords")
        {
            // Maak een dictionary met string als sleutel en een lijst van strings als waarde
            Dictionary<string, List<object>> table = new Dictionary<string, List<object>>();


            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    // Execute the query and read the results
                    using (var reader = command.ExecuteReader())
                    {
                        List<string> columns = new List<string>();
                        // Add column names as header
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            table[reader.GetName(i)] = new List<object>();
                        }
                        

                        // Add rows
                        while (reader.Read())
                        {
                            // Loop through the columns and append them to the raw output string
                            for (int i = 0; i < reader.FieldCount; i++)
                            {

                                    if (int.TryParse(reader[i].ToString(), out int number))
                                    {
                                        table[reader.GetName(i)].Add(number);

                                    }
                                    else
                                    {
                                        table[reader.GetName(i)].Add(reader[i].ToString());
                                    }
                                
                                

                               
                            }

                        }
                        
                    }
                }
                return table;
            }


        }


    }
}

