using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalApp
{
    class Instrument //Класс для работы с инструментами
    {
        public int Ind { get; set; }
        public string Brand { get; set; }
        public string Type { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public Instrument(int ind, string brand, string type, double price, int quantity)
        {
            Ind = ind;
            Brand = brand;
            Type = type;
            Price = price;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"{Brand} {Type}, Цена: {Price} руб., Количество: {Quantity}";
        }
    }

    class FileManager //Для считывания данных из файла в структуру
    {
        public static List<Instrument> ReadInstrumentsFromFile(string fileName)
        {
            List<Instrument> instruments = new List<Instrument>();

            try //Обработчик исключений, в случае ошибки в строке он пропустит её и запишет только верные
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] values = line.Split(';');
                        if (values.Length < 5) //Проверка на формат
                        {
                            Console.WriteLine($"Ошибка: некорректный формат данных в строке '{line}'");
                            continue;
                        }
                        int index = int.Parse(values[0]);
                        string brand = values[1];
                        string type = values[2];
                        double price;
                        if (!double.TryParse(values[3], out price)) //Проверка на цену
                        {
                            Console.WriteLine($"Ошибка: некорректный формат цены в строке '{line}'");
                            continue;
                        }
                        int quantity;
                        if (!int.TryParse(values[4], out quantity)) //Проверка на количество
                        {
                            Console.WriteLine($"Ошибка: некорректный формат количества в строке '{line}'");
                            continue;
                        }

                        instruments.Add(new Instrument(index, brand, type, price, quantity));
                    }
                }
            }
            catch (FileNotFoundException) //Вывод ошибок
            {
                Console.WriteLine($"Ошибка: файл {fileName} не найден.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            return instruments;
        }

        public static void WriteInstrumentsToFile(string fileName, List<Instrument> instruments) //Функция для записи в файл при закрытии приложения
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (Instrument instrument in instruments)
                {
                    sw.WriteLine($"{instrument.Ind};{instrument.Brand};{instrument.Type};{instrument.Price};{instrument.Quantity}");
                }
            }
        }

        public static void BuyInstruments(List<Instrument> selectedInstruments) //Функция для уменьшения количества купленного товара
        {
            foreach (var instrument in selectedInstruments)
            {
                instrument.Quantity--;
            }
        }
    }

    class ProgramClient //Функционал программы для клиента
    {
        public static void DisplayInstruments(List<Instrument> instruments) //Показывает все товары
        {
            Console.WriteLine("Список товаров:");
            Console.WriteLine();

            foreach (Instrument instrument in instruments)
            {
                Console.WriteLine($"{instrument.Brand} ({instrument.Type}), цена: {instrument.Price}, количество: {instrument.Quantity}");
            }
        }

        public static List<Instrument> SelectInstruments(List<Instrument> instruments, int numInstruments) //Выбор инструментов для покупки
        {
            Console.WriteLine("Выберите инструменты:");

            List<Instrument> selectedInstruments = new List<Instrument>();
            int index = 0;
            while (selectedInstruments.Count < numInstruments)
            {
                Console.WriteLine($"Выберите инструмент {selectedInstruments.Count + 1}:");
                foreach (var instrument in instruments)
                {
                    Console.WriteLine($"{++index}. {instrument.ToString()}");
                }

                int choice;
                while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > instruments.Count)
                {
                    Console.WriteLine("Некорректный выбор. Пожалуйста, выберите номер инструмента из списка.");
                }

                var selectedInstrument = instruments[choice - 1];
                if (!selectedInstruments.Contains(selectedInstrument))
                {
                    selectedInstruments.Add(selectedInstrument);
                    index = 0;
                }
                else
                {
                    Console.WriteLine("Инструмент уже выбран. Пожалуйста, выберите другой.");
                    index = 0;
                }
            }

            return selectedInstruments;
        }

        public static double CalculateTotalCost(List<Instrument> selectedInstruments) //Считает сумму выбранных инструментов
        {
            double totalCost = 0;
            foreach (var instrument in selectedInstruments)
            {
                totalCost += instrument.Price;
            }
            return totalCost;
        }
    }
    class ProgramCusomer //Функционал программы для продавца
    {
        public static void ViewInstruments(List<Instrument> instruments) //Функция отображения товара
        {
            Console.WriteLine("Список товаров:");

            foreach (var instrument in instruments)
            {
                Console.WriteLine($"{instrument.Ind}. {instrument.Brand} {instrument.Type} - {instrument.Price} руб. ({instrument.Quantity} шт.)");
            }
        }

        // Функция добавления нового товара
        public static void AddNewInstrument(List<Instrument> instruments)
        {
            int index;
            string brand;
            string type;
            double price;
            int quantity;

            // Запрашиваем индекс товара
            while (true)
            {
                Console.Write("Введите индекс товара: ");
                if (!int.TryParse(Console.ReadLine(), out index))
                {
                    Console.WriteLine("Ошибка: некорректный формат индекса.");
                    continue;
                }
                if (instruments.Any(i => i.Ind == index))
                {
                    Console.WriteLine($"Ошибка: индекс {index} уже существует.");
                    continue;
                }
                break;
            }

            // Запрашиваем бренд товара
            while (true)
            {
                Console.Write("Введите бренд товара: ");
                brand = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(brand))
                {
                    Console.WriteLine("Ошибка: бренд не может быть пустым.");
                    continue;
                }
                break;
            }

            // Запрашиваем вид инструмента
            while (true)
            {
                Console.Write("Введите вид инструмента: ");
                type = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(type))
                {
                    Console.WriteLine("Ошибка: вид инструмента не может быть пустым.");
                    continue;
                }
                break;
            }

            // Запрашиваем цену товара
            while (true)
            {
                Console.Write("Введите цену товара: ");
                if (!double.TryParse(Console.ReadLine(), out price))
                {
                    Console.WriteLine("Ошибка: некорректный формат цены.");
                    continue;
                }
                break;
            }

            // Запрашиваем количество товара
            while (true)
            {
                Console.Write("Введите количество товара: ");
                if (!int.TryParse(Console.ReadLine(), out quantity))
                {
                    Console.WriteLine("Ошибка: некорректный формат количества.");
                    continue;
                }
                break;
            }

            // Показываем информацию о новом товаре и запрашиваем подтверждение
            Console.WriteLine($"Вы добавляете новый товар: {index};{brand};{type};{price};{quantity}");
            Console.Write("Введите Y, чтобы подтвердить, или любой другой символ, чтобы отменить: ");
            if (Console.ReadLine().ToLower() != "y")
            {
                Console.WriteLine("Добавление нового товара отменено.");
                return;
            }

            // Добавляем новый товар в список и сохраняем в файл
            instruments.Add(new Instrument(index, brand, type, price, quantity));
            Console.WriteLine("Новый товар успешно добавлен.");
        }

        public static void EditInstrument(List<Instrument> instruments, int EditIndex)
        {
            var instrument = instruments.FirstOrDefault(i => i.Ind == EditIndex);

            if (instrument != null)
            {
                Console.WriteLine($"Найден товар: {instrument}");
                Console.WriteLine("Введите номер поля, которое нужно изменить (1 - бренд, 2 - вид инструмента, 3 - цена, 4 - количество):");
                int fieldNumber = int.Parse(Console.ReadLine());

                switch (fieldNumber)
                {
                    case 1:
                        Console.Write("Введите новый бренд: ");
                        instrument.Brand = Console.ReadLine();
                        break;
                    case 2:
                        Console.Write("Введите новый вид инструмента: ");
                        instrument.Type = Console.ReadLine();
                        break;
                    case 3:
                        Console.Write("Введите новую цену: ");
                        instrument.Price = double.Parse(Console.ReadLine());
                        break;
                    case 4:
                        Console.Write("Введите новое количество: ");
                        instrument.Quantity = int.Parse(Console.ReadLine());
                        break;
                    default:
                        Console.WriteLine("Ошибка: введен неправильный номер поля.");
                        break;
                }

                Console.WriteLine("Товар успешно изменен:");
                Console.WriteLine(instrument);
            }
            else
            {
                Console.WriteLine("Товар с таким индексом не найден.");
            }
        }

        public static void RemoveInstrument(List<Instrument> instruments)
        {
            Console.Write("Введите индекс товара, который нужно удалить: ");
            int index = int.Parse(Console.ReadLine());

            // поиск товара по индексу
            Instrument productToRemove = instruments.Find(i => i.Ind == index);

            if (productToRemove == null)
            {
                Console.WriteLine("Товар с таким индексом не найден!");
                return;
            }

            // вывод информации о товаре, который будет удален
            Console.WriteLine("Удаляемый товар:");
            Console.Write("Введите Y, чтобы подтвердить, или любой другой символ, чтобы отменить: ");
            if (Console.ReadLine().ToLower() != "y")
            {
                Console.WriteLine("Удаление товара отменено.");
                return;
            }
            Console.WriteLine(productToRemove);

            // удаление товара из списка
            instruments.Remove(productToRemove);

            Console.WriteLine("Товар удален из списка!");
        }

    }

    internal class MainActivity
    {
        static List<Instrument> instruments;

        static void Main(string[] args)
        {
            //Обработчик событий при закрытии программы
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnCancelKeyPress);

            instruments = FileManager.ReadInstrumentsFromFile("instruments.csv");

            // Вывод меню выбора режима работы программы
            Console.WriteLine("Выберите режим работы программы:");
            Console.WriteLine("1 - для работы с программой в режиме клиента");
            Console.WriteLine("2 - для работы с программой в режиме продавца");
            Console.WriteLine("3 - для завершения работы с программой");

            int mode;

            while (true)
            {
                if (!int.TryParse(Console.ReadLine(), out mode) || mode < 1 || mode > 3)
                {
                    Console.WriteLine("Некорректный выбор. Пожалуйста, выберите от 1 до 3.");
                }
                else
                {
                    break;
                }
            }

            // Обработка выбора режима работы программы
            if (mode == 1)
            {
                // Логика программы для клиента
                while (true)
                {
                    Console.WriteLine("Выберите действие:");
                    Console.WriteLine("1. Просмотреть список товаров");
                    Console.WriteLine("2. Купить товар");
                    Console.WriteLine("3. Выйти из программы");

                    int choice;
                    while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3)
                    {
                        Console.WriteLine("Некорректное выбор. Пожалуйста, введите от 1 до 3.");
                    }

                    switch (choice)
                    {
                        case 1:
                            ProgramClient.DisplayInstruments(instruments);
                            break;

                        case 2:
                            Console.WriteLine("Сколько инструментов Вы хотите приобрести?");
                            int numInstruments;
                            while (!int.TryParse(Console.ReadLine(), out numInstruments) || numInstruments <= 0 || numInstruments >= instruments.Count)
                            {
                                Console.WriteLine("Некорректное количество. Пожалуйста, введите положительное целое число.");
                            }

                            List<Instrument> selectedInstruments = ProgramClient.SelectInstruments(instruments, numInstruments);
                            double totalCost = ProgramClient.CalculateTotalCost(selectedInstruments);

                            Console.WriteLine($"Общая сумма заказа: {totalCost} руб.");

                            Console.WriteLine("1 - для оплаты наличными");
                            Console.WriteLine("2 - для оплаты картой");

                            int answer;
                            while (!int.TryParse(Console.ReadLine(), out answer) || answer < 1 || answer > 2)
                            {
                                Console.WriteLine("Некорректное выбор. Пожалуйста, введите от 1 до 2.");
                            }

                            FileManager.BuyInstruments(selectedInstruments);

                            Console.WriteLine($"Оплата в размере {totalCost} успешно произведена");
                            break;

                        case 3:
                            Console.WriteLine("До свидания!");
                            return;
                    }
                }
            }
            else if (mode == 2)
            {
                // Логика программы для продавца
                while (true)
                {
                    Console.WriteLine("Выберите действие:");
                    Console.WriteLine("1. Просмотреть список товаров");
                    Console.WriteLine("2. Добавить новый товар");
                    Console.WriteLine("3. Изменить данные о товаре");
                    Console.WriteLine("4. Удалить товар");
                    Console.WriteLine("5. Выйти из программы");

                    int choice;
                    while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 5)
                    {
                        Console.WriteLine("Некорректное выбор. Пожалуйста, введите от 1 до 5.");
                    }

                    switch (choice)
                    {
                        case 1:
                            ProgramCusomer.ViewInstruments(instruments);
                            break;

                        case 2:
                            ProgramCusomer.AddNewInstrument(instruments);
                            break;

                        case 3:
                            int index;
                            while (true)
                            {
                                Console.Write("Введите индекс товара: ");
                                if (!int.TryParse(Console.ReadLine(), out index))
                                {
                                    Console.WriteLine("Ошибка: некорректный ввод индекса!");
                                    continue;
                                }

                                if (instruments.FirstOrDefault(i => i.Ind == index) == null)
                                {
                                    Console.WriteLine("Ошибка: товар с таким индексом не найден!");
                                    continue;
                                }
                                ProgramCusomer.EditInstrument(instruments, index);
                                break;
                            }
                            break;

                        case 4:
                            ProgramCusomer.RemoveInstrument(instruments);
                            break;

                        case 5:
                            Console.WriteLine("До свидания!");
                            return;
                    }
                }
            }
            else if (mode == 3)
            {
                Console.WriteLine("До свидания!");
                Console.ReadKey();
            }
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            FileManager.WriteInstrumentsToFile("instruments.csv", instruments);
        }

        static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            FileManager.WriteInstrumentsToFile("instruments.csv", instruments);
        }
    }
}