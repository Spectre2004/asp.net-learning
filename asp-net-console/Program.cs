using ExcelMapper;
using System.Linq;
using System.Text.Encodings;

namespace asp_net_console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var tanks = GetTanks();
            var units = GetUnits();
            var factories = GetFactories();
            Console.WriteLine($"Количество резервуаров: {tanks.Length}, установок: {units.Length}");

            var foundUnit = FindUnit(units, tanks, "Резервуар 256");
            var factory = FindFactory(factories, foundUnit);

            Console.WriteLine($"Резервуар 256 принадлежит установке {foundUnit.Name} и заводу {factory.Name}");

            var totalVolume = GetTotalVolume(tanks);
            Console.WriteLine($"Общий объем резервуаров: {totalVolume}");

            foreach (var item in tanks)
            {
                Console.WriteLine($"{item.Name} принадлежит установке {FindUnit(units, tanks, item.Name!).Name} и заводу {FindFactory(factories, foundUnit).Name}");
            }

            Console.WriteLine("------------------------------------------------------------------------------------------------");
            /*Console.Write();*/
            Console.ReadLine();

        }

        // реализуйте этот метод, чтобы он возвращал массив резервуаров, согласно приложенным таблицам
        // можно использовать создание объектов прямо в C# коде через new, или читать из файла (на своё усмотрение)
        public static Tank[] GetTanks()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); //строка для исправления ошибки с кодировкой 1251

            string path = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;

            using var stream = File.OpenRead(path + "/Data/Tanks.xlsx");
            using var importer = new ExcelImporter(stream);
            ExcelSheet sheet = importer.ReadSheet();

            Tank[] tanks = sheet.ReadRows<Tank>().ToArray();

            return tanks;
        }
        // реализуйте этот метод, чтобы он возвращал массив установок, согласно приложенным таблицам
        public static Unit[] GetUnits()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); //строка для исправления ошибки с кодировкой 1251

            string path = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;

            using var stream = File.OpenRead(path + "/Data/Units.xlsx");
            using var importer = new ExcelImporter(stream);
            ExcelSheet sheet = importer.ReadSheet();

            Unit[] units = sheet.ReadRows<Unit>().ToArray();

            return units;
        }
        // реализуйте этот метод, чтобы он возвращал массив заводов, согласно приложенным таблицам
        public static Factory[] GetFactories()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); //строка для исправления ошибки с кодировкой 1251

            string path = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;

            using var stream = File.OpenRead(path + "/Data/Factories.xlsx");
            using var importer = new ExcelImporter(stream);
            ExcelSheet sheet = importer.ReadSheet();

            Factory[] factories = sheet.ReadRows<Factory>().ToArray();

            return factories;
        }

        // реализуйте этот метод, чтобы он возвращал установку (Unit), которой
        // принадлежит резервуар (Tank), найденный в массиве резервуаров по имени
        // учтите, что по заданному имени может быть не найден резервуар
        public static Unit FindUnit(Unit[] units, Tank[] tanks, string tankName)
        {
            var searchedTank = tanks.Where(t => t.Name == tankName);
            if (searchedTank.Count() == 0) { return new Unit(); }
            var searchedUnit = units.Single(u => u.Id == searchedTank.Single().UnitId);

            return searchedUnit;
        }

        // реализуйте этот метод, чтобы он возвращал объект завода, соответствующий установке
        public static Factory FindFactory(Factory[] factories, Unit unit)
        {
            return factories.Single(f => f.Id == unit.FactoryId);
        }

        // реализуйте этот метод, чтобы он возвращал суммарный объем резервуаров в массиве
        public static int GetTotalVolume(Tank[] units)
        {
            return units.Sum(u => u.Volume);
        }
    }

    /// <summary>
    /// Установка
    /// </summary>
    public class Unit
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int FactoryId { get; set; }
    }

    /// <summary>
    /// Завод
    /// </summary>
    public class Factory
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// Резервуар
    /// </summary>
    public class Tank
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Volume { get; set; }
        public int MaxVolume { get; set; }
        public int UnitId { get; set; }
    }
}