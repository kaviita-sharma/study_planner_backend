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
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<User> GetByIdAsync(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT u.*, up.* 
                    FROM Users u
                    LEFT JOIN UserPreferences up ON u.Id = up.UserId
                    WHERE u.Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapUserFromReader(reader);
                        }
                        return null;
                    }
                }
            }
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT u.*, up.* 
                    FROM Users u
                    LEFT JOIN UserPreferences up ON u.Id = up.UserId
                    WHERE u.Email = @Email";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapUserFromReader(reader);
                        }
                        return null;
                    }
                }
            }
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT u.*, up.* 
                    FROM Users u
                    LEFT JOIN UserPreferences up ON u.Id = up.UserId
                    WHERE u.Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapUserFromReader(reader);
                        }
                        return null;
                    }
                }
            }
        }

        public async Task<int> CreateAsync(User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Use a transaction to ensure both user and preferences are created or neither is
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Create user
                        string query = @"
                            INSERT INTO Users (Username, Email, PasswordHash, Salt, FirstName, LastName, IsActive)
                            VALUES (@Username, @Email, @PasswordHash, @Salt, @FirstName, @LastName, @IsActive);
                            SELECT SCOPE_IDENTITY();";

                        int userId;
                        using (SqlCommand command = new SqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Username", user.Username);
                            command.Parameters.AddWithValue("@Email", user.Email);
                            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                            command.Parameters.AddWithValue("@Salt", user.Salt);
                            command.Parameters.AddWithValue("@FirstName", (object)user.FirstName ?? DBNull.Value);
                            command.Parameters.AddWithValue("@LastName", (object)user.LastName ?? DBNull.Value);
                            command.Parameters.AddWithValue("@IsActive", user.IsActive);

                            var result = await command.ExecuteScalarAsync();
                            userId = Convert.ToInt32(result);
                        }

                        transaction.Commit();
                        return userId;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<bool> UpdateAsync(User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    UPDATE Users
                    SET Username = @Username,
                        Email = @Email,
                        FirstName = @FirstName,
                        LastName = @LastName,
                        IsActive = @IsActive
                    WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@FirstName", (object)user.FirstName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LastName", (object)user.LastName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", user.IsActive);

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

                string query = "DELETE FROM Users WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT COUNT(1) FROM Users WHERE Email = @Email";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return count == 0;
                }
            }
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return count == 0;
                }
            }
        }

        private User MapUserFromReader(SqlDataReader reader)
        {
            var user = new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                Salt = reader.GetString(reader.GetOrdinal("Salt")),
                FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };

            // Check if user has preferences by checking if UserId column exists and has a value
            if (reader.HasRows && !reader.IsDBNull(reader.GetOrdinal("UserId")))
            {
                user.Preferences = new UserPreference
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

            return user;
        }
    }
}
