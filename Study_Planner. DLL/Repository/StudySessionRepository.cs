using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Study_Planner._DLL.IRepository;
using Study_Planner.Core.DTOs;
using Microsoft.Extensions.Configuration;
using Study_Planner.Core.DTOs;
using Study_Planner._DLL.IRepository;

namespace Study_Planner._DLL.Repository
{
    public class StudySessionRepository : IStudySessionRepository
    {
        private readonly string _connectionString;

        public StudySessionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // CREATE
        public async Task<StudySessionDto> CreateStudySessionAsync(StudySessionDto studySessionDto)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = @"
                INSERT INTO StudySessions 
                (UserId, SubjectId, TopicId, SubTopicId, Notes, ScheduledStartTime, ScheduledEndTime, 
                 ActualStartTime, ActualEndTime, Status, FocusRating, ComprehensionRating, CreatedAt, UpdatedAt) 
                OUTPUT INSERTED.Id 
                VALUES 
                (@UserId, @SubjectId, @TopicId, @SubTopicId, @Notes, @ScheduledStartTime, @ScheduledEndTime, 
                 @ActualStartTime, @ActualEndTime, @Status, @FocusRating, @ComprehensionRating, GETDATE(), GETDATE())";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", studySessionDto.UserId);
            cmd.Parameters.AddWithValue("@SubjectId", studySessionDto.SubjectId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TopicId", studySessionDto.TopicId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SubTopicId", studySessionDto.SubTopicId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", studySessionDto.Notes);
            cmd.Parameters.AddWithValue("@ScheduledStartTime", studySessionDto.ScheduledStartTime);
            cmd.Parameters.AddWithValue("@ScheduledEndTime", studySessionDto.ScheduledEndTime);
            cmd.Parameters.AddWithValue("@ActualStartTime", studySessionDto.ActualStartTime ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ActualEndTime", studySessionDto.ActualEndTime ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", studySessionDto.Status);
            cmd.Parameters.AddWithValue("@FocusRating", studySessionDto.FocusRating ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ComprehensionRating", studySessionDto.ComprehensionRating ?? (object)DBNull.Value);

            studySessionDto.Id = (int)await cmd.ExecuteScalarAsync();
            return studySessionDto;
        }

        // READ (GET ALL)
        public async Task<IEnumerable<StudySessionDto>> GetAllStudySessionsAsync()
        {
            var studySessions = new List<StudySessionDto>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = "SELECT * FROM StudySessions";
            using var cmd = new SqlCommand(query, conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                studySessions.Add(MapToStudySessionDto(reader));
            }

            return studySessions;
        }

        // READ (GET BY ID)
        public async Task<StudySessionDto> GetStudySessionByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = "SELECT * FROM StudySessions WHERE Id = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToStudySessionDto(reader);
            }

            return null;
        }

        public async Task<StudySessionDto> UpdateStudySessionAsync(int id, StudySessionDto studySessionDto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = @"UPDATE StudySessions 
                                 SET UserId = @UserId, SubjectId = @SubjectId, TopicId = @TopicId, 
                                     SubTopicId = @SubTopicId, Notes = @Notes, 
                                     ScheduledStartTime = @ScheduledStartTime, ScheduledEndTime = @ScheduledEndTime, 
                                     ActualStartTime = @ActualStartTime, ActualEndTime = @ActualEndTime, 
                                     Status = @Status, FocusRating = @FocusRating, 
                                     ComprehensionRating = @ComprehensionRating, UpdatedAt = GETDATE() 
                                 WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@UserId", studySessionDto.UserId);
                    cmd.Parameters.AddWithValue("@SubjectId", studySessionDto.SubjectId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TopicId", studySessionDto.TopicId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SubTopicId", studySessionDto.SubTopicId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Notes", studySessionDto.Notes);
                    cmd.Parameters.AddWithValue("@ScheduledStartTime", studySessionDto.ScheduledStartTime);
                    cmd.Parameters.AddWithValue("@ScheduledEndTime", studySessionDto.ScheduledEndTime);
                    cmd.Parameters.AddWithValue("@ActualStartTime", (object)studySessionDto.ActualStartTime ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ActualEndTime", (object)studySessionDto.ActualEndTime ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", studySessionDto.Status);
                    cmd.Parameters.AddWithValue("@FocusRating", (object)studySessionDto.FocusRating ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ComprehensionRating", (object)studySessionDto.ComprehensionRating ?? DBNull.Value);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0 ? studySessionDto : null;
                }
            }
        }
        // DELETE
        public async Task<bool> DeleteStudySessionAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = "DELETE FROM StudySessions WHERE Id = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        // Helper Method to Map Reader to DTO
        private StudySessionDto MapToStudySessionDto(SqlDataReader reader)
        {
            return new StudySessionDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                SubjectId = reader.IsDBNull(reader.GetOrdinal("SubjectId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("SubjectId")),
                TopicId = reader.IsDBNull(reader.GetOrdinal("TopicId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("TopicId")),
                SubTopicId = reader.IsDBNull(reader.GetOrdinal("SubTopicId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("SubTopicId")),
                Notes = reader.GetString(reader.GetOrdinal("Notes")),
                ScheduledStartTime = reader.GetDateTime(reader.GetOrdinal("ScheduledStartTime")),
                ScheduledEndTime = reader.GetDateTime(reader.GetOrdinal("ScheduledEndTime")),
                ActualStartTime = reader.IsDBNull(reader.GetOrdinal("ActualStartTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ActualStartTime")),
                ActualEndTime = reader.IsDBNull(reader.GetOrdinal("ActualEndTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ActualEndTime")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                FocusRating = reader.IsDBNull(reader.GetOrdinal("FocusRating")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("FocusRating")),
                ComprehensionRating = reader.IsDBNull(reader.GetOrdinal("ComprehensionRating")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ComprehensionRating"))
            };
        }
    }
}
