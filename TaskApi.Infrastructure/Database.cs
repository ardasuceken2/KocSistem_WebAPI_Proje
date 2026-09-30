using System.Collections.Generic;
using TaskApi.Domain;

namespace TaskApi.Infrastructure
{
    public static class Database
    {
        public static List<User> Users { get; set; } = new List<User>
        {
            new User { Username = "Admin", Password = "123456", Role = "Admin" }
        };
    }
}