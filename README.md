using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Перечисления
public enum PlanetType { Rocky, Gas, Ice, Ocean, Desert }
public enum ResourceType { Water, Minerals, Energy, Organic, Rare }
public enum MissionStatus { Planning, InProgress, Success, Failed, Critical }

// Исключения
public class MissionCriticalException : Exception
{
    public string ErrorCode { get; }
    
    public MissionCriticalException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
    
    public override string Message => $"[КРИТИЧЕСКАЯ ОШИБКА {ErrorCode}] {base.Message}";
}

public class ResourceDepletedException : Exception
{
    public string ResourceName { get; }
    
    public ResourceDepletedException(string resourceName) 
        : base($"Ресурс {resourceName} полностью исчерпан")
    {
        ResourceName = resourceName;
    }
    
    public override string Message => $"[ДЕФИЦИТ РЕСУРСА] {base.Message}";
}

// Интерфейсы
public interface IResearchable
{
    void ConductResearch();
    string GetResearchData();
}

public interface IRepairable
{
    bool Repair(int skillLevel);
    int Durability { get; }
}

// Запись (record)
public record Coordinates(double X, double Y, double Z)
{
    public double DistanceTo(Coordinates other)
    {
        return Math.Sqrt(Math.Pow(X - other.X, 2) + 
                        Math.Pow(Y - other.Y, 2) + 
                        Math.Pow(Z - other.Z, 2));
    }
}

// Базовый абстрактный класс
public abstract class Astronaut : IEquatable<Astronaut>
{
    public string Name { get; protected set; }
    public int Health { get; protected set; }
    public int SkillLevel { get; protected set; }
    public Coordinates Position { get; protected set; }
    
    protected Astronaut(string name, int health, int skillLevel, Coordinates position)
    {
        Name = name;
        Health = health;
        SkillLevel = skillLevel;
        Position = position;
    }
    
    // Абстрактный метод
    public abstract void PerformDuty();
    
    public virtual void TakeDamage(int damage)
    {
        Health = Math.Max(0, Health - damage);
        Console.WriteLine($"{Name} получает урон {damage}. Здоровье: {Health}");
    }
    
    public virtual void Heal(int amount)
    {
        Health = Math.Min(100, Health + amount);
        Console.WriteLine($"{Name} восстанавливает {amount} здоровья. Здоровье: {Health}");
    }
    
    // Переопределение методов Object
    public override bool Equals(object obj)
    {
        return Equals(obj as Astronaut);
    }
    
    public bool Equals(Astronaut other)
    {
        return other != null && Name == other.Name && Health == other.Health;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Health);
    }
    
    public override string ToString()
    {
        return $"{GetType().Name} {Name} [Здоровье: {Health}, Навык: {SkillLevel}]";
    }
}

// Конкретные классы астронавтов
public class Researcher : Astronaut, IResearchable
{
    public int ResearchPoints { get; private set; }
    
    public Researcher(string name, Coordinates position) 
        : base(name, 100, 85, position)
    {
        ResearchPoints = 0;
    }
    
    public override void PerformDuty()
    {
        Console.WriteLine($"{Name} проводит научные исследования...");
        ResearchPoints += 10;
    }
    
    public void ConductResearch()
    {
        var random = new Random();
        int discoveryChance = random.Next(1, 101);
        
        if (discoveryChance > 70)
        {
            ResearchPoints += 25;
            Console.WriteLine($"{Name} совершает важное научное открытие! +25 очков исследований");
        }
        else
        {
            ResearchPoints += 10;
            Console.WriteLine($"{Name} проводит стандартные исследования. +10 очков исследований");
        }
    }
    
    public string GetResearchData()
    {
        return $"Исследователь {Name} собрал {ResearchPoints} очков данных";
    }
}

public class Pilot : Astronaut
{
    public int FlightHours { get; private set; }
    
    public Pilot(string name, Coordinates position) 
        : base(name, 100, 90, position)
    {
        FlightHours = 0;
    }
    
    public override void PerformDuty()
    {
        Console.WriteLine($"{Name} управляет космическим аппаратом...");
        FlightHours += 1;
    }
    
