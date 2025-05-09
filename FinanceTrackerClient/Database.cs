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
    }
}