using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhotoStudioManagement
{
    // Абстрактный класс Person
    public abstract class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public virtual string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }
    }

    // Интерфейс для сущностей с ID
    public interface IIdentifiable
    {
        int Id { get; set; }
    }

    // Класс Клиент
    public class Client : Person, IIdentifiable
    {
        public int Id { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int SessionsCount { get; set; }

        public Client()
        {
            RegistrationDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Клиент #{Id}: {GetFullName()} | Телефон: {Phone} | Сессий: {SessionsCount}";
        }
    }

    // Класс Фотограф
    public class Photographer : Person, IIdentifiable
    {
        public int Id { get; set; }
        public string Specialization { get; set; }
        public decimal HourlyRate { get; set; }
        public int ExperienceYears { get; set; }

        public override string ToString()
        {
            return $"Фотограф #{Id}: {GetFullName()} | Специализация: {Specialization} | Стаж: {ExperienceYears} лет";
        }
    }

    // Перечисление типов оборудования
    public enum EquipmentType
    {
        Camera,
        Lens,
        Lighting,
        Background,
        Other
    }

    // Класс Оборудование
    public class Equipment : IIdentifiable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EquipmentType Type { get; set; }
        public string Model { get; set; }
        public decimal RentalPrice { get; set; }
        public bool IsAvailable { get; set; }
        public string Condition { get; set; }

        public Equipment()
        {
            IsAvailable = true;
            Condition = "Отличное";
        }

        public override string ToString()
        {
            string availability = IsAvailable ? "Доступно" : "Занято";
            return $"{Type}: {Name} ({Model}) | Цена: {RentalPrice} руб./час | Состояние: {Condition} | {availability}";
        }
    }

    // Перечисление статусов фотосессии
    public enum SessionStatus
    {
        Planned,
        InProgress,
        Completed,
        Cancelled
    }

    // Класс Фотосессия
    public class PhotoSession : IIdentifiable
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int PhotographerId { get; set; }
        public List<int> EquipmentIds { get; set; }
        public DateTime SessionDate { get; set; }
        public int DurationHours { get; set; }
        public string SessionType { get; set; }
        public string Location { get; set; }
        public SessionStatus Status { get; set; }
        public decimal TotalCost { get; set; }

        public PhotoSession()
        {
            EquipmentIds = new List<int>();
            Status = SessionStatus.Planned;
        }

        public void CalculateTotalCost(decimal photographerRate, List<Equipment> equipment)
        {
            decimal equipmentCost = 0;
            foreach (var equip in equipment)
            {
                if (EquipmentIds.Contains(equip.Id))
                {
                    equipmentCost += equip.RentalPrice * DurationHours;
                }
            }
            TotalCost = (photographerRate * DurationHours) + equipmentCost;
        }

        public override string ToString()
        {
            return $"Сессия #{Id}: {SessionType} | Дата: {SessionDate} | Длительность: {DurationHours}ч | Статус: {Status} | Стоимость: {TotalCost} руб.";
        }
    }

    // Главный класс управления студией
    public class StudioManager
    {
        private List<Client> clients;
        private List<Photographer> photographers;
        private List<Equipment> equipment;
        private List<PhotoSession> sessions;
        private int nextClientId = 1;
        private int nextPhotographerId = 1;
        private int nextEquipmentId = 1;
        private int nextSessionId = 1;

        public StudioManager()
        {
            clients = new List<Client>();
            photographers = new List<Photographer>();
            equipment = new List<Equipment>();
            sessions = new List<PhotoSession>();
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            // Добавляем примеры данных
            clients.Add(new Client { Id = nextClientId++, FirstName = "Анна", LastName = "Иванова", Phone = "+79161234567", Email = "anna@mail.ru" });
            clients.Add(new Client { Id = nextClientId++, FirstName = "Петр", LastName = "Сидоров", Phone = "+79167654321", Email = "petr@mail.ru" });

            photographers.Add(new Photographer { Id = nextPhotographerId++, FirstName = "Мария", LastName = "Петрова", Specialization = "Портретная съемка", HourlyRate = 2000, ExperienceYears = 5 });
            photographers.Add(new Photographer { Id = nextPhotographerId++, FirstName = "Алексей", LastName = "Кузнецов", Specialization = "Свадебная фотография", HourlyRate = 3000, ExperienceYears = 8 });

            equipment.Add(new Equipment { Id = nextEquipmentId++, Name = "Canon EOS R5", Type = EquipmentType.Camera, Model = "EOS R5", RentalPrice = 1500 });
            equipment.Add(new Equipment { Id = nextEquipmentId++, Name = "Sony A7III", Type = EquipmentType.Camera, Model = "A7III", RentalPrice = 1200 });
            equipment.Add(new Equipment { Id = nextEquipmentId++, Name = "Объект 85mm", Type = EquipmentType.Lens, Model = "85mm f/1.8", RentalPrice = 500 });
        }

        // Методы для работы с клиентами
        public void AddClient(Client client)
        {
            try
            {
                client.Id = nextClientId++;
                clients.Add(client);
                Console.WriteLine("Клиент успешно добавлен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении клиента: {ex.Message}");
            }
        }

        public List<Client> GetAllClients() => clients;
        public Client FindClientById(int id) => clients.Find(c => c.Id == id);

        // Методы для работы с фотографами
        public void AddPhotographer(Photographer photographer)
        {
            try
            {
                photographer.Id = nextPhotographerId++;
                photographers.Add(photographer);
                Console.WriteLine("Фотограф успешно добавлен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении фотографа: {ex.Message}");
            }
        }

        public List<Photographer> GetAllPhotographers() => photographers;

        // Методы для работы с оборудованием
        public void AddEquipment(Equipment newEquipment)
        {
            try
            {
                newEquipment.Id = nextEquipmentId++;
                equipment.Add(newEquipment);
                Console.WriteLine("Оборудование успешно добавлено!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении оборудования: {ex.Message}");
            }
        }

        public List<Equipment> GetAllEquipment() => equipment;
        public List<Equipment> GetAvailableEquipment() => equipment.FindAll(e => e.IsAvailable);

        // Методы для работы с фотосессиями
        public void AddPhotoSession(PhotoSession session)
        {
            try
            {
                session.Id = nextSessionId++;
                
                // Проверяем доступность оборудования
                foreach (int equipId in session.EquipmentIds)
                {
                    var equip = equipment.Find(e => e.Id == equipId);
                    if (equip != null && !equip.IsAvailable)
                    {
                        throw new Exception($"Оборудование {equip.Name} уже занято");
                    }
                }

                // Резервируем оборудование
                foreach (int equipId in session.EquipmentIds)
                {
                    var equip = equipment.Find(e => e.Id == equipId);
                    if (equip != null)
                    {
                        equip.IsAvailable = false;
                    }
                }

                // Находим фотографа для расчета стоимости
                var photographer = photographers.Find(p => p.Id == session.PhotographerId);
                var sessionEquipment = equipment.FindAll(e => session.EquipmentIds.Contains(e.Id));
                
                session.CalculateTotalCost(photographer?.HourlyRate ?? 0, sessionEquipment);
                sessions.Add(session);

                // Увеличиваем счетчик сессий клиента
                var client = clients.Find(c => c.Id == session.ClientId);
                if (client != null)
                {
                    client.SessionsCount++;
                }

                Console.WriteLine("Фотосессия успешно забронирована!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при бронировании фотосессии: {ex.Message}");
            }
        }

        public List<PhotoSession> GetAllSessions() => sessions;

        public void CompleteSession(int sessionId)
        {
            try
            {
                var session = sessions.Find(s => s.Id == sessionId);
                if (session != null)
                {
                    session.Status = SessionStatus.Completed;
                    
                    // Освобождаем оборудование
                    foreach (int equipId in session.EquipmentIds)
                    {
                        var equip = equipment.Find(e => e.Id == equipId);
                        if (equip != null)
                        {
                            equip.IsAvailable = true;
                        }
                    }
                    Console.WriteLine("Сессия завершена, оборудование освобождено.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при завершении сессии: {ex.Message}");
            }
        }

        // Поисковые методы
        public List<PhotoSession> FindSessionsByClient(int clientId) => 
            sessions.FindAll(s => s.ClientId == clientId);

        public List<PhotoSession> FindSessionsByPhotographer(int photographerId) => 
            sessions.FindAll(s => s.PhotographerId == photographerId);

        // Сериализация данных
        public void SaveData(string filename)
        {
            try
            {
                var data = new
                {
                    Clients = clients,
                    Photographers = photographers,
                    Equipment = equipment,
                    Sessions = sessions,
                    NextIds = new { nextClientId, nextPhotographerId, nextEquipmentId, nextSessionId }
                };

                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filename, json);
                Console.WriteLine("Данные успешно сохранены!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        public void LoadData(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                {
                    Console.WriteLine("Файл с данными не найден.");
                    return;
                }

                string json = File.ReadAllText(filename);
                var data = JsonSerializer.Deserialize<StudioData>(json);

                clients = data.Clients;
                photographers = data.Photographers;
                equipment = data.Equipment;
                sessions = data.Sessions;
                nextClientId = data.NextIds.nextClientId;
                nextPhotographerId = data.NextIds.nextPhotographerId;
                nextEquipmentId = data.NextIds.nextEquipmentId;
                nextSessionId = data.NextIds.nextSessionId;

                Console.WriteLine("Данные успешно загружены!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
            }
        }
    }

    // Класс для десериализации
    public class StudioData
    {
        public List<Client> Clients { get; set; }
        public List<Photographer> Photographers { get; set; }
        public List<Equipment> Equipment { get; set; }
        public List<PhotoSession> Sessions { get; set; }
        public NextIds NextIds { get; set; }
    }

    public class NextIds
    {
        public int nextClientId { get; set; }
        public int nextPhotographerId { get; set; }
        public int nextEquipmentId { get; set; }
        public int nextSessionId { get; set; }
    }

    // Класс пользовательского интерфейса
    public class StudioUI
    {
        private StudioManager manager;

        public StudioUI()
        {
            manager = new StudioManager();
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== СИСТЕМА УПРАВЛЕНИЯ ФОТОСТУДИЕЙ ===");
                Console.WriteLine("1. Управление клиентами");
                Console.WriteLine("2. Управление фотографами");
                Console.WriteLine("3. Управление оборудованием");
                Console.WriteLine("4. Управление фотосессиями");
                Console.WriteLine("5. Поиск и отчеты");
                Console.WriteLine("6. Сохранить данные");
                Console.WriteLine("7. Загрузить данные");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите пункт меню: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": ManageClients(); break;
                    case "2": ManagePhotographers(); break;
                    case "3": ManageEquipment(); break;
                    case "4": ManageSessions(); break;
                    case "5": SearchAndReports(); break;
                    case "6": manager.SaveData("studio_data.json"); break;
                    case "7": manager.LoadData("studio_data.json"); break;
                    case "0": return;
                    default: Console.WriteLine("Неверный выбор!"); break;
                }
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        private void ManageClients()
        {
            Console.Clear();
            Console.WriteLine("=== УПРАВЛЕНИЕ КЛИЕНТАМИ ===");
            Console.WriteLine("1. Добавить клиента");
            Console.WriteLine("2. Просмотреть всех клиентов");
            Console.WriteLine("3. Найти клиента по ID");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    var client = new Client();
                    Console.Write("Имя: "); client.FirstName = Console.ReadLine();
                    Console.Write("Фамилия: "); client.LastName = Console.ReadLine();
                    Console.Write("Телефон: "); client.Phone = Console.ReadLine();
                    Console.Write("Email: "); client.Email = Console.ReadLine();
                    manager.AddClient(client);
                    break;

                case "2":
                    Console.WriteLine("\nСписок клиентов:");
                    foreach (var c in manager.GetAllClients())
                    {
                        Console.WriteLine(c);
                    }
                    break;

                case "3":
                    Console.Write("Введите ID клиента: ");
                    if (int.TryParse(Console.ReadLine(), out int clientId))
                    {
                        var foundClient = manager.FindClientById(clientId);
                        if (foundClient != null)
                            Console.WriteLine(foundClient);
                        else
                            Console.WriteLine("Клиент не найден.");
                    }
                    break;
            }
        }

        private void ManagePhotographers()
        {
            Console.Clear();
            Console.WriteLine("=== УПРАВЛЕНИЕ ФОТОГРАФАМИ ===");
            Console.WriteLine("1. Добавить фотографа");
            Console.WriteLine("2. Просмотреть всех фотографов");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    var photographer = new Photographer();
                    Console.Write("Имя: "); photographer.FirstName = Console.ReadLine();
                    Console.Write("Фамилия: "); photographer.LastName = Console.ReadLine();
                    Console.Write("Телефон: "); photographer.Phone = Console.ReadLine();
                    Console.Write("Email: "); photographer.Email = Console.ReadLine();
                    Console.Write("Специализация: "); photographer.Specialization = Console.ReadLine();
                    Console.Write("Стаж (лет): "); photographer.ExperienceYears = int.Parse(Console.ReadLine());
                    Console.Write("Ставка в час: "); photographer.HourlyRate = decimal.Parse(Console.ReadLine());
                    manager.AddPhotographer(photographer);
                    break;

                case "2":
                    Console.WriteLine("\nСписок фотографов:");
                    foreach (var p in manager.GetAllPhotographers())
                    {
                        Console.WriteLine(p);
                    }
                    break;
            }
        }

        private void ManageEquipment()
        {
            Console.Clear();
            Console.WriteLine("=== УПРАВЛЕНИЕ ОБОРУДОВАНИЕМ ===");
            Console.WriteLine("1. Добавить оборудование");
            Console.WriteLine("2. Просмотреть все оборудование");
            Console.WriteLine("3. Просмотреть доступное оборудование");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    var equipment = new Equipment();
                    Console.Write("Название: "); equipment.Name = Console.ReadLine();
                    Console.Write("Тип (0-Camera, 1-Lens, 2-Lighting, 3-Background, 4-Other): ");
                    equipment.Type = (EquipmentType)int.Parse(Console.ReadLine());
                    Console.Write("Модель: "); equipment.Model = Console.ReadLine();
                    Console.Write("Цена аренды в час: "); equipment.RentalPrice = decimal.Parse(Console.ReadLine());
                    Console.Write("Состояние: "); equipment.Condition = Console.ReadLine();
                    manager.AddEquipment(equipment);
                    break;

                case "2":
                    Console.WriteLine("\nВсе оборудование:");
                    foreach (var e in manager.GetAllEquipment())
                    {
                        Console.WriteLine(e);
                    }
                    break;

                case "3":
                    Console.WriteLine("\nДоступное оборудование:");
                    foreach (var e in manager.GetAvailableEquipment())
                    {
                        Console.WriteLine(e);
                    }
                    break;
            }
        }

        private void ManageSessions()
        {
            Console.Clear();
            Console.WriteLine("=== УПРАВЛЕНИЕ ФОТОСЕССИЯМИ ===");
            Console.WriteLine("1. Забронировать фотосессию");
            Console.WriteLine("2. Просмотреть все фотосессии");
            Console.WriteLine("3. Завершить фотосессию");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    var session = new PhotoSession();
                    Console.Write("ID клиента: "); session.ClientId = int.Parse(Console.ReadLine());
                    Console.Write("ID фотографа: "); session.PhotographerId = int.Parse(Console.ReadLine());
                    Console.Write("Тип сессии: "); session.SessionType = Console.ReadLine();
                    Console.Write("Место проведения: "); session.Location = Console.ReadLine();
                    Console.Write("Дата (гггг-мм-дд): "); session.SessionDate = DateTime.Parse(Console.ReadLine());
                    Console.Write("Длительность (часов): "); session.DurationHours = int.Parse(Console.ReadLine());

                    Console.WriteLine("Доступное оборудование:");
                    var availableEquipment = manager.GetAvailableEquipment();
                    for (int i = 0; i < availableEquipment.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {availableEquipment[i]}");
                    }

                    Console.Write("Введите номера оборудования через запятую: ");
                    string[] equipNumbers = Console.ReadLine().Split(',');
                    foreach (string num in equipNumbers)
                    {
                        if (int.TryParse(num.Trim(), out int index) && index > 0 && index <= availableEquipment.Count)
                        {
                            session.EquipmentIds.Add(availableEquipment[index - 1].Id);
                        }
                    }

                    manager.AddPhotoSession(session);
                    break;

                case "2":
                    Console.WriteLine("\nВсе фотосессии:");
                    foreach (var s in manager.GetAllSessions())
                    {
                        Console.WriteLine(s);
                    }
                    break;

                case "3":
                    Console.Write("Введите ID сессии для завершения: ");
                    if (int.TryParse(Console.ReadLine(), out int sessionId))
                    {
                        manager.CompleteSession(sessionId);
                    }
                    break;
            }
        }

        private void SearchAndReports()
        {
            Console.Clear();
            Console.WriteLine("=== ПОИСК И ОТЧЕТЫ ===");
            Console.WriteLine("1. Найти сессии клиента");
            Console.WriteLine("2. Найти сессии фотографа");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Введите ID клиента: ");
                    if (int.TryParse(Console.ReadLine(), out int clientId))
                    {
                        var clientSessions = manager.FindSessionsByClient(clientId);
                        Console.WriteLine($"\nСессии клиента #{clientId}:");
                        foreach (var session in clientSessions)
                        {
                            Console.WriteLine(session);
                        }
                    }
                    break;

                case "2":
                    Console.Write("Введите ID фотографа: ");
                    if (int.TryParse(Console.ReadLine(), out int photographerId))
                    {
                        var photographerSessions = manager.FindSessionsByPhotographer(photographerId);
                        Console.WriteLine($"\nСессии фотографа #{photographerId}:");
                        foreach (var session in photographerSessions)
                        {
                            Console.WriteLine(session);
                        }
                    }
                    break;
            }
        }
    }

    // Главный класс программы
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                StudioUI ui = new StudioUI();
                ui.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
            }
            finally
            {
                Console.WriteLine("Программа завершена.");
            }
        }
    }
}
