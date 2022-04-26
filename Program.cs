using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportCompany
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Транспортная компания";
            Console.BufferWidth = 200;

            bool error = false;
            ReadDrivers(ref error);
            ReadAutos(ref error);
            ReadRoutes(ref error);

            if (error)
            {
                Console.WriteLine("При чтении файла были обнаружены некорректные записи.");
                Console.WriteLine("Они не были добавлены в таблицы и будут удалены");
                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить");
                Console.ReadKey(true);
            }
            DriversMenu.CreateTable();
            AutosMenu.CreateTable();
            RoutesMenu.CreateTable();

            ConsoleKeyInfo key;
            error = false;

            do
            {
                if (!error)
                {
                    Console.Write(new string('*', Console.BufferWidth));
                    Console.WriteLine("Добро пожаловать в информационную систему транспортной компании");
                    Console.Write(new string('*', Console.BufferWidth));
                }
                else
                {
                    Console.WriteLine("Ошибка ввода");
                    error = false;
                }

                Console.WriteLine("Выберите необходимое действие:");
                Console.WriteLine("1 - Работа с таблицей водителей");
                Console.WriteLine("2 - Работа с таблицей автомобилей");
                Console.WriteLine("3 - Работа с таблицей маршрутов");
                Console.WriteLine("4 - Выход из программы");
                key = Console.ReadKey(true);
                Console.Clear();
                switch (key.KeyChar)
                {
                    case '1':
                        DriversMenu.WriteMenu();
                        break;
                    case '2':
                        AutosMenu.WriteMenu();
                        break;
                    case '3':
                        RoutesMenu.WriteMenu();
                        break;
                    default:
                        error = true;
                        break;
                }
            } while (key.KeyChar != '4');
            SaveAll();
        }

        static void ReadDrivers(ref bool error)
        {
            int id;
            string[] data;
            var reader = new StreamReader(new FileStream("drivers.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite));

            while (!reader.EndOfStream)
            {
                try
                {
                    data = reader.ReadLine().Split('\t');
                    id = int.Parse(data[0]);
                    if (Driver.drivers.Any(t => t.id == id))
                        throw new ArgumentException();

                    Driver.drivers.Add(new Driver
                    {
                        id = id,
                        lastName = data[1],
                        firstName = data[2],
                        birthDate = DateTime.Parse(data[3]),
                        hireDate = DateTime.Parse(data[4]),
                        phone = data[5],
                        driveCategory = (DriveCategory)Enum.Parse(typeof(DriveCategory), data[6])
                    });
                }
                catch
                {
                    error = true;
                }
            }
            reader.Close();
        }

        static void ReadAutos(ref bool error)
        {
            int id;
            string[] data;
            var reader = new StreamReader(new FileStream("autos.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite));
            while (!reader.EndOfStream)
            {
                try
                {
                    data = reader.ReadLine().Split('\t');
                    id = int.Parse(data[0]);
                    if (Auto.autos.Any(t => t.id == id))
                        throw new ArgumentException();

                    Auto.autos.Add(new Auto
                    {
                        id = id,
                        number = data[1],
                        brand = data[2],
                        model = data[3],
                        year = int.Parse(data[4]),
                        placeCount = int.Parse(data[5]),
                        consumption = decimal.Parse(data[6]),
                    });
                }
                catch
                {
                    error = true;
                }
            }
            reader.Close();
        }

        static void ReadRoutes(ref bool error)
        {
            int id;
            string[] data;
            var reader = new StreamReader(new FileStream("routes.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite));
            while (!reader.EndOfStream)
            {
                try
                {
                    data = reader.ReadLine().Split('\t');
                    id = int.Parse(data[0]);
                    if (Route.routes.Any(t => t.id == id))
                        throw new ArgumentException();


                    Route.routes.Add(new Route
                    {
                        id = id,
                        driver = Driver.drivers.First(t => t.id == int.Parse(data[1])),
                        auto = Auto.autos.First(t => t.id == int.Parse(data[2])),
                        dateTime = DateTime.Parse(data[3]),
                        departurePoint = data[4],
                        arrivalPoint = data[5],
                    });
                }
                catch
                {
                    error = true;
                }
            }
            reader.Close();
        }

        static void SaveAll()
        {
            StreamWriter writer;
            writer = new StreamWriter("drivers.txt");
            foreach (var d in Driver.drivers)
                writer.WriteLine(string.Join("\t", d.id, d.lastName, d.firstName,
                    d.birthDate.ToShortDateString(), d.hireDate.ToShortDateString(),
                    d.phone, (int)d.driveCategory));
            writer.Close();

            writer = new StreamWriter("autos.txt");
            foreach (var a in Auto.autos)
                writer.WriteLine(string.Join("\t", a.id, a.number, a.brand,
                    a.model, a.year, a.placeCount, a.consumption));
            writer.Close();

            writer = new StreamWriter("routes.txt");
            foreach (var r in Route.routes)
                writer.WriteLine(string.Join("\t", r.id, r.driver.id, r.auto.id,
                    r.dateTime.ToString(), r.departurePoint, r.arrivalPoint));
            writer.Close();
        }

        public static string ReadLine(string text, bool allowEmpty = false)
        {
            bool success;
            string str;
            do
            {
                Console.Write(text);
                str = Console.ReadLine().Trim();
                success = !string.IsNullOrWhiteSpace(str) || allowEmpty;
                if (!success)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(new string('\0', Console.BufferWidth));
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                }
            } while (!success);
            return str;
        }
    }
}
