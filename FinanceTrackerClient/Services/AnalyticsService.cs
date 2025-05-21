using System;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceTrackerClient.Models
{
    public interface IAnalyticsService
    {
        Task<AnalyticsResult> GetAnalyticsAsync(int userId, string period);
    }
    public class AnalyticsService : IAnalyticsService
    {
        public async Task<AnalyticsResult> GetAnalyticsAsync(int userId, string period)
        {
            return await Task.Run(() =>
            {
                DateTime start;
                switch (period)
                {
                    case "day":
                        start = DateTime.Today;
                        break;
                    case "week":
                        start = DateTime.Today.AddDays(-7);
                        break;
                    case "month":
                        start = DateTime.Today.AddMonths(-1);
                        break;
                    case "year":
                        start = DateTime.Today.AddYears(-1);
                        break;
                    default:
                        start = DateTime.Today.AddMonths(-1);
                        break;
                }
                // Отримуємо всі записи за період для користувача
                var entries = Database.GetEntries(userId)
                    .Where(e => e.Date >= start)
                    .ToList();
                // Групуємо за категоріями
                var categories = entries
                    .GroupBy(e => e.CategoryID)
                    .Select(g =>
                    {
                        var category = Database.GetCategoryById(g.Key);
                        return new CategoryStat
                        {
                            Category = category?.Name ?? "Невідомо",
                            Amount = g.Sum(e => e.Amount)
                        };
                    })
                    .ToList();
                // Тимчасова лінія (групування по датах)
                var timeline = entries
                    .GroupBy(e => e.Date.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new TimePoint
                    {
                        Date = g.Key,
                        Income = g.Where(e => e.Type == "Income").Sum(e => e.Amount),
                        Expenses = g.Where(e => e.Type == "Expense").Sum(e => e.Amount)
                    })
                    .ToList();

                return new AnalyticsResult
                {
                    Categories = categories,
                    Timeline = timeline,
                    Period = period
                };
            });
        }
    }
}