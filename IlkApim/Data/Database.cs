using IlkApim.Models;

namespace IlkApim.Data
{
    /// <summary>
    /// Geliştirme aşamasında database gibi davranan geçici local database
    /// </summary>
    public static class Database
    {
        // TODO: İleriki mimari aşamalarda SQL veritabanına geçirilecek.
        public static List<User> Users { get; set; } = new List<User>
        {
            new User { Username = "Admin", Password = "123456", Role = "Admin" }
        };
    }
}