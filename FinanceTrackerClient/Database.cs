using FinanceTrackerClient.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace FinanceTrackerClient
{
    public static class Database
    {
        private static readonly string connectionString =
            "Server=DESKTOP-M39AAQO;Database=PersonalFinanceTracker;Trusted_Connection=True;";

        public static List<Category> GetCategories()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<Category>("SELECT * FROM Categories").ToList();
            }
        }

        public static void AddEntry(Entry entry)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Execute(@"INSERT INTO Entries (UserID, CategoryID, Type, Amount, Date, Note)
                                 VALUES (@UserID, @CategoryID, @Type, @Amount, @Date, @Note)", entry);
            }
        }

        public static List<Entry> GetEntries(int userId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<Entry>("SELECT * FROM Entries WHERE UserID = @UserID", new { UserID = userId }).ToList();
            }
        }

        public static void DeleteEntry(int entryId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Execute("DELETE FROM Entries WHERE EntryID = @EntryID", new { EntryID = entryId });
            }
        }

        public static bool Register(string login, string password)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var existing = connection.QueryFirstOrDefault<AuthUser>(
                    "SELECT * FROM Auth WHERE Login = @Login", new { Login = login });
                if (existing != null)
                    return false; //логін зайнятий
                string hash = GetHash(password); //хешуємо
                connection.Execute(
                    "INSERT INTO Auth (Login, Password) VALUES (@Login, @Password)",
                    new { Login = login, Password = hash });
                return true;
            }
        }

        public static AuthUser Login(string login, string password)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string hash = GetHash(password); //хеш для порівняння
                return connection.QueryFirstOrDefault<AuthUser>(
                    "SELECT * FROM Auth WHERE Login = @Login AND Password = @Password",
                    new { Login = login, Password = hash });
            }
        }
        //Метод для отримання хешу пароля
        public static string GetHash(string input)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(input);
                var hashBytes = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static Category GetCategoryById(int categoryId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.QueryFirstOrDefault<Category>(
                    "SELECT * FROM Categories WHERE CategoryID = @id", new { id = categoryId });
            }
        }
    }
}