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

        public async Task<int> AddSubTopicAsync(SubTopics subTopicDto)
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
                        cmd.Parameters.AddWithValue("@subjectId", subTopicDto.TopicId);
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
                            SubTopicName = reader["TopicName"].ToString(),
                            TopicId = (int)reader["TopicId"],
                            OrderIndex = (int)reader["OrderIndex"],
                            DifficultyLevel = (int)reader["DifficultyLevel"],
                            EstimatedCompletionTime = (int)reader["EstimatedCompletionTime"]
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
                var cmd = new SqlCommand("SELECT * FROM Topics WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new SubTopics
                        {
                            id = (int)reader["id"],
                            SubTopicName = reader["TopicName"].ToString(),
                            TopicId = (int)reader["TopicId"],
                            EstimatedCompletionTime = (int)reader["EstimatedCompletionTime"]
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
                                SubTopicName = reader["TopicName"].ToString(),
                                TopicId = (int)reader["TopicId"],
                                OrderIndex = (int)reader["OrderIndex"],
                                DifficultyLevel = (int)reader["DifficultyLevel"],
                                EstimatedCompletionTime = (int)reader["EstimatedCompletionTime"]
                            });
                        }
                    }
                }
                return subTopics;
            }
        }
    }
}
