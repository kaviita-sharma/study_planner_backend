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
    public class TopicRepository : ITopicRepository
    {
        private readonly string _connectionString;

        public TopicRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> SubjectExistsAsync(int subjectId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var cmd = new SqlCommand(
                    "SELECT COUNT(1) FROM Subjects WHERE Id = @subjectId",
                    conn);

                cmd.Parameters.AddWithValue("@subjectId", subjectId);

                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            }
        }

        public async Task<bool> TopicExistsAsync(string topicName, int subjectId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var cmd = new SqlCommand(
                    "SELECT COUNT(1) FROM Topics WHERE TopicName = @topicName AND SubjectId = @subjectId",
                    conn);

                cmd.Parameters.AddWithValue("@topicName", topicName);
                cmd.Parameters.AddWithValue("@subjectId", subjectId);

                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            }
        }

        public async Task<bool> SubTopicExistsAsync(string subTopicName, int topicId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var cmd = new SqlCommand(
                    "SELECT COUNT(1) FROM SubTopics WHERE SubTopicName = @subTopicName AND TopicId = @topicId",
                    conn);

                cmd.Parameters.AddWithValue("@subTopicName", subTopicName);
                cmd.Parameters.AddWithValue("@topicId", topicId);

                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            }
        }

        public async Task<IEnumerable<Topics>> GetAllTopicsAsync()
        {
            var topics = new List<Topics>();

            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT * FROM Topics", conn);
                conn.Open();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        topics.Add(new Topics
                        {
                            TopicId = (int)reader["id"],
                            TopicName = reader["TopicName"].ToString(),
                            SubjectId = (int)reader["SubjectId"],
                            OrderIndex = (int)reader["OrderIndex"],
                            DifficultyLevel = (int)reader["DifficultyLevel"],
                            EstimatedCompletionTime = (int)reader["EstimatedCompletionTime"]
                        });
                    }
                }
            }
            return topics;
        }

        public async Task<Topics> GetTopicByIdAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT * FROM Topics WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Topics
                        {
                            TopicId = (int)reader["id"],
                            TopicName = reader["TopicName"].ToString(),
                            SubjectId = (int)reader["SubjectId"],
                            EstimatedCompletionTime = (int)reader["EstimatedCompletionTime"]
                        };
                    }
                }
            }
            return null;
        }

        public async Task<int> AddTopicAsync(Topics topicDto)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction()) // Begin Transaction
                {
                    try
                    {
                        // Insert Topic
                        var cmd = new SqlCommand(
                            @"INSERT INTO Topics (TopicName, SubjectId, OrderIndex, DifficultyLevel, EstimatedCompletionTime, IsActive)
                      VALUES (@topicName, @subjectId, @orderIndex, @difficultyLevel, @estimatedCompletionTime, @isActive);
                      SELECT SCOPE_IDENTITY();",
                            conn, transaction);

                        cmd.Parameters.AddWithValue("@topicName", topicDto.TopicName);
                        cmd.Parameters.AddWithValue("@subjectId", topicDto.SubjectId);
                        cmd.Parameters.AddWithValue("@orderIndex", topicDto.OrderIndex);
                        cmd.Parameters.AddWithValue("@difficultyLevel", topicDto.DifficultyLevel);
                        cmd.Parameters.AddWithValue("@estimatedCompletionTime", topicDto.EstimatedCompletionTime);
                        cmd.Parameters.AddWithValue("@isActive", topicDto.IsActive);

                        var topicId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                        // Insert SubTopics (if available)
                        if (topicDto.SubTopics != null && topicDto.SubTopics.Any())
                        {
                            foreach (var subTopic in topicDto.SubTopics)
                            {
                                var subTopicCmd = new SqlCommand(
                                    @"INSERT INTO SubTopics (TopicId, SubTopicName, DifficultyLevel, 
                              EstimatedCompletionTime, OrderIndex, IsActive, CreatedAt, UpdatedAt)
                              VALUES (@topicId, @subTopicName, @difficultyLevel, @estimatedCompletionTime,
                              @orderIndex, @isActive, GETDATE(), GETDATE());",
                                    conn, transaction);

                                subTopicCmd.Parameters.AddWithValue("@topicId", topicId);
                                subTopicCmd.Parameters.AddWithValue("@subTopicName", subTopic.SubTopicName);
                                subTopicCmd.Parameters.AddWithValue("@difficultyLevel", subTopic.DifficultyLevel);
                                subTopicCmd.Parameters.AddWithValue("@estimatedCompletionTime", subTopic.EstimatedCompletionTime);
                                subTopicCmd.Parameters.AddWithValue("@orderIndex", subTopic.OrderIndex);
                                subTopicCmd.Parameters.AddWithValue("@isActive", subTopic.IsActive);

                                await subTopicCmd.ExecuteNonQueryAsync();
                            }
                        }

                        await transaction.CommitAsync(); // Commit Transaction
                        return topicId;
                    }
                    catch
                    {
                        await transaction.RollbackAsync(); // Rollback if any failure
                        throw;
                    }
                }
            }
        }


        public async Task<bool> UpdateTopicAsync(int id, Topics topicDto)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var updates = new List<string>();
                var cmd = new SqlCommand();

                if (!string.IsNullOrEmpty(topicDto.TopicName))
                {
                    updates.Add("TopicName = @topicName");
                    cmd.Parameters.AddWithValue("@topicName", topicDto.TopicName);
                }

                if (topicDto.EstimatedCompletionTime.HasValue)
                {
                    updates.Add("EstimatedCompletionTime = @estimatedCompletionTime");
                    cmd.Parameters.AddWithValue("@estimatedCompletionTime", topicDto.EstimatedCompletionTime);
                }

                if (topicDto.DifficultyLevel.HasValue)
                {
                    updates.Add("DifficultyLevel = @difficultyLevel");
                    cmd.Parameters.AddWithValue("@difficultyLevel", topicDto.DifficultyLevel);
                }

                if (topicDto.OrderIndex.HasValue)
                {
                    updates.Add("OrderIndex = @orderIndex");
                    cmd.Parameters.AddWithValue("@orderIndex", topicDto.OrderIndex);
                }

                if (topicDto.SubjectId>0)
                {
                    updates.Add("SubjectId = @subjectId");
                    cmd.Parameters.AddWithValue("@subjectId", topicDto.SubjectId);
                }

                if (topicDto.IsActive.HasValue)
                {
                    updates.Add("IsActive = @isActive");
                    cmd.Parameters.AddWithValue("@isActive", topicDto.IsActive);
                }

                // If no properties to update, return false
                if (!updates.Any())
                {
                    throw new ArgumentException("No valid fields provided for update.");
                }

                // Build dynamic query
                cmd.CommandText = $"UPDATE Topics SET {string.Join(", ", updates)} WHERE id = @id";
                cmd.Parameters.AddWithValue("@id", id);

                cmd.Connection = conn;
                conn.Open();

                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }


        public async Task<bool> DeleteTopicAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("DELETE FROM Topics WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }
    }
}
