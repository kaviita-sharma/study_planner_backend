using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Study_Planner._DLL.IRepository;
using Study_Planner.Core.DTOs;

namespace Study_Planner._DLL.Repository
{
    public class UserPreferenceRepository : IUserPreferenceRepository
    {
        private readonly string _connectionString;

        public UserPreferenceRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<UserPreference> GetByUserIdAsync(int userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM UserPreferences WHERE UserId = @UserId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new UserPreference
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                LearningStyle = reader.GetString(reader.GetOrdinal("LearningStyle")),
                                PreferredStudyTime = reader.GetString(reader.GetOrdinal("PreferredStudyTime")),
                                StudySessionDuration = reader.GetInt32(reader.GetOrdinal("StudySessionDuration")),
                                BreakDuration = reader.GetInt32(reader.GetOrdinal("BreakDuration")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public async Task<int> CreateAsync(UserPreference preference)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    INSERT INTO UserPreferences (UserId, LearningStyle, PreferredStudyTime, StudySessionDuration, BreakDuration, UpdatedAt)
                    VALUES (@UserId, @LearningStyle, @PreferredStudyTime, @StudySessionDuration, @BreakDuration, @UpdatedAt);
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", preference.UserId);
                    command.Parameters.AddWithValue("@LearningStyle", preference.LearningStyle);
                    command.Parameters.AddWithValue("@PreferredStudyTime", preference.PreferredStudyTime);
                    command.Parameters.AddWithValue("@StudySessionDuration", preference.StudySessionDuration);
                    command.Parameters.AddWithValue("@BreakDuration", preference.BreakDuration);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<bool> UpdateAsync(UserPreference preference)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    UPDATE UserPreferences
                    SET LearningStyle = @LearningStyle,
                        PreferredStudyTime = @PreferredStudyTime,
                        StudySessionDuration = @StudySessionDuration,
                        BreakDuration = @BreakDuration,
                        UpdatedAt = @UpdatedAt
                    WHERE UserId = @UserId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", preference.UserId);
                    command.Parameters.AddWithValue("@LearningStyle", preference.LearningStyle);
                    command.Parameters.AddWithValue("@PreferredStudyTime", preference.PreferredStudyTime);
                    command.Parameters.AddWithValue("@StudySessionDuration", preference.StudySessionDuration);
                    command.Parameters.AddWithValue("@BreakDuration", preference.BreakDuration);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM UserPreferences WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
