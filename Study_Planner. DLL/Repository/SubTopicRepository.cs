using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Study_Planner._DLL.IRepository;
using Study_Planner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner._DLL.Repository
{
    public class SubTopicRepository : ISubTopicRepository
    {
        private readonly string _connectionString;

        public SubTopicRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> AddSubTopicAsync(int topicId,SubTopics subTopicDto)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction()) // Begin Transaction
                {
                    try
                    {
                        // Insert SubTopic
                        var cmd = new SqlCommand(
                            @"INSERT INTO SubTopics (SubTopicName, TopicId, OrderIndex, DifficultyLevel, EstimatedCompletionTime, IsActive)
                      VALUES (@subTopicName, @topicId, @orderIndex, @difficultyLevel, @estimatedCompletionTime, @isActive);
                      SELECT SCOPE_IDENTITY();",
                            conn, transaction);

                        cmd.Parameters.AddWithValue("@subTopicName", subTopicDto.SubTopicName);
                        cmd.Parameters.AddWithValue("@topicId", topicId);
                        cmd.Parameters.AddWithValue("@orderIndex", subTopicDto.OrderIndex);
                        cmd.Parameters.AddWithValue("@difficultyLevel", subTopicDto.DifficultyLevel);
                        cmd.Parameters.AddWithValue("@estimatedCompletionTime", subTopicDto.EstimatedCompletionTime);
                        cmd.Parameters.AddWithValue("@isActive", subTopicDto.IsActive);

                        var subTopicId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                        await transaction.CommitAsync(); // Commit Transaction
                        return subTopicId;
                    }
                    catch
                    {
                        await transaction.RollbackAsync(); // Rollback if any failure
                        throw;
                    }
                }
            }
        }

        public async Task<IEnumerable<SubTopics>> GetAllSubTopicsAsync()
        {
            var subTopics = new List<SubTopics>();

            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT * FROM SubTopics", conn);
                conn.Open();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        subTopics.Add(new SubTopics
                        {
                            id = (int)reader["id"],
                            SubTopicName = reader["SubTopicName"].ToString(),
                            TopicId = (int)reader["TopicId"],
                            OrderIndex = (int)reader["OrderIndex"],
                            DifficultyLevel = (int)reader["DifficultyLevel"],
                            EstimatedCompletionTime = (int)reader["EstimatedCompletionTime"],
                            IsActive = (bool)reader["IsActive"],
                            sessionId = reader["sessionId"] == DBNull.Value ? (int?)null : (int)reader["sessionId"]
                        });
                    }
                }
            }
            return subTopics;
        }

        public async Task<SubTopics> GetSubTopicByIdAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT * FROM SubTopics WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new SubTopics
                        {
                            id = (int)reader["id"],
                            SubTopicName = reader["SubTopicName"].ToString(),
                            TopicId = (int)reader["TopicId"],
                            OrderIndex = (int)reader["OrderIndex"],
                            DifficultyLevel = (int)reader["DifficultyLevel"],
                            EstimatedCompletionTime = (int)reader["EstimatedCompletionTime"],
                            IsActive = (bool)reader["IsActive"],
                            sessionId = reader["sessionId"] == DBNull.Value ? (int?)null : (int)reader["sessionId"]
                        };
                    }
                }
            }
            return null;
        }

        public async Task<IEnumerable<SubTopics>> GetSubTopicByTopicId(int topicId)
        {
            {
                var subTopics = new List<SubTopics>();

                using (var conn = new SqlConnection(_connectionString))
                {
                    var cmd = new SqlCommand("SELECT * FROM SubTopics where topicId=@topicId", conn);
                    cmd.Parameters.AddWithValue("@topicId", topicId);

                    conn.Open();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            subTopics.Add(new SubTopics
                            {
                                id = (int)reader["id"],
                                SubTopicName = reader["SubTopicName"].ToString(),
                                TopicId = (int)reader["TopicId"],
                                OrderIndex = (int)reader["OrderIndex"],
                                DifficultyLevel = (int)reader["DifficultyLevel"],
                                EstimatedCompletionTime = (int)reader["EstimatedCompletionTime"],
                                IsActive = (bool)reader["IsActive"],
                                sessionId = reader["sessionId"] == DBNull.Value ? (int?)null : (int)reader["sessionId"]
                            });
                        }
                    }
                }
                return subTopics;
            }
        }
        public async Task<bool> DeleteSubTopicAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("DELETE FROM SubTopics WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> UpdateSubTopicByIdAsync(int id, SubTopics updateDto)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var updates = new List<string>();
                var cmd = new SqlCommand();

                if (!string.IsNullOrEmpty(updateDto.SubTopicName))
                {
                    updates.Add("SubTopicName = @SubTopicName");
                    cmd.Parameters.AddWithValue("@SubTopicName", updateDto.SubTopicName);
                }

                if (updateDto.EstimatedCompletionTime.HasValue)
                {
                    updates.Add("EstimatedCompletionTime = @estimatedCompletionTime");
                    cmd.Parameters.AddWithValue("@estimatedCompletionTime", updateDto.EstimatedCompletionTime);
                }

                if (updateDto.DifficultyLevel.HasValue)
                {
                    updates.Add("DifficultyLevel = @difficultyLevel");
                    cmd.Parameters.AddWithValue("@difficultyLevel", updateDto.DifficultyLevel);
                }

                if (updateDto.OrderIndex.HasValue)
                {
                    updates.Add("OrderIndex = @orderIndex");
                    cmd.Parameters.AddWithValue("@orderIndex", updateDto.OrderIndex);
                }

                if (updateDto.TopicId > 0)
                {
                    updates.Add("TopicId = @TopicId");
                    cmd.Parameters.AddWithValue("@TopicId", updateDto.TopicId);
                }

                if (updateDto.IsActive.HasValue)
                {
                    updates.Add("IsActive = @isActive");
                    cmd.Parameters.AddWithValue("@isActive", updateDto.IsActive);
                }

                // If no properties to update, return false
                if (!updates.Any())
                {
                    throw new ArgumentException("No valid fields provided for update.");
                }

                // Build dynamic query
                cmd.CommandText = $"UPDATE SubTopics SET {string.Join(", ", updates)} WHERE id = @id";
                cmd.Parameters.AddWithValue("@id", id);

                cmd.Connection = conn;
                conn.Open();

                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

    }
}