    public void NavigateTo(Coordinates target)
    {
        double distance = Position.DistanceTo(target);
        Console.WriteLine($"{Name} прокладывает курс на расстояние {distance:F2} световых лет");
        Position = target;
    }
}

public class Engineer : Astronaut, IRepairable
{
    public int RepairSkill { get; private set; }
    public int Durability { get; private set; } = 100;
    
    public Engineer(string name, Coordinates position) 
        : base(name, 100, 80, position)
    {
        RepairSkill = 75;
    }
    
    public override void PerformDuty()
    {
        Console.WriteLine($"{Name} проводит техническое обслуживание...");
        RepairSkill += 1;
    }
    
    public bool Repair(int skillLevel)
    {
        var random = new Random();
        int repairChance = random.Next(1, 101);
        
        if (repairChance <= skillLevel)
        {
            Durability = Math.Min(100, Durability + 30);
            Console.WriteLine($"{Name} успешно починил оборудование! Прочность: {Durability}");
            return true;
        }
        else
        {
            Console.WriteLine($"{Name} не удалось починить оборудование");
            return false;
        }
    }
}

// Класс космического корабля
public class Spacecraft : IRepairable, IEquatable<Spacecraft>
{
    public string Name { get; private set; }
    public int Durability { get; private set; }
    public int Fuel { get; private set; }
    public Coordinates Position { get; private set; }
    public ArrayList Crew { get; private set; }
    
    public Spacecraft(string name, Coordinates position)
    {
        Name = name;
        Durability = 100;
        Fuel = 100;
        Position = position;
        Crew = new ArrayList();
    }
    
    public void AddCrewMember(Astronaut astronaut)
    {
        Crew.Add(astronaut);
        Console.WriteLine($"{astronaut.Name} присоединился к экипажу {Name}");
    }
    
    public bool Launch(int fuelCost)
    {
        if (Fuel < fuelCost)
        {
            throw new ResourceDepletedException("топливо");
        }
        
        Fuel -= fuelCost;
        Console.WriteLine($"{Name} запускается! Расход топлива: {fuelCost}. Остаток: {Fuel}");
        return true;
    }
    
    public bool Repair(int skillLevel)
    {
        var random = new Random();
        if (random.Next(1, 101) <= skillLevel)
        {
            Durability = Math.Min(100, Durability + 25);
            Console.WriteLine($"{Name} отремонтирован! Прочность: {Durability}");
            return true;
        }
        return false;
    }
    
    public void TakeDamage(int damage)
    {
        Durability = Math.Max(0, Durability - damage);
        Console.WriteLine($"{Name} получает повреждения {damage}. Прочность: {Durability}");
        
        if (Durability <= 0)
        {
            throw new MissionCriticalException("Корабль уничтожен!", "SHIP_DESTROYED");
        }
    }
    
    // Переопределение методов Object
    public override bool Equals(object obj)
    {
        return Equals(obj as Spacecraft);
    }
    
    public bool Equals(Spacecraft other)
    {
        return other != null && Name == other.Name && Durability == other.Durability;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Durability);
    }
    
    public override string ToString()
    {
        return $"Корабль {Name} [Прочность: {Durability}, Топливо: {Fuel}, Экипаж: {Crew.Count}]";
    }
}

// Класс планеты
public class Planet : IResearchable
{
    public string Name { get; private set; }
    public PlanetType Type { get; private set; }
    public Coordinates Coordinates { get; private set; }
    public List<Resource> Resources { get; private set; }
    public bool HasAlienLife { get; private set; }
    
    public Planet(string name, PlanetType type, Coordinates coords, bool hasAlienLife = false)
    {
        Name = name;
        Type = type;
        Coordinates = coords;
        HasAlienLife = hasAlienLife;
        Resources = new List<Resource>();
        InitializeResources();
    }
    
    private void InitializeResources()
    {
        var random = new Random();
        var resourceTypes = Enum.GetValues(typeof(ResourceType));
        
        foreach (ResourceType type in resourceTypes)
        {
            int amount = random.Next(20, 100);
            Resources.Add(new Resource(type, amount));
        }
    }
    
