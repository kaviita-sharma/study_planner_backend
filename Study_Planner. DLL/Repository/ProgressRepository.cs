using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Study_Planner._DLL.IRepository;
using Study_Planner.Core.DTOs;
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
        public IEnumerable<EnrichedProgressDTO> GetAllEnrichedProgress()
        {
            var enrichedProgressList = new List<EnrichedProgressDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
            SELECT p.*, 
                   s.SubjectName, 
                   t.TopicName, 
                   st.SubTopicName
            FROM Progress p
            LEFT JOIN Subjects s ON p.SubjectId = s.Id
            LEFT JOIN Topics t ON p.TopicId = t.Id
            LEFT JOIN SubTopics st ON p.SubTopicId = st.Id";

                var command = new SqlCommand(query, connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        enrichedProgressList.Add(MapReaderToEnrichedProgress(reader));
                    }
                }
            }

            return enrichedProgressList;
        }

        private EnrichedProgressDTO MapReaderToEnrichedProgress(SqlDataReader reader)
        {
            return new EnrichedProgressDTO
            {
                Id = (int)reader["Id"],
                UserId = (int)reader["UserId"],
                SubjectId = (int)reader["SubjectId"],
                SubjectName = reader["SubjectName"]?.ToString(),
                TopicId = reader["TopicId"] as int?,
                TopicName = reader["TopicName"]?.ToString(),
                SubTopicId = reader["SubTopicId"] as int?,
                SubTopicName = reader["SubTopicName"]?.ToString(),
                CompletionPercentage = (decimal)reader["CompletionPercentage"],
                LastStudyDate = reader["LastStudyDate"] as DateTime?,
                NextReviewDate = reader["NextReviewDate"] as DateTime?,
                ConfidenceLevel = reader["ConfidenceLevel"] as int?,
                CreatedAt = (DateTime)reader["CreatedAt"],
                UpdatedAt = (DateTime)reader["UpdatedAt"]
            };
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

        public List<ProgressDTO> GetProgressByUserId(int userId)
        {
            var progressList = new List<ProgressDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Progress WHERE UserId = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read()) // Use `while` instead of `if` to read multiple rows
                    {
                        progressList.Add(MapReaderToProgress(reader));
                    }
                }
            }

            return progressList;
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
            ", connection);

                AddParameters(command, progress);
                connection.Open();
                command.ExecuteNonQuery();
            }

            // Return the UserId since Progress is linked to the user
            return progress.UserId;
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
        public UserPreferenceDTO GetUserPreferences(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("SELECT LearningStyle, PreferredStudyTime, StudySessionDuration, BreakDuration FROM UserPreferences WHERE UserId = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserPreferenceDTO
                            {
                                LearningStyle = reader["LearningStyle"].ToString(),
                                PreferredStudyTime = reader["PreferredStudyTime"].ToString(),
                                StudySessionDuration = Convert.ToInt32(reader["StudySessionDuration"]),
                                BreakDuration = Convert.ToInt32(reader["BreakDuration"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<TimeSlotDTO> GetUserTimeSlots(int userId)
        {
            var timeSlots = new List<TimeSlotDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("SELECT Day, StartTime, EndTime FROM StudySessions WHERE UserId = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            timeSlots.Add(new TimeSlotDTO
                            {
                                Day = reader["Day"].ToString(),
                                StartTime = reader["StartTime"].ToString(),
                                EndTime = reader["EndTime"].ToString()
                            });
                        }
                    }
                }
            }

            return timeSlots;
        }

        public List<ProgressWithDetailsDTO> GetUserProgressWithDetails(int userId)
        {
            var progressList = new List<ProgressWithDetailsDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT 
                        p.SubjectId,
                        s.SubjectName,
                        s.Priority,
                        s.DifficultyLevel,
                        s.EstimatedCompletionTime
                    FROM Progress p
                    INNER JOIN Subjects s ON p.SubjectId = s.id
                    WHERE p.UserId = @UserId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            progressList.Add(new ProgressWithDetailsDTO
                            {
                                SubjectId = Convert.ToInt32(reader["SubjectId"]),
                                SubjectName = reader["SubjectName"].ToString(),
                                Priority = Convert.ToInt32(reader["Priority"]),
                                DifficultyLevel = Convert.ToInt32(reader["DifficultyLevel"]),
                                EstimatedCompletionTime = reader["EstimatedCompletionTime"] as int?
                            });
                        }
                    }
                }
            }

            return progressList;
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
