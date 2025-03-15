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
        public async Task<StudySessionDto> CreateStudySessionAsync(int userId, StudySessionDto studySessionDto, int? subTopicId = null)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            // Insert the study session and retrieve its new Id
            string insertQuery = @"
         INSERT INTO StudySessions 
         (UserId, Notes, ScheduledStartTime, ScheduledEndTime, 
          ActualStartTime, ActualEndTime, Status, FocusRating, ComprehensionRating, CreatedAt, UpdatedAt,title) 
         OUTPUT INSERTED.Id 
         VALUES 
         (@UserId, @Notes, @ScheduledStartTime, @ScheduledEndTime, 
          @ActualStartTime, @ActualEndTime, @Status, @FocusRating, @ComprehensionRating, GETDATE(), GETDATE(),@title)";

            using var insertCmd = new SqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@UserId", userId);
            insertCmd.Parameters.AddWithValue("@Notes", studySessionDto.Notes ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@ScheduledStartTime", studySessionDto.ScheduledStartTime);
            insertCmd.Parameters.AddWithValue("@ScheduledEndTime", studySessionDto.ScheduledEndTime);
            insertCmd.Parameters.AddWithValue("@ActualStartTime", studySessionDto.ActualStartTime ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@ActualEndTime", studySessionDto.ActualEndTime ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@Status", studySessionDto.status);
            insertCmd.Parameters.AddWithValue("@FocusRating", studySessionDto.FocusRating ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@ComprehensionRating", studySessionDto.ComprehensionRating ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@title",studySessionDto.title ?? (object)DBNull.Value);

            // Retrieve and assign the inserted session's Id
            studySessionDto.Id = (int)await insertCmd.ExecuteScalarAsync();

            // Update the subtopics table if a subTopicId is provided.
            // If no subtopic is provided, the session remains unlinked.
            if (subTopicId.HasValue)
            {
                string updateQuery = "UPDATE subtopics SET sessionId = @SessionId WHERE id = @SubTopicId";
                using var updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@SessionId", studySessionDto.Id);
                updateCmd.Parameters.AddWithValue("@SubTopicId", subTopicId.Value);
                await updateCmd.ExecuteNonQueryAsync();
            }

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
                                 SET UserId = @UserId, Notes = @Notes, 
                                     ScheduledStartTime = @ScheduledStartTime, ScheduledEndTime = @ScheduledEndTime, 
                                     ActualStartTime = @ActualStartTime, ActualEndTime = @ActualEndTime, 
                                     Status = @Status, FocusRating = @FocusRating, 
                                     ComprehensionRating = @ComprehensionRating, UpdatedAt = GETDATE() 
                                 WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@UserId", studySessionDto.UserId);
                    cmd.Parameters.AddWithValue("@Notes", studySessionDto.Notes ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ScheduledStartTime", studySessionDto.ScheduledStartTime);
                    cmd.Parameters.AddWithValue("@ScheduledEndTime", studySessionDto.ScheduledEndTime);
                    cmd.Parameters.AddWithValue("@ActualStartTime", (object)studySessionDto.ActualStartTime ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ActualEndTime", (object)studySessionDto.ActualEndTime ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", studySessionDto.status);
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
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? string.Empty : reader.GetString(reader.GetOrdinal("Notes")),
                ScheduledStartTime = reader.GetDateTime(reader.GetOrdinal("ScheduledStartTime")),
                ScheduledEndTime = reader.GetDateTime(reader.GetOrdinal("ScheduledEndTime")),
                ActualStartTime = reader.IsDBNull(reader.GetOrdinal("ActualStartTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ActualStartTime")),
                ActualEndTime = reader.IsDBNull(reader.GetOrdinal("ActualEndTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ActualEndTime")),
                status = reader.IsDBNull(reader.GetOrdinal("Status")) ? string.Empty : reader.GetString(reader.GetOrdinal("Status")),
                FocusRating = reader.IsDBNull(reader.GetOrdinal("FocusRating")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("FocusRating")),
                ComprehensionRating = reader.IsDBNull(reader.GetOrdinal("ComprehensionRating")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ComprehensionRating")),
                title= reader.IsDBNull(reader.GetOrdinal("title")) ? string.Empty : reader.GetString(reader.GetOrdinal("title")),
            };
        }
    }
}