    public void ConductResearch()
    {
        Console.WriteLine($"Проводятся исследования планеты {Name}...");
        
        if (HasAlienLife)
        {
            Console.WriteLine($"Обнаружена инопланетная жизнь на {Name}!");
        }
    }
    
    public string GetResearchData()
    {
        return $"Планета {Name}, Тип: {Type}, Ресурсы: {Resources.Count} видов";
    }
    
    public Resource ExtractResource(ResourceType type)
    {
        var resource = Resources.Find(r => r.Type == type);
        if (resource != null && resource.Quantity > 0)
        {
            resource.Quantity -= 10;
            if (resource.Quantity <= 0)
            {
                throw new ResourceDepletedException(type.ToString());
            }
            return new Resource(type, 10);
        }
        throw new ResourceDepletedException(type.ToString());
    }
    
    public override string ToString()
    {
        return $"Планета {Name} ({Type})";
    }
}

// Класс ресурса
public class Resource
{
    public ResourceType Type { get; set; }
    public int Quantity { get; set; }
    
    public Resource(ResourceType type, int quantity)
    {
        Type = type;
        Quantity = quantity;
    }
    
    public override string ToString()
    {
        return $"{Type}: {Quantity} единиц";
    }
}

// Класс управления миссией
public class MissionControl
{
    public string MissionName { get; private set; }
    public MissionStatus Status { get; private set; }
    public List<Astronaut> Team { get; private set; }
    public Spacecraft Ship { get; private set; }
    
    public MissionControl(string missionName, Spacecraft ship)
    {
        MissionName = missionName;
        Status = MissionStatus.Planning;
        Team = new List<Astronaut>();
        Ship = ship;
    }
    
    public void AddToTeam(Astronaut astronaut)
    {
        Team.Add(astronaut);
        Ship.AddCrewMember(astronaut);
    }
    
    public void StartMission()
    {
        Status = MissionStatus.InProgress;
        Console.WriteLine($"\n=== МИССИЯ '{MissionName}' НАЧАЛАСЬ ===\n");
    }
    
    public void CompleteMission(bool success)
    {
        Status = success ? MissionStatus.Success : MissionStatus.Failed;
        Console.WriteLine($"\n=== МИССИЯ {(success ? "УСПЕШНО ЗАВЕРШЕНА" : "ПРОВАЛЕНА")} ===\n");
    }
    
    public void SimulateMissionDay()
    {
        var random = new Random();
        Console.WriteLine($"\n--- День миссии {MissionName} ---");
        
        // Астронавты выполняют свои обязанности
        foreach (var astronaut in Team)
        {
            astronaut.PerformDuty();
            
            // Случайные события
            if (random.Next(1, 101) > 80)
            {
                astronaut.TakeDamage(random.Next(5, 20));
            }
        }
        
        // Использование корабля
        try
        {
            Ship.Launch(random.Next(5, 15));
        }
        catch (ResourceDepletedException ex)
        {
            Console.WriteLine($"ПРЕДУПРЕЖДЕНИЕ: {ex.Message}");
            Status = MissionStatus.Critical;
        }
        
        // Случайные повреждения корабля
        if (random.Next(1, 101) > 85)
        {
            try
            {
                Ship.TakeDamage(random.Next(10, 30));
            }
            catch (MissionCriticalException ex)
            {
                Console.WriteLine($"КАТАСТРОФА: {ex.Message}");
                Status = MissionStatus.Failed;
                return;
            }
        }
    }
}

