using CsvHelper;
using System.Globalization;
using System.IO;
using System.Text;

namespace FinanceTrackerClient.Models
{
    public static class AnalyticsExporter
    {
        public static void ExportToCsv(AnalyticsResult result, string filePath)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // Шапка
                csv.WriteField("Тип");
                csv.WriteField("Категорія/Дата");
                csv.WriteField("Сума");
                csv.NextRecord();
                // Категорії
                foreach (var cat in result.Categories)
                {
                    csv.WriteField("Категорія");
                    csv.WriteField(cat.Category);
                    csv.WriteField(cat.Amount);
                    csv.NextRecord();
                }
                csv.NextRecord(); // пустий рядок
                // Timeline
                foreach (var point in result.Timeline)
                {
                    csv.WriteField("Дата");
                    csv.WriteField(point.Date.ToString("yyyy-MM-dd"));
                    csv.WriteField($"Доходи: {point.Income}; Витрати: {point.Expenses}");
                    csv.NextRecord();
                }
            }
        }
    }
}