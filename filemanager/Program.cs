using Newtonsoft.Json;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

public class Car
{
    public string Model { get; set; }
    public double Cost { get; set; }
    public int HorsePower { get; set; }

    public string ToText()
    {
        return $"{Model}\n{Cost}\n{HorsePower}";
    }
}

public class FileManager
{
    private string FileLocation;
    public List<Car> Cars = new List<Car>();

    public void ReadFile()
    {

        Console.WriteLine("Введите путь к файлу:");
        FileLocation = Console.ReadLine();

        if (File.Exists(FileLocation))
        {
            string fileExtension = Path.GetExtension(FileLocation);

            if (fileExtension == ".txt")
            {
                string[] lines = File.ReadAllLines(FileLocation);
                for (int i = 0; i < lines.Length; i += 3)
                {
                    if (i + 2 < lines.Length)
                    {
                        Car Car = new Car
                        {
                            Model = lines[i],
                        };

                        if (double.TryParse(lines[i + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out double Cost))
                        {
                            Car.Cost = Cost;
                        }
                        else
                        {
                            Console.WriteLine("Тип данных не подходит под тип данных в моделе.");
                        }

                        if (int.TryParse(lines[i + 2], NumberStyles.Any, CultureInfo.InvariantCulture, out int HorsePower))
                        {
                            Car.HorsePower = HorsePower;
                        }
                        else
                        {
                            Console.WriteLine("Тип данных не подходит под тип данных в моделе.");
                        }

                        Cars.Add(Car);
                    }
                }

                Console.Clear();
                Console.WriteLine("Содержимое файла:");
                Console.WriteLine(string.Join(Environment.NewLine, lines));
            }
            else if (fileExtension == ".json" || fileExtension == ".xml")
            {
                if (fileExtension == ".json")
                {
                    string jsonContent = File.ReadAllText(FileLocation);
                    Cars = JsonConvert.DeserializeObject<List<Car>>(jsonContent);
                }
                else if (fileExtension == ".xml")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Car>));
                    using (FileStream fs = new FileStream(FileLocation, FileMode.Open))
                    {
                        Cars = (List<Car>)serializer.Deserialize(fs);
                    }
                }

                Console.WriteLine("Содержимое файла:");
                foreach (var car in Cars)
                {
                    Console.WriteLine(car.ToText());
                }
            }
            else
            {
                Console.WriteLine("Неверное расширение файла.");
                Environment.Exit(0);
            }
        }
        else
        {
            Console.WriteLine("Файл не найден.");
            Environment.Exit(0);
        }
    }

    public void SaveFile()
    {
        Console.Clear();

        Console.WriteLine("Введите новый путь для сохранения файла:");
        string newFileLocation = Console.ReadLine();

        string fileExtension = Path.GetExtension(newFileLocation);
        if (string.IsNullOrEmpty(fileExtension))
        {
            Console.WriteLine("Неверное расширение файла.");
            return;
        }

        if (fileExtension == ".json")
        {
            string jsonContent = JsonConvert.SerializeObject(Cars, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(newFileLocation, jsonContent);
            Console.WriteLine("Файл сохранен.");
        }
        else if (fileExtension == ".xml")
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Car>));
            using (FileStream fs = new FileStream(newFileLocation, FileMode.Create))
            {
                serializer.Serialize(fs, Cars);
            }
            Console.WriteLine("Файл сохранен");
        }
        else if (fileExtension == ".txt")
        {
            using (StreamWriter writer = new StreamWriter(newFileLocation))
            {
                foreach (var car in Cars)
                {
                    writer.WriteLine(car.ToText());
                }
            }
            Console.WriteLine("Файл сохранен.");
        }
        else
        {
            Console.WriteLine("Неверное расширение файла.");
        }
    }

}

class Program
{
    static void Main(string[] args)
    {

        FileManager FileManager = new FileManager();
        FileManager.ReadFile();

        Console.WriteLine("\nF1-Сохранить Esc-Закрыть");

        ConsoleKeyInfo keyInfo = Console.ReadKey();

        switch (keyInfo.Key)
        {
            case ConsoleKey.F1:
                FileManager.SaveFile();
                break;
            case ConsoleKey.Escape:
                Environment.Exit(0);
                break;
        }
    }
}