// Главный класс программы
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== СИМУЛЯТОР КОСМИЧЕСКОЙ ЭКСПЕДИЦИИ ===\n");
        
        try
        {
            // Создание объектов
            var ship = new Spacecraft("Галактик-1", new Coordinates(0, 0, 0));
            var mission = new MissionControl("Исследование Ксенона", ship);
            
            // Создание астронавтов
            var researcher = new Researcher("Доктор Смит", new Coordinates(0, 0, 0));
            var pilot = new Pilot("Капитан Иванов", new Coordinates(0, 0, 0));
            var engineer = new Engineer("Инженер Петров", new Coordinates(0, 0, 0));
            
            // Добавление в команду
            mission.AddToTeam(researcher);
            mission.AddToTeam(pilot);
            mission.AddToTeam(engineer);
            
            // Создание планет
            var targetPlanet = new Planet("Ксенон-Prime", PlanetType.Rocky, 
                new Coordinates(150, 75, 200), true);
            
            // Демонстрация использования локального класса
            DemonstrateLocalClass();
            
            // Запуск миссии
            mission.StartMission();
            
            // Симуляция дней миссии
            for (int day = 1; day <= 5 && mission.Status == MissionStatus.InProgress; day++)
            {
                mission.SimulateMissionDay();
                
                // Демонстрация полиморфизма
                DemonstratePolymorphism(researcher, engineer);
                
                // Демонстрация работы с ресурсами
                if (day == 3)
                {
                    DemonstrateResourceExtraction(targetPlanet);
                }
            }
            
            // Завершение миссии
            mission.CompleteMission(mission.Status != MissionStatus.Failed);
            
            // Вывод итогов
            PrintMissionSummary(mission, researcher, pilot, engineer, ship);
        }
        catch (MissionCriticalException ex)
        {
            Console.WriteLine($"\n💀 МИССИЯ ПРЕРВАНА: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n⚠️ Непредвиденная ошибка: {ex.Message}");
        }
        
        // Демонстрация непроверяемого исключения
        DemonstrateUncheckedException();
    }
    
    // Локальный метод для демонстрации полиморфизма
    static void DemonstratePolymorphism(params Astronaut[] astronauts)
    {
        Console.WriteLine("\n--- Демонстрация полиморфизма ---");
        foreach (var astronaut in astronauts)
        {
            Console.WriteLine(astronaut.ToString());
        }
    }
    
    // Демонстрация работы с ресурсами
    static void DemonstrateResourceExtraction(Planet planet)
    {
        Console.WriteLine($"\n--- Добыча ресурсов на {planet.Name} ---");
        try
        {
            var water = planet.ExtractResource(ResourceType.Water);
            Console.WriteLine($"Добыто: {water}");
            
            // Попытка добыть несуществующий ресурс
            for (int i = 0; i < 15; i++)
            {
                planet.ExtractResource(ResourceType.Rare);
            }
        }
        catch (ResourceDepletedException ex)
        {
            Console.WriteLine($"Ошибка добычи: {ex.Message}");
        }
    }
    
    // Демонстрация локального класса
    static void DemonstrateLocalClass()
    {
        // Локальный класс для экстренного протокола
        class EmergencyProtocol
        {
            public string Code { get; set; }
            public string Description { get; set; }
            
            public void Execute()
            {
                Console.WriteLine($"Активирован протокол {Code}: {Description}");
            }
        }
        
        var protocol = new EmergencyProtocol 
        { 
            Code = "RED-ALPHA", 
            Description = "Экстренная эвакуация" 
        };
        protocol.Execute();
    }
    
    // Демонстрация непроверяемого исключения
    static void DemonstrateUncheckedException()
    {
        Console.WriteLine("\n--- Демонстрация обработки исключений ---");
        try
        {
            // Намеренное деление на ноль
            int zero = 0;
            int result = 100 / zero;
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine($"Перехвачено непроверяемое исключение: {ex.Message}");
        }
    }
    
    // Вывод итогов миссии
    static void PrintMissionSummary(MissionControl mission, params Astronaut[] astronauts)
    {
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("ИТОГИ МИССИИ");
        Console.WriteLine(new string('=', 50));
        
        Console.WriteLine($"Миссия: {mission.MissionName}");
        Console.WriteLine($"Статус: {mission.Status}");
        Console.WriteLine($"Корабль: {mission.Ship}");
        
        Console.WriteLine("\nСостав экипажа:");
        foreach (var astronaut in astronauts)
        {
            Console.WriteLine($"  - {astronaut}");
        }
        
        // Демонстрация использования исследователя как IResearchable
        var researcher = astronauts.OfType<Researcher>().FirstOrDefault();
        if (researcher != null)
        {
            Console.WriteLine($"\nНаучные достижения: {researcher.GetResearchData()}");
        }
    }
}
