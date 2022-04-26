using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportCompany
{
    class DriversMenu
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
                Console.WriteLine("1 - Добавить водителя");
                Console.WriteLine("2 - Редактировать водителя");
                Console.WriteLine("3 - Удалить водителя");
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
            string[] columns = new string[] { "ID", "Фамилия", "Имя", "Дата рождения", "Дата найма", 
                "Номер телефона", "Водительские категории" };
            int[] widths = new int[columns.Length];

            table.Clear();
            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].Length;

            foreach (var d in Driver.drivers)
            {
                if (d.id.ToString().Length > widths[0])
                    widths[0] = d.id.ToString().Length;
                if (d.lastName.Length > widths[1])
                    widths[1] = d.lastName.Length;
                if (d.firstName.Length > widths[2])
                    widths[2] = d.firstName.Length;
                if (d.phone.Length > widths[5])
                    widths[5] = d.phone.Length;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 3;

            var temp = new (string text, int width)[columns.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = (columns[i], widths[i]);
            table.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));

            foreach (var d in Driver.drivers)
            {
                temp[0] = (d.id.ToString(), widths[0]);
                temp[1] = (d.lastName, widths[1]);
                temp[2] = (d.firstName, widths[2]);
                temp[3] = (d.birthDate.ToString("dd.MM.yyyy"), widths[3]);
                temp[4] = (d.hireDate.ToString("dd.MM.yyyy"), widths[4]);
                temp[5] = (d.phone, widths[5]);
                temp[6] = (d.driveCategory.ToString(), widths[6]);
                table.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));
            }
        }

        public static bool Add()
        {
            string lastName, firstName, birthStr, hireStr, phone;
            DateTime birthDate;
            DateTime hireDate;
            DriveCategory category = DriveCategory.None;
            bool success;

            lastName = Program.ReadLine("Введите фамилию водителя: ");
            firstName = Program.ReadLine("Введите имя водителя: ");

            do
            {
                birthStr = Program.ReadLine("Введите дату рождения водителя: ");

                success = DateTime.TryParse(birthStr, out birthDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                hireStr = Program.ReadLine("Введите дату найма водителя: ");

                success = DateTime.TryParse(hireStr, out hireDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            phone = Program.ReadLine("Введите номер телефона водителя: ");
            CategoryWork(ref category);

            Console.Clear();
            Console.WriteLine("Фамилия: {0}", lastName);
            Console.WriteLine("Имя: {0}", firstName);
            Console.WriteLine("Дата роджения: {0}", birthDate.ToString("dd.mm.yyyy"));
            Console.WriteLine("Дата найма: {0}", hireDate.ToString("dd.mm.yyyy"));
            Console.WriteLine("Номер телефона: {0}", phone);
            Console.WriteLine("Категория: {0}", category.ToString());
            Console.WriteLine("Добавить данного водителя?");
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

            Driver.drivers.Add(new Driver
            {
                id = Driver.drivers.Count > 0 ? Driver.drivers.Last().id + 1 : 0,
                lastName = lastName,
                firstName = firstName,
                birthDate = birthDate,
                hireDate = hireDate,
                phone = phone,
                driveCategory = category,
            });
            return true;
        }

        public static bool Edit()
        {
            table.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID водителя, которого необходимо отредактировать: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Driver d;
            if (!success || (d = Driver.drivers.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Console.Clear();
            Console.WriteLine("В скобках указывается текущее значение.");
            Console.WriteLine("Если необходимо его оставить без изменений, оставьте строку ввода пустой.");
            Console.WriteLine();

            string lastName, firstName, birthStr, hireStr, phone;
            DateTime birthDate = new DateTime();
            DateTime hireDate = new DateTime();
            DriveCategory category = d.driveCategory;

            lastName = Program.ReadLine($"Введите фамилию водителя ({d.lastName}): ", true);
            firstName = Program.ReadLine($"Введите имя водителя ({d.firstName}): ", true);

            do
            {
                birthStr = Program.ReadLine($"Введите дату рождения водителя " +
                    $"({d.birthDate.ToString("dd.MM.yyyy")}): ", true);
                if (string.IsNullOrWhiteSpace(birthStr))
                    break;

                success = DateTime.TryParse(birthStr, out birthDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                hireStr = Program.ReadLine($"Введите дату найма водителя " +
                    $"({d.hireDate.ToString("dd.MM.yyyy")}): ", true);
                if (string.IsNullOrWhiteSpace(hireStr))
                    break;

                success = DateTime.TryParse(hireStr, out hireDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            phone = Program.ReadLine($"Введите номер телефона водителя ({d.phone}): ", true);
            CategoryWork(ref category);

            Console.Clear();
            Console.WriteLine("Фамилия: {0}", string.IsNullOrWhiteSpace(lastName) ? d.lastName : lastName);
            Console.WriteLine("Имя: {0}", string.IsNullOrWhiteSpace(firstName) ? d.firstName : firstName);
            Console.WriteLine("Дата роджения: {0}", string.IsNullOrWhiteSpace(birthStr) ? 
                d.birthDate.ToString("dd.mm.yyyy") : birthDate.ToString("dd.mm.yyyy"));
            Console.WriteLine("Дата найма: {0}", string.IsNullOrWhiteSpace(hireStr) ? 
                d.hireDate.ToString("dd.mm.yyyy") : hireDate.ToString("dd.mm.yyyy"));
            Console.WriteLine("Номер телефона: {0}", string.IsNullOrWhiteSpace(phone) ? d.phone : phone);
            Console.WriteLine("Категория: {0}", category.ToString());
            Console.WriteLine("Сохранить данного водителя?");
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

            if (!string.IsNullOrWhiteSpace(lastName)) d.lastName = lastName;
            if (!string.IsNullOrWhiteSpace(firstName)) d.firstName = firstName;
            if (!string.IsNullOrWhiteSpace(birthStr)) d.birthDate = birthDate;
            if (!string.IsNullOrWhiteSpace(hireStr)) d.hireDate = hireDate;
            if (!string.IsNullOrWhiteSpace(phone)) d.phone = phone;
            d.driveCategory = category;
            
            return true;
        }

        private static void CategoryWork(ref DriveCategory category)
        {
            bool success = true;
            int id;
            string str;
            ConsoleKeyInfo key;

            do
            {
                Console.Clear();
                Console.WriteLine("Текущие категории: {0}", category != DriveCategory.None ? 
                    category.ToString() : "отсутствуют");
                Console.WriteLine();
                if (!success)
                    Console.WriteLine("Ошибка ввода");
                Console.WriteLine("1 - Добавить категорию");
                Console.WriteLine("2 - Удалить категорию");
                if (category != 0)
                    Console.WriteLine("3 - Продолжить");
                key = Console.ReadKey(true);
                if (key.KeyChar == '1')
                {
                    Console.WriteLine("1 - A");
                    Console.WriteLine("2 - B");
                    Console.WriteLine("3 - C");
                    Console.WriteLine("4 - D");
                    Console.WriteLine("5 - E");
                    Console.WriteLine("6 - M");
                    str = Program.ReadLine("Введите категорию, которую необходимо добавить: ");
                    success = int.TryParse(str, out id);
                    if (success)
                    {
                        if (id < 1 || !Enum.IsDefined(typeof(DriveCategory), 1 << (id - 1)))
                        {
                            success = false;
                            continue;
                        }
                        category |= (DriveCategory)(1 << (id - 1));
                    }
                    else
                    {
                        if (!Enum.IsDefined(typeof(DriveCategory), str.ToUpper()))
                        {
                            success = false;
                            continue;
                        }
                        category |= (DriveCategory)Enum.Parse(typeof(DriveCategory), str.ToUpper());
                        success = true;
                    }
                }
                else if (key.KeyChar == '2')
                {
                    Console.WriteLine("{0}", category);
                    str = Program.ReadLine("Введите категорию, которую необходимо удалить: ");
                    if (Enum.IsDefined(typeof(DriveCategory), str))
                        category &= ~(DriveCategory)Enum.Parse(typeof(DriveCategory), str);
                    success = true;
                }
            } while (key.KeyChar != '3' || category == DriveCategory.None);
        }

        public static bool Delete()
        {
            table.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();
            Console.Write("Введите ID водителя, которого необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Driver d;
            if (!success || (d = Driver.drivers.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Route.routes.RemoveAll(c => c.driver.id == d.id);
            RoutesMenu.CreateTable();
            Driver.drivers.Remove(d);
            return true;
        }
    }
}