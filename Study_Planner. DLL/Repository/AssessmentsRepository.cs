using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Study_Planner.Core.DTOs;
using Study_Planner._DLL.IRepository;
using System;
using System.Collections.Generic;

namespace Study_Planner._DLL.Repository
{
    public class AssessmentsRepository : IAssessmentsRepository
    {
        private readonly string _connectionString;

        public AssessmentsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Fetch All Assessments
        public IEnumerable<AssessmentDTO> GetAllAssessments()
        {
            var assessments = new List<AssessmentDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Assessments", connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        assessments.Add(MapReaderToAssessment(reader));
                    }
                }
            }
            return assessments;
        }

        // Fetch Assessment by ID
        public AssessmentDTO GetAssessmentById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Assessments WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapReaderToAssessment(reader);
                    }
                }
            }
            return null;
        }

        // Create New Assessment
        public int CreateAssessment(CreateAssessmentDTO assessment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
                    INSERT INTO Assessments (UserId, SubjectId, TopicId, AssessmentName, Description, 
                    AssessmentType, MaxScore, DueDate, IsCompleted, CreatedAt, UpdatedAt) 
                    VALUES (@UserId, @SubjectId, @TopicId, @AssessmentName, @Description, @AssessmentType, 
                    @MaxScore, @DueDate, 0, GETDATE(), GETDATE());
                    SELECT SCOPE_IDENTITY();", connection);

                command.Parameters.AddWithValue("@UserId", assessment.UserId);
                command.Parameters.AddWithValue("@SubjectId", assessment.SubjectId);
                command.Parameters.AddWithValue("@TopicId", (object?)assessment.TopicId ?? DBNull.Value);
                command.Parameters.AddWithValue("@AssessmentName", assessment.AssessmentName);
                command.Parameters.AddWithValue("@Description", (object?)assessment.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@AssessmentType", assessment.AssessmentType);
                command.Parameters.AddWithValue("@MaxScore", assessment.MaxScore);
                command.Parameters.AddWithValue("@DueDate", (object?)assessment.DueDate ?? DBNull.Value);

                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        // Update Assessment (Partial Updates)
        public bool UpdateAssessment(int id, UpdateAssessmentDTO assessment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var updateFields = new List<string>();
                var parameters = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(assessment.AssessmentName))
                {
                    updateFields.Add("AssessmentName = @AssessmentName");
                    parameters["@AssessmentName"] = assessment.AssessmentName;
                }

                if (!string.IsNullOrEmpty(assessment.Description))
                {
                    updateFields.Add("Description = @Description");
                    parameters["@Description"] = assessment.Description;
                }

                if (!string.IsNullOrEmpty(assessment.AssessmentType))
                {
                    updateFields.Add("AssessmentType = @AssessmentType");
                    parameters["@AssessmentType"] = assessment.AssessmentType;
                }

                if (assessment.MaxScore.HasValue)
                {
                    updateFields.Add("MaxScore = @MaxScore");
                    parameters["@MaxScore"] = assessment.MaxScore.Value;
                }

                if (assessment.ActualScore.HasValue)
                {
                    updateFields.Add("ActualScore = @ActualScore");
                    parameters["@ActualScore"] = assessment.ActualScore.Value;
                }

                if (assessment.CompletionDate.HasValue)
                {
                    updateFields.Add("CompletionDate = @CompletionDate");
                    parameters["@CompletionDate"] = assessment.CompletionDate.Value;
                }

                if (assessment.DueDate.HasValue)
                {
                    updateFields.Add("DueDate = @DueDate");
                    parameters["@DueDate"] = assessment.DueDate.Value;
                }

                if (assessment.IsCompleted.HasValue)
                {
                    updateFields.Add("IsCompleted = @IsCompleted");
                    parameters["@IsCompleted"] = assessment.IsCompleted.Value;
                }

                if (updateFields.Count == 0)
                {
                    return false; // No updates
                }

                var query = $"UPDATE Assessments SET {string.Join(", ", updateFields)}, UpdatedAt = GETDATE() WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // Delete Assessment
        public bool DeleteAssessment(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("DELETE FROM Assessments WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        // Map Database Result to DTO
        private AssessmentDTO MapReaderToAssessment(SqlDataReader reader)
        {
            return new AssessmentDTO
            {
                Id = Convert.ToInt32(reader["Id"]),
                UserId = Convert.ToInt32(reader["UserId"]),
                SubjectId = Convert.ToInt32(reader["SubjectId"]),
                TopicId = reader["TopicId"] != DBNull.Value ? Convert.ToInt32(reader["TopicId"]) : (int?)null,
                AssessmentName = reader["AssessmentName"].ToString(),
                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                AssessmentType = reader["AssessmentType"].ToString(),
                MaxScore = Convert.ToDecimal(reader["MaxScore"]),
                ActualScore = reader["ActualScore"] != DBNull.Value ? Convert.ToDecimal(reader["ActualScore"]) : (decimal?)null,
                CompletionDate = reader["CompletionDate"] != DBNull.Value ? Convert.ToDateTime(reader["CompletionDate"]) : (DateTime?)null,
                DueDate = reader["DueDate"] != DBNull.Value ? Convert.ToDateTime(reader["DueDate"]) : (DateTime?)null,
                IsCompleted = Convert.ToBoolean(reader["IsCompleted"])
            };
        }
    }
}
