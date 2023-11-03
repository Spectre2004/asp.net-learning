using Lesson1ConsoleApp.Model;
using System.Collections.Immutable;
using System.Text.Json;

namespace Lesson1ConsoleApp
{
    internal class Program
    {
        public static readonly string path = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
        static void Main(string[] args)
        {
            //Консольное приложение в функции Main должно вызывать методы GetNumbersOfDeals и GetSumsByMonth и выводить в консоль результаты
            //(количество найденных значений, сами идентификаторы через запятую, пары месяц-сумма).

            IList<Deal> deal = DealJsonParser(path + "/Data/JSON_sample_1.json");

            Console.WriteLine($"Значений найденно - {deal.Count}");
            Console.WriteLine();
            var numbersOfDeals = GetNumbersOfDeals(deal);
            foreach (var item in numbersOfDeals)
            {
                Console.Write($"{item},");
            }
            Console.WriteLine();
            Console.WriteLine();
            var sumByMonth = GetSumsByMonth(deal);
            foreach (var item in sumByMonth)
            {
                Console.WriteLine($"{item.Month} {item.Sum}");
            }
        }

        public static IList<Deal> DealJsonParser(string jsonPath)
        {
            //чтение из json файла (десериализация)
            string jsonStr = File.ReadAllText(jsonPath);
            IList<Deal>? array = JsonSerializer.Deserialize<IList<Deal>>(jsonStr);
            if (array == null)
            {
                return new Deal[] { };
            }
            return array;
        }

        public static IList<string> GetNumbersOfDeals(IEnumerable<Deal> deals)
        {
            IList<string> result = new List<string>();
            deals = deals.Where(d => d.Sum >= 100).OrderBy(d => d.Date).Take(5);
            deals = deals.OrderByDescending(d => d.Sum);
            foreach (Deal deal in deals)
            {
                result.Add(deal.Id);
            }
            return result;

        }

        // record - это синтаксический сахар для объявления класса (имеет семантику сравнения по значению)
        // подробнее https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/records

        // Реализовать метод GetSumsByMonth, который принимает коллекцию объектов класса Deal, 
        // группирует по месяцу сделки и возвращает сумму сделок за каждый месяц

        public record SumByMonth(DateTime Month, int Sum);

        public static IList<SumByMonth> GetSumsByMonth(IEnumerable<Deal> deals)
        {
            //создаем лист SumByMonth из 
            // данных из deals и потом группируем по месяцу
            var result = deals.Select(d => new SumByMonth(new DateTime(d.Date.Year, d.Date.Month, 1), d.Sum))
                              .GroupBy(r => r.Month)
                              .Select(r => new SumByMonth(r.Key, r.Sum(r => r.Sum))).OrderBy(r => r.Month).ToList();

            //создаем лист SumByMonth только теперь по каждому месяцу сумма r.Key = месяц из группировки, r.Sum(r => r.Sum) суммируем Sum из коллекции 
            // IGrouping
            return result;
        }
    }
}