using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportCompany
{
    class AutosMenu
    {
        public static List<string> table = new List<string>();
        public static void WriteMenu()
        {
            var wasChanged = false;
            var error = false;
            ConsoleKeyInfo k;

            do
            {
                if (wasChanged)
                    CreateTable();

                table.ForEach(t => Console.WriteLine(t));

                Console.WriteLine();
                if (error)
                {
                    Console.WriteLine("Ошибка ввода");
                    error = false;
                }
                Console.WriteLine("Выберите необходимое действие:");
                Console.WriteLine("1 - Добавить автомобиль");
                Console.WriteLine("2 - Редактировать автомобиль");
                Console.WriteLine("3 - Удалить автомобиль");
                Console.WriteLine("4 - Вернуться в предыдущее меню");

                k = Console.ReadKey(true);
                Console.Clear();
                switch (k.KeyChar)
                {
                    case '1':
                        wasChanged = Add();
                        break;
                    case '2':
                        wasChanged = Edit();
                        break;
                    case '3':
                        wasChanged = Delete();
                        break;
                    default:
                        error = true;
                        break;
                }
                Console.Clear();
            } while (k.KeyChar != '4');
        }

        public static void CreateTable()
        {
            string[] columns = new string[] { "ID", "Госномер", "Марка", "Модель", "Год выпуска",
                "Количество мест", "Расход топлива" };
            int[] widths = new int[columns.Length];

            table.Clear();
            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].Length;

            foreach (var a in Auto.autos)
            {
                if (a.id.ToString().Length > widths[0])
                    widths[0] = a.id.ToString().Length;
                if (a.number.Length > widths[1])
                    widths[1] = a.number.Length;
                if (a.brand.Length > widths[2])
                    widths[2] = a.brand.Length;
                if (a.model.Length > widths[3])
                    widths[3] = a.model.Length;
                if (a.year.ToString().Length > widths[4])
                    widths[4] = a.year.ToString().Length;
                if (a.placeCount.ToString().Length > widths[5])
                    widths[5] = a.placeCount.ToString().Length;
                if (a.consumption.ToString().Length > widths[6])
                    widths[6] = a.consumption.ToString().Length;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 3;

            var temp = new (string text, int width)[columns.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = (columns[i], widths[i]);
            table.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));

            foreach (var a in Auto.autos)
            {
                temp[0] = (a.id.ToString(), widths[0]);
                temp[1] = (a.number, widths[1]);
                temp[2] = (a.brand, widths[2]);
                temp[3] = (a.model, widths[3]);
                temp[4] = (a.year.ToString(), widths[4]);
                temp[5] = (a.placeCount.ToString(), widths[5]);
                temp[6] = (a.consumption.ToString(), widths[6]);
                table.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));
            }
        }

        public static bool Add()
        {
            string number, brand, model, yearStr, placeStr, consumptionStr;
            int year;
            int placeCount;
            decimal consumption;
            bool success;

            number = Program.ReadLine("Введите госномер автомобиля: ");
            brand = Program.ReadLine("Введите марку автомобиля: ");
            model = Program.ReadLine("Введите модель автомобиль: ");

            do
            {
                yearStr = Program.ReadLine("Введите год выпуска автомобиля: ");

                success = int.TryParse(yearStr, out year);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                placeStr = Program.ReadLine("Введите количество мест автомобиля: ");

                success = int.TryParse(placeStr, out placeCount);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                consumptionStr = Program.ReadLine("Введите расход топлива автомобиля: ");

                success = decimal.TryParse(consumptionStr, out consumption);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            Console.WriteLine("Госномер: {0}", number);
            Console.WriteLine("Марка: {0}", brand);
            Console.WriteLine("Модель: {0}", model);
            Console.WriteLine("Год выпуска: {0}", year);
            Console.WriteLine("Количество мест: {0}", placeCount);
            Console.WriteLine("Расход: {0}", consumption);
            Console.WriteLine("Добавить данный автомобиль?");
            Console.WriteLine("1 - Да");
            Console.WriteLine("2 - Нет");
            do
            {
                var k = Console.ReadKey(true);
                if (k.KeyChar == '1')
                    break;
                else if (k.KeyChar == '2')
                    return false;

            } while (true);

            Auto.autos.Add(new Auto
            {
                id = Auto.autos.Count > 0 ? Auto.autos.Last().id + 1 : 0,
                number = number,
                brand = brand,
                model = model,
                year = year,
                placeCount = placeCount,
                consumption = consumption,
            });
            return true;
        }

        public static bool Edit()
        {
            table.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID автомобиля, который необходимо отредактировать: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Auto a;
            if (!success || (a = Auto.autos.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Console.Clear();
            Console.WriteLine("В скобках указывается текущее значение.");
            Console.WriteLine("Если необходимо его оставить без изменений, оставьте строку ввода пустой.");
            Console.WriteLine();

            string number, brand, model, yearStr, placeStr, consumptionStr;
            int year = 0;
            int placeCount = 0;
            decimal consumption = 0;

            number = Program.ReadLine($"Введите госномер автомобиля ({a.number}): ", true);
            brand = Program.ReadLine($"Введите марку автомобиля ({a.brand}): ", true);
            model = Program.ReadLine($"Введите модель автомобиль ({a.model}): ", true);

            do
            {
                yearStr = Program.ReadLine($"Введите год выпуска автомобиля ({a.year}): ", true);
                if (string.IsNullOrWhiteSpace(yearStr))
                    break;

                success = int.TryParse(yearStr, out year);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                placeStr = Program.ReadLine($"Введите количество мест автомобиля ({a.placeCount}): ", true);
                if (string.IsNullOrWhiteSpace(placeStr))
                    break;

                success = int.TryParse(placeStr, out placeCount);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                consumptionStr = Program.ReadLine($"Введите расход топлива автомобиля ({a.consumption}): ", true);
                if (string.IsNullOrWhiteSpace(consumptionStr))
                    break;

                success = decimal.TryParse(consumptionStr, out consumption);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            Console.WriteLine("Госномер: {0}", string.IsNullOrWhiteSpace(number) ? a.number : number);
            Console.WriteLine("Марка: {0}", string.IsNullOrWhiteSpace(brand) ? a.brand : brand);
            Console.WriteLine("Модель: {0}", string.IsNullOrWhiteSpace(model) ? a.model : model);
            Console.WriteLine("Год выпуска: {0}", string.IsNullOrWhiteSpace(yearStr) ? a.year : year);
            Console.WriteLine("Количество мест: {0}", string.IsNullOrWhiteSpace(placeStr) ? a.placeCount : placeCount);
            Console.WriteLine("Расход: {0}", string.IsNullOrWhiteSpace(consumptionStr) ? a.consumption : consumption);
            Console.WriteLine("Сохранить данный автомобиль?");
            Console.WriteLine("1 - Да");
            Console.WriteLine("2 - Нет");
            do
            {
                var k = Console.ReadKey(true);
                if (k.KeyChar == '1')
                    break;
                else if (k.KeyChar == '2')
                    return false;

            } while (true);

            if (!string.IsNullOrWhiteSpace(number)) a.number = number;
            if (!string.IsNullOrWhiteSpace(brand)) a.brand = brand;
            if (!string.IsNullOrWhiteSpace(model)) a.model = model;
            if (!string.IsNullOrWhiteSpace(yearStr)) a.year = year;
            if (!string.IsNullOrWhiteSpace(placeStr)) a.placeCount = placeCount;
            if (!string.IsNullOrWhiteSpace(consumptionStr)) a.consumption = consumption;

            return true;
        }

        public static bool Delete()
        {
            table.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();
            Console.Write("Введите ID автомобилz, которого необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Auto d;
            if (!success || (d = Auto.autos.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Route.routes.RemoveAll(c => c.auto.id == d.id);
            RoutesMenu.CreateTable();
            Auto.autos.Remove(d);
            return true;
        }
    }
}