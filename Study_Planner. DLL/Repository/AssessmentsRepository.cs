using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Study_Planner._DLL.IRepository;
using Study_Planner.Core.DTOs;

namespace Study_Planner._DLL.Repository
{
    public class AssessmentsRepository : IAssessmentsRepository
    {
        private readonly string _connectionString;

        public AssessmentsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

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

        public int CreateAssessment(AssessmentDTO assessment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
                    INSERT INTO Assessments (UserId, SubjectId, TopicId, AssessmentName, Description, 
                    AssessmentType, MaxScore, ActualScore, CompletionDate, DueDate, IsCompleted, CreatedAt, UpdatedAt) 
                    VALUES (@UserId, @SubjectId, @TopicId, @AssessmentName, @Description, @AssessmentType, 
                    @MaxScore, @ActualScore, @CompletionDate, @DueDate, @IsCompleted, GETDATE(), GETDATE());
                    SELECT SCOPE_IDENTITY();", connection);

                AddParameters(command, assessment);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public bool UpdateAssessment(int id, AssessmentDTO assessment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
                    UPDATE Assessments 
                    SET UserId = @UserId, SubjectId = @SubjectId, TopicId = @TopicId, AssessmentName = @AssessmentName, 
                    Description = @Description, AssessmentType = @AssessmentType, MaxScore = @MaxScore, 
                    ActualScore = @ActualScore, CompletionDate = @CompletionDate, DueDate = @DueDate, 
                    IsCompleted = @IsCompleted, UpdatedAt = GETDATE()
                    WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", id);
                AddParameters(command, assessment);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

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

        private void AddParameters(SqlCommand command, AssessmentDTO assessment)
        {
            command.Parameters.AddWithValue("@UserId", assessment.UserId);
            command.Parameters.AddWithValue("@SubjectId", assessment.SubjectId);
            command.Parameters.AddWithValue("@TopicId", (object?)assessment.TopicId ?? DBNull.Value);
            command.Parameters.AddWithValue("@AssessmentName", assessment.AssessmentName);
            command.Parameters.AddWithValue("@Description", (object?)assessment.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@AssessmentType", assessment.AssessmentType);
            command.Parameters.AddWithValue("@MaxScore", assessment.MaxScore);
            command.Parameters.AddWithValue("@ActualScore", (object?)assessment.ActualScore ?? DBNull.Value);
            command.Parameters.AddWithValue("@CompletionDate", (object?)assessment.CompletionDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@DueDate", (object?)assessment.DueDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsCompleted", assessment.IsCompleted);
        }

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
                IsCompleted = Convert.ToBoolean(reader["IsCompleted"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
            };
        }
    }
}
