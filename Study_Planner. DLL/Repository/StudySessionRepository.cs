using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Study_Planner._DLL.IRepository;
using Study_Planner.Core.DTOs;

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
        public async Task<StudySessionDto> CreateStudySessionAsync(CreateStudySessionDto createDto)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = @"
                INSERT INTO StudySessions 
                (UserId, SubjectId, TopicId, SubTopicId, Notes, ScheduledStartTime, 
                ScheduledEndTime, Status, FocusRating, ComprehensionRating, CreatedAt, UpdatedAt) 
                OUTPUT INSERTED.Id 
                VALUES 
                (@UserId, @SubjectId, @TopicId, @SubTopicId, @Notes, @ScheduledStartTime, 
                @ScheduledEndTime, @Status, @FocusRating, @ComprehensionRating, GETDATE(), GETDATE())";

            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@UserId", createDto.UserId);
            cmd.Parameters.AddWithValue("@SubjectId", createDto.SubjectId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TopicId", createDto.TopicId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SubTopicId", createDto.SubTopicId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", createDto.Notes);
            cmd.Parameters.AddWithValue("@ScheduledStartTime", createDto.ScheduledStartTime);
            cmd.Parameters.AddWithValue("@ScheduledEndTime", createDto.ScheduledEndTime);
            cmd.Parameters.AddWithValue("@Status", createDto.Status);
            cmd.Parameters.AddWithValue("@FocusRating", createDto.FocusRating ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ComprehensionRating", createDto.ComprehensionRating ?? (object)DBNull.Value);

            var id = (int)await cmd.ExecuteScalarAsync();
            return await GetStudySessionByIdAsync(id);
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

        // UPDATE (Null-Check Based)
        public async Task<StudySessionDto> UpdateStudySessionAsync(int id, UpdateStudySessionDto updateDto)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var fieldsToUpdate = new List<string>();
            var parameters = new Dictionary<string, object>();

            if (updateDto.SubjectId.HasValue)
            {
                fieldsToUpdate.Add("SubjectId = @SubjectId");
                parameters["@SubjectId"] = updateDto.SubjectId;
            }
            if (updateDto.TopicId.HasValue)
            {
                fieldsToUpdate.Add("TopicId = @TopicId");
                parameters["@TopicId"] = updateDto.TopicId;
            }
            if (updateDto.SubTopicId.HasValue)
            {
                fieldsToUpdate.Add("SubTopicId = @SubTopicId");
                parameters["@SubTopicId"] = updateDto.SubTopicId;
            }
            if (!string.IsNullOrEmpty(updateDto.Notes))
            {
                fieldsToUpdate.Add("Notes = @Notes");
                parameters["@Notes"] = updateDto.Notes;
            }
            if (updateDto.ScheduledStartTime.HasValue)
            {
                fieldsToUpdate.Add("ScheduledStartTime = @ScheduledStartTime");
                parameters["@ScheduledStartTime"] = updateDto.ScheduledStartTime;
            }
            if (updateDto.ScheduledEndTime.HasValue)
            {
                fieldsToUpdate.Add("ScheduledEndTime = @ScheduledEndTime");
                parameters["@ScheduledEndTime"] = updateDto.ScheduledEndTime;
            }
            if (!string.IsNullOrEmpty(updateDto.Status))
            {
                fieldsToUpdate.Add("Status = @Status");
                parameters["@Status"] = updateDto.Status;
            }
            if (updateDto.FocusRating.HasValue)
            {
                fieldsToUpdate.Add("FocusRating = @FocusRating");
                parameters["@FocusRating"] = updateDto.FocusRating;
            }
            if (updateDto.ComprehensionRating.HasValue)
            {
                fieldsToUpdate.Add("ComprehensionRating = @ComprehensionRating");
                parameters["@ComprehensionRating"] = updateDto.ComprehensionRating;
            }

            if (!fieldsToUpdate.Any()) return await GetStudySessionByIdAsync(id); // No fields to update

            string query = $@"
                UPDATE StudySessions 
                SET {string.Join(", ", fieldsToUpdate)}, UpdatedAt = GETDATE() 
                WHERE Id = @Id";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            foreach (var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }

            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0 ? await GetStudySessionByIdAsync(id) : null;
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
                SubjectId = reader.IsDBNull("SubjectId") ? (int?)null : reader.GetInt32("SubjectId"),
                TopicId = reader.IsDBNull("TopicId") ? (int?)null : reader.GetInt32("TopicId"),
                SubTopicId = reader.IsDBNull("SubTopicId") ? (int?)null : reader.GetInt32("SubTopicId"),
                Notes = reader.GetString(reader.GetOrdinal("Notes")),
                ScheduledStartTime = reader.GetDateTime(reader.GetOrdinal("ScheduledStartTime")),
                ScheduledEndTime = reader.GetDateTime(reader.GetOrdinal("ScheduledEndTime")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                FocusRating = reader.IsDBNull("FocusRating") ? (int?)null : reader.GetInt32("FocusRating"),
                ComprehensionRating = reader.IsDBNull("ComprehensionRating") ? (int?)null : reader.GetInt32("ComprehensionRating"),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };
        }
    }
}
