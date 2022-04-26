using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportCompany
{
    class RoutesMenu
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
                Console.WriteLine("1 - Добавить маршрут");
                Console.WriteLine("2 - Редактировать маршрут");
                Console.WriteLine("3 - Удалить маршрут");
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
            string[] columns = new string[] { "ID", "Водитель", "Автомобиль", "Дата и время отправки", "Место отправки",
                "Место прибытия" };
            int[] widths = new int[columns.Length];

            table.Clear();
            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].Length;

            foreach (var r in Route.routes)
            {
                if (r.id.ToString().Length > widths[0])
                    widths[0] = r.id.ToString().Length;
                if (r.driver.ToString().Length > widths[1])
                    widths[1] = r.driver.ToString().Length;
                if (r.auto.ToString().Length > widths[2])
                    widths[2] = r.auto.ToString().Length;
                if (r.departurePoint.Length > widths[4])
                    widths[4] = r.departurePoint.Length;
                if (r.arrivalPoint.Length > widths[5])
                    widths[5] = r.arrivalPoint.Length;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 3;

            var temp = new (string text, int width)[columns.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = (columns[i], widths[i]);
            table.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));

            foreach (var d in Route.routes)
            {
                temp[0] = (d.id.ToString(), widths[0]);
                temp[1] = (d.driver.ToString(), widths[1]);
                temp[2] = (d.auto.ToString(), widths[2]);
                temp[3] = (d.dateTime.ToString("dd.MM.yy HH:mm"), widths[3]);
                temp[4] = (d.departurePoint, widths[4]);
                temp[5] = (d.arrivalPoint, widths[5]);
                table.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));
            }
        }

        public static bool Add()
        {
            string driverStr, autoStr, dateStr, departurePoint, arrivalPoint;
            DateTime dateTime;
            int driver;
            int auto;
            bool success;

            Console.Clear();
            DriversMenu.table.ForEach(t => Console.WriteLine(t));
            do
            {
                driverStr = Program.ReadLine("Введите ID водителя: ");

                success = int.TryParse(driverStr, out driver) &&
                    Driver.drivers.Any(t => t.id == driver);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            AutosMenu.table.ForEach(t => Console.WriteLine(t));
            do
            {
                autoStr = Program.ReadLine("Введите ID автомобиля: ");

                success = int.TryParse(autoStr, out auto) &&
                    Auto.autos.Any(t => t.id == auto);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                dateStr = Program.ReadLine("Введите дату и время отправления: ");

                success = DateTime.TryParse(dateStr, out dateTime);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            departurePoint = Program.ReadLine("Введите место отправления: ");
            arrivalPoint = Program.ReadLine("Введите место прибытия: ");

            Console.Clear();
            Console.WriteLine("Водитель: {0}", Driver.drivers.First(t => t.id == driver));
            Console.WriteLine("Автомобиль: {0}", Auto.autos.First(t => t.id == auto));
            Console.WriteLine("Дата и время отправления: {0}", dateTime.ToString("dd.mm.yy HH:mm"));
            Console.WriteLine("Место отправления: {0}", departurePoint);
            Console.WriteLine("Место прибытия: {0}", arrivalPoint);
            Console.WriteLine("Добавить данный маршрут?");
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

            Route.routes.Add(new Route
            {
                id = Route.routes.Count > 0 ? Route.routes.Last().id + 1 : 0,
                driver = Driver.drivers.First(t => t.id == driver),
                auto = Auto.autos.First(t => t.id == auto),
                dateTime = dateTime,
                departurePoint = departurePoint,
                arrivalPoint = arrivalPoint,
            });
            return true;
        }

        public static bool Edit()
        {
            table.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID маршрута, который необходимо отредактировать: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Route r;
            if (!success || (r = Route.routes.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Console.Clear();
            Console.WriteLine("В скобках указывается текущее значение.");
            Console.WriteLine("Если необходимо его оставить без изменений, оставьте строку ввода пустой.");
            Console.WriteLine();

            string driverStr, autoStr, dateStr, departurePoint, arrivalPoint;
            DateTime dateTime = new DateTime();
            int driver = 0;
            int auto = 0;

            Console.Clear();
            DriversMenu.table.ForEach(t => Console.WriteLine(t));
            do
            {
                driverStr = Program.ReadLine($"Введите ID водителя ({r.driver.id}): ", true);
                if (string.IsNullOrWhiteSpace(driverStr))
                    break;

                success = int.TryParse(driverStr, out driver) &&
                    Driver.drivers.Any(t => t.id == driver);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            AutosMenu.table.ForEach(t => Console.WriteLine(t));
            do
            {
                autoStr = Program.ReadLine($"Введите ID автомобиля ({r.auto.id}): ", true);
                if (string.IsNullOrWhiteSpace(autoStr))
                    break;

                success = int.TryParse(autoStr, out auto) &&
                    Auto.autos.Any(t => t.id == auto);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                dateStr = Program.ReadLine($"Введите дату и время отправления " +
                    $"({r.dateTime.ToString("dd.MM.yy HH:mm")}): ", true);
                if (string.IsNullOrWhiteSpace(dateStr))
                    break;

                success = DateTime.TryParse(dateStr, out dateTime);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            departurePoint = Program.ReadLine($"Введите место отправления ({r.departurePoint}): ", true);
            arrivalPoint = Program.ReadLine($"Введите место прибытия ({r.arrivalPoint}): ", true);

            Console.Clear();
            Console.WriteLine("Водитель: {0}", string.IsNullOrWhiteSpace(driverStr) ? 
                r.driver : Driver.drivers.First(t => t.id == driver));
            Console.WriteLine("Автомобиль: {0}", string.IsNullOrWhiteSpace(autoStr) ? 
                r.auto : Auto.autos.First(t => t.id == auto));
            Console.WriteLine("Дата и время отправления: {0}", string.IsNullOrWhiteSpace(dateStr) ? 
                r.dateTime.ToString("dd.mm.yy HH:mm") : dateTime.ToString("dd.mm.yy HH:mm"));
            Console.WriteLine("Место отправления: {0}", string.IsNullOrWhiteSpace(departurePoint) ? 
                r.departurePoint : departurePoint);
            Console.WriteLine("Место прибытия: {0}", string.IsNullOrWhiteSpace(arrivalPoint) ? 
                r.arrivalPoint : arrivalPoint);
            Console.WriteLine("Сохранить данный маршрут?");
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

            if (!string.IsNullOrWhiteSpace(driverStr))
                r.driver = Driver.drivers.First(t => t.id == driver);
            if (!string.IsNullOrWhiteSpace(autoStr))
                r.auto = Auto.autos.First(t => t.id == auto);
            if (!string.IsNullOrWhiteSpace(dateStr)) r.dateTime = dateTime;
            if (!string.IsNullOrWhiteSpace(departurePoint))
                r.departurePoint = departurePoint;
            if (!string.IsNullOrWhiteSpace(arrivalPoint))
                r.arrivalPoint = arrivalPoint;

            return true;
        }

        public static bool Delete()
        {
            table.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();
            Console.Write("Введите ID маршрута, который необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Route d;
            if (!success || (d = Route.routes.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Route.routes.RemoveAll(c => c.id == d.id);
            return true;
        }
    }
}