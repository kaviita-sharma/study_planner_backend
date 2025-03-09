using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Study_Planner._DLL.IRepository;
using Study_Planner.Core.DTOs.Study_Planner.Core.DTOs;

namespace Study_Planner._DLL.Repository
{
    public class ProgressRepository : IProgressRepository
    {
        private readonly string _connectionString;

        public ProgressRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<ProgressDTO> GetAllProgress()
        {
            var progressList = new List<ProgressDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Progress", connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        progressList.Add(MapReaderToProgress(reader));
                    }
                }
            }

            return progressList;
        }

        public ProgressDTO GetProgressById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Progress WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapReaderToProgress(reader);
                    }
                }
            }
            return null;
        }

        public int CreateProgress(ProgressDTO progress)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
                    INSERT INTO Progress (UserId, SubjectId, TopicId, SubTopicId, CompletionPercentage, 
                    LastStudyDate, NextReviewDate, ConfidenceLevel, CreatedAt, UpdatedAt) 
                    VALUES (@UserId, @SubjectId, @TopicId, @SubTopicId, @CompletionPercentage, 
                    @LastStudyDate, @NextReviewDate, @ConfidenceLevel, GETDATE(), GETDATE());
                    SELECT SCOPE_IDENTITY();", connection);

                AddParameters(command, progress);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public bool UpdateProgress(int id, ProgressDTO progress)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
                    UPDATE Progress 
                    SET UserId = @UserId, SubjectId = @SubjectId, TopicId = @TopicId, SubTopicId = @SubTopicId, 
                    CompletionPercentage = @CompletionPercentage, LastStudyDate = @LastStudyDate, 
                    NextReviewDate = @NextReviewDate, ConfidenceLevel = @ConfidenceLevel, UpdatedAt = GETDATE()
                    WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", id);
                AddParameters(command, progress);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteProgress(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("DELETE FROM Progress WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        private void AddParameters(SqlCommand command, ProgressDTO progress)
        {
            command.Parameters.AddWithValue("@UserId", progress.UserId);
            command.Parameters.AddWithValue("@SubjectId", progress.SubjectId);
            command.Parameters.AddWithValue("@TopicId", (object?)progress.TopicId ?? DBNull.Value);
            command.Parameters.AddWithValue("@SubTopicId", (object?)progress.SubTopicId ?? DBNull.Value);
            command.Parameters.AddWithValue("@CompletionPercentage", progress.CompletionPercentage);
            command.Parameters.AddWithValue("@LastStudyDate", (object?)progress.LastStudyDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@NextReviewDate", (object?)progress.NextReviewDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@ConfidenceLevel", (object?)progress.ConfidenceLevel ?? DBNull.Value);
        }

        private ProgressDTO MapReaderToProgress(SqlDataReader reader)
        {
            return new ProgressDTO
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                SubjectId = reader.GetInt32(reader.GetOrdinal("SubjectId")),
                TopicId = reader.IsDBNull(reader.GetOrdinal("TopicId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("TopicId")),
                SubTopicId = reader.IsDBNull(reader.GetOrdinal("SubTopicId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("SubTopicId")),
                CompletionPercentage = reader.GetDecimal(reader.GetOrdinal("CompletionPercentage")),
                LastStudyDate = reader.IsDBNull(reader.GetOrdinal("LastStudyDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LastStudyDate")),
                NextReviewDate = reader.IsDBNull(reader.GetOrdinal("NextReviewDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("NextReviewDate")),
                ConfidenceLevel = reader.IsDBNull(reader.GetOrdinal("ConfidenceLevel")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ConfidenceLevel")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };
        }
    }

}
