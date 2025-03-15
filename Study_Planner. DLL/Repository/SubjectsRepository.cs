using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Study_Planner._DLL.IRepository;
using Study_Planner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Study_Planner._DLL.Repository
{
    public class SubjectsRepository : ISubjectsRepository
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITopicRepository _topicRepository;

        public SubjectsRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ITopicRepository topicRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _httpContextAccessor = httpContextAccessor;
            _topicRepository = topicRepository;
        }

        public async Task<int> AddSubjectWithDetailsAsync(Subjects subjectDto, string userId="3")
        {

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authorized.");
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int subjectId;
                        using (SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO Subjects (UserId, SubjectName, DifficultyLevel, Priority, 
                            EstimatedCompletionTime, Status, StartDate, EndDate, CreatedAt, UpdatedAt)
                            VALUES (@UserId, @SubjectName, @DifficultyLevel, @Priority, 
                            @EstimatedCompletionTime, @Status, @StartDate, @EndDate, GETDATE(), GETDATE());
                            SELECT SCOPE_IDENTITY();", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.Parameters.AddWithValue("@SubjectName", subjectDto.SubjectName);
                            cmd.Parameters.AddWithValue("@DifficultyLevel", subjectDto.DifficultyLevel);
                            cmd.Parameters.AddWithValue("@Priority", subjectDto.Priority);
                            cmd.Parameters.AddWithValue("@EstimatedCompletionTime", subjectDto.EstimatedCompletionTime ?? 60);
                            cmd.Parameters.AddWithValue("@Status", subjectDto.Status ?? "ToDo");
                            cmd.Parameters.AddWithValue("@StartDate", (object?)subjectDto.StartDate ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@EndDate", (object?)subjectDto.EndDate ?? DBNull.Value);

                            subjectId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        }
                        if (subjectDto.Topic != null)
                        {
                            int topicId;
                            using (SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO Topics (TopicName, SubjectId, OrderIndex, DifficultyLevel, EstimatedCompletionTime, CreatedAt, UpdatedAt)
                            VALUES (@TopicName, @SubjectId, @OrderIndex, 
                            ISNULL(@DifficultyLevel, 5), 
                            ISNULL(@EstimatedCompletionTime, 60), GETDATE(), GETDATE());
                            SELECT SCOPE_IDENTITY();", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@TopicName", subjectDto.Topic.TopicName);
                                cmd.Parameters.AddWithValue("@SubjectId", subjectId);
                                cmd.Parameters.AddWithValue("@OrderIndex", subjectDto.Topic.OrderIndex);
                                cmd.Parameters.AddWithValue("@DifficultyLevel", subjectDto.Topic.DifficultyLevel ?? 5);
                                cmd.Parameters.AddWithValue("@EstimatedCompletionTime", subjectDto.Topic.EstimatedCompletionTime ?? 60);

                                topicId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                            }

                            if (subjectDto.Topic.SubTopics != null)
                            {
                                foreach (var subtopic in subjectDto.Topic.SubTopics)
                                {
                                    using (SqlCommand cmd = new SqlCommand(@"
                                    INSERT INTO SubTopics (SubTopicName, TopicId, OrderIndex, DifficultyLevel, EstimatedCompletionTime, IsActive, CreatedAt, UpdatedAt)
                                    VALUES (@SubTopicName, @TopicId, @OrderIndex, 
                                    ISNULL(@DifficultyLevel, 5), 
                                    ISNULL(@EstimatedCompletionTime, 60), 
                                    @IsActive, GETDATE(), GETDATE());", conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@SubTopicName", subtopic.SubTopicName);
                                        cmd.Parameters.AddWithValue("@TopicId", topicId);
                                        cmd.Parameters.AddWithValue("@OrderIndex", subtopic.OrderIndex);
                                        cmd.Parameters.AddWithValue("@DifficultyLevel", subtopic.DifficultyLevel ?? 5);
                                        cmd.Parameters.AddWithValue("@EstimatedCompletionTime", subtopic.EstimatedCompletionTime ?? 60);
                                        cmd.Parameters.AddWithValue("@IsActive", subtopic.IsActive);

                                        await cmd.ExecuteNonQueryAsync();
                                    }
                                }
                            }
                        }

                        await transaction.CommitAsync();
                        return subjectId;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }


        public async Task<IEnumerable<Subjects>> GetAllSubjectsAsync()
        {
            var subjects = new List<Subjects>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT * FROM Subjects", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            subjects.Add(new Subjects
                            {
                                id = reader.GetInt32("id"),
                                SubjectName = reader.GetString("SubjectName"),
                                DifficultyLevel = reader.GetInt32("DifficultyLevel"),
                                Priority = reader.GetInt32("Priority"),
                                EstimatedCompletionTime = reader["EstimatedCompletionTime"] as int?,
                                Status = reader.GetString("Status"),
                                StartDate = reader["StartDate"] as DateTime?, 
                                EndDate = reader["EndDate"] as DateTime?
                            });
                        }
                    }
                }
            }

            return subjects;
        }

        public async Task<Subjects?> GetSubjectByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT * FROM Subjects WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var subjects =  new Subjects
                            {
                                id = reader.GetInt32("id"),
                                SubjectName = reader.GetString("SubjectName"),
                                DifficultyLevel = reader.GetInt32("DifficultyLevel"),
                                Priority = reader.GetInt32("Priority"),
                                EstimatedCompletionTime = reader["EstimatedCompletionTime"] as int?,
                                Status = reader.GetString("Status"),
                                StartDate = reader["StartDate"] as DateTime?,
                                EndDate = reader["EndDate"] as DateTime?
                            };

                            return subjects;
                        }
                    }
                }
            }

            return null;
        }

        public async Task<bool> UpdateSubjectAsync(int id, UpdateSubjects subjectDto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var updateQuery = new List<string>();
                var command = new SqlCommand();

                if (!string.IsNullOrEmpty(subjectDto.SubjectName))
                {
                    updateQuery.Add("SubjectName = @SubjectName");
                    command.Parameters.AddWithValue("@SubjectName", subjectDto.SubjectName);
                }

                if (subjectDto.DifficultyLevel.HasValue && subjectDto.DifficultyLevel > 0)
                {
                    updateQuery.Add("DifficultyLevel = @DifficultyLevel");
                    command.Parameters.AddWithValue("@DifficultyLevel", subjectDto.DifficultyLevel);
                }

                if (subjectDto.Priority.HasValue && subjectDto.Priority > 0)
                {
                    updateQuery.Add("Priority = @Priority");
                    command.Parameters.AddWithValue("@Priority", subjectDto.Priority);
                }

                if (subjectDto.EstimatedCompletionTime.HasValue && subjectDto.EstimatedCompletionTime > 0)
                {
                    updateQuery.Add("EstimatedCompletionTime = @EstimatedCompletionTime");
                    command.Parameters.AddWithValue("@EstimatedCompletionTime", subjectDto.EstimatedCompletionTime);
                }

                if (!string.IsNullOrEmpty(subjectDto.Status))
                {
                    updateQuery.Add("Status = @Status");
                    command.Parameters.AddWithValue("@Status", subjectDto.Status);
                }

                if (subjectDto.StartDate.HasValue)
                {
                    updateQuery.Add("StartDate = @StartDate");
                    command.Parameters.AddWithValue("@StartDate", subjectDto.StartDate);
                }

                if (subjectDto.EndDate.HasValue)
                {
                    updateQuery.Add("EndDate = @EndDate");
                    command.Parameters.AddWithValue("@EndDate", subjectDto.EndDate);
                }

                if (!updateQuery.Any())
                {
                    // No valid data provided for update
                    return false;
                }

                command.CommandText = $@"
            UPDATE Subjects
            SET {string.Join(", ", updateQuery)}
            WHERE id = @id";

                command.Parameters.AddWithValue("@id", id);
                command.Connection = connection;

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }


        public async Task<bool> DeleteSubjectAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("DELETE FROM Subjects WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}