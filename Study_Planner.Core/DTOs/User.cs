using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.Core.DTOs
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public UserPreference Preferences { get; set; }
    }
    public class UserPreference
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string LearningStyle { get; set; }
        public string PreferredStudyTime { get; set; }
        public int StudySessionDuration { get; set; }
        public int BreakDuration { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


}
