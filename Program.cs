using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Xml.Serialization;

namespace DotnetExercises
{
    public delegate bool ShouldSwap<T>(T current, T next);

    enum State
    {
        InStock,
        AlmostOut,
        OutOfStock
    }

    interface ISortable<T> where T : IComparable<T>
    {
        public void BubbleSort(ShouldSwap<T> shouldSwap);
    }

    public class MutationEventPayload : EventArgs
    {
        public DateTime OccurrenceTime = DateTime.Now;
    }

    [Serializable]
    [XmlRoot(ElementName = "Books")]
    public class SuperList<T> : List<T>, ISortable<T> where T : IComparable<T>
    {
        public event EventHandler OnAdd;

        public event EventHandler<MutationEventPayload> OnMutate;

        public SuperList()
        {
        }

        public SuperList(T[] items) : base(items)
        {
        }

        private void OnAddEmitted() => OnAdd?.Invoke(this, EventArgs.Empty);

        private void OnMutateEmitted() => OnMutate?.Invoke(this, new MutationEventPayload());

        public void BubbleSort(ShouldSwap<T> shouldSwap)
        {
            T temp;
            bool isMutated = false;

            for (int j = 0; j <= Count - 2; j++)
            for (int i = 0; i <= Count - 2; i++)
                if (shouldSwap(this[i], this[i + 1]))
                {
                    isMutated = true;
                    temp = this[i + 1];
                    this[i + 1] = this[i];
                    this[i] = temp;
                }

            if (isMutated) OnMutateEmitted();
        }

        public static SuperList<T> operator +(SuperList<T> a, SuperList<T> b)
        {
            foreach (var item in b) a.Add(item);
            a.OnAddEmitted();
            a.OnMutateEmitted();
            return a;
        }

        public static SuperList<T> operator ++(SuperList<T> a)
        {
            int len = a.Count;

            if (len == 0) return a;
            a.Add(a[len - 1]);
            a.OnAddEmitted();
            a.OnMutateEmitted();
            return a;
        }

        public override string ToString()
        {
            if (Count == 0) return "Empty list";
            string message = "";
            for (int i = 0; i < Count; i++) message += $"{i}) {this[i]}";
            return message;
        }
    }

    [Serializable]
    public class Book : IComparable<Book>, IComparer<Book>
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public string Publisher { get; set; }

        public int Price { get; set; }

        public Book()
        {
        }

        public Book(string title, string author, string publisher, int price)
        {
            Title = title;
            Author = author;
            Publisher = publisher;
            Price = price;
        }

        public static Book Create()
        {
            string title, author, publisher;
            int price;

            Console.Write("Enter Title: ");
            title = Console.ReadLine();
            Console.Write("Enter Author: ");
            author = Console.ReadLine();
            Console.Write("Enter Publisher: ");
            publisher = Console.ReadLine();
            Console.Write("Enter Price: ");

            while (!int.TryParse(Console.ReadLine(), out price))
                Console.Write("Invalid Price value. Please re-enter new number: ");
            return new Book(title, author, publisher, price);
        }

        public static bool operator >(Book a, Book b) => a.CompareTo(b) > 0;

        public static bool operator <(Book a, Book b) => a.CompareTo(b) < 0;

        public override string ToString() => $@"
Title: {Title},
Author: {Author},
Publisher: {Publisher},
Price: {$"{Price:n0}"}$.
";

        public int CompareTo(Book other) => Price.CompareTo(other.Price);

        public int Compare(Book x, Book y) => x.CompareTo(y);
    }

    class BookManager
    {
        private SuperList<Book> _books = new SuperList<Book>();

        private State _currentState = State.OutOfStock;

        private DateTime? _lastMutation;

        private readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(SuperList<Book>));

        private readonly IFormatter _binaryFormatter = new BinaryFormatter();

        private readonly string _fileName = "books";

        private readonly string _currentDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            IgnoreNullValues = true
        };

        void Book_OnMutate(object sender, EventArgs args) =>
            _lastMutation = ((MutationEventPayload) args).OccurrenceTime;

        void Book_OnAdd(object sender, EventArgs args)
        {
            var manager = sender as SuperList<Book>;
            int count = manager?.Count ?? 0;

            if (count > 10) _currentState = State.InStock;
            else if (count > 0) _currentState = State.AlmostOut;
            else _currentState = State.OutOfStock;
        }

        public void Run()
        {
            // Try loading data
            PrintLoadMain();
            int choice;
            FileStream fs;

            do
            {
                choice = GetChoice();
                string filePath;

                switch (choice)
                {
                    case 1:
                        filePath = $"{_currentDir}\\{_fileName}.json";
                        if (!File.Exists(filePath))
                        {
                            Console.WriteLine($"File {filePath} does not exist.");
                            break;
                        }

                        var jsonUtf8Reader = new Utf8JsonReader(File.ReadAllBytes(filePath));
                        _books = JsonSerializer.Deserialize<SuperList<Book>>(ref jsonUtf8Reader, _serializerOptions);
                        break;
                    case 2:
                        filePath = $"{_currentDir}\\{_fileName}.xml";
                        if (!File.Exists(filePath))
                        {
                            Console.WriteLine($"File {filePath} does not exist.");
                            break;
                        }

                        fs = File.OpenRead(filePath);
                        if (_xmlSerializer.Deserialize(fs) is SuperList<Book> booksFromXml)
                            _books = booksFromXml;
                        fs.Close();
                        break;
                    case 3:
                        filePath = $"{_currentDir}\\{_fileName}.bin";
                        if (!File.Exists(filePath))
                        {
                            Console.WriteLine($"File {filePath} does not exist.");
                            break;
                        }

                        fs = File.OpenRead(filePath);
                        if (_binaryFormatter.Deserialize(fs) is Book[] booksFromBinary)
                            _books = new SuperList<Book>(booksFromBinary);
                        fs.Close();
                        break;
                    case 4:
                        // Skip load savings phase
                        break;
                }
            } while (choice < 1 || choice > 4);

            if (choice != 4)
            {
                Console.Write("Finish loading savings data. Press any key to continue.");
                Console.ReadLine();
            }

            // Assigning events handlers
            _books.OnAdd += Book_OnAdd;
            _books.OnMutate += Book_OnMutate;

            // Run core program
            while (true)
            {
                Console.Clear();
                PrintState();
                Console.WriteLine("---------");
                PrintMainMenu();
                choice = GetChoice();
                switch (choice)
                {
                    case 1:
                        Console.WriteLine(_books);
                        Console.WriteLine("----------------");
                        Console.Write("Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 2:
                        bool isContinue;
                        var newBooks = new SuperList<Book>();
                        do
                        {
                            Console.WriteLine("------------------------");
                            Console.WriteLine("Please enter book info.");
                            newBooks.Add(Book.Create());
                            Console.WriteLine("----------");
                            Console.Write("New book added successfully.");
                            Console.WriteLine("Do you want to continue adding new book? (Y): Yes; (N): No;");
                            Console.Write("Your choice: ");
                            isContinue = Console.ReadLine()?.Trim().ToUpper() == "Y";
                        } while (isContinue);

                        _books += newBooks;
                        Console.Write("Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 3:
                        Console.WriteLine("----------");
                        if (_books.Count == 0)
                            Console.Write("Cannot copy since books list is empty. Press any key to continue.");
                        else
                        {
                            _books++;
                            Console.Write("New book added successfully. Press any key to continue.");
                        }

                        Console.ReadLine();
                        break;
                    case 4:
                        _books.BubbleSort(AscendingSort);
                        Console.WriteLine("Books are ascending sorted by Price. Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 5:
                        _books.BubbleSort(DescendingSort);
                        Console.WriteLine("Books are ascending sorted by Price. Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 6:
                        byte[] serialized = JsonSerializer.SerializeToUtf8Bytes(_books, _serializerOptions);
                        File.WriteAllBytes($"{_currentDir}\\{_fileName}.json", serialized);
                        Console.WriteLine($"Books are saved in `{_fileName}.json`. Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 7:
                        fs = File.Open($"{_currentDir}\\{_fileName}.xml", FileMode.Create);
                        _xmlSerializer.Serialize(fs, _books);
                        Console.WriteLine($"Books are saved in `{_fileName}.xml`. Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 8:
                        fs = File.Open($"{_currentDir}\\{_fileName}.bin", FileMode.Create);
                        _binaryFormatter.Serialize(fs, _books.ToArray());
                        Console.WriteLine($"Books are saved in `{_fileName}.bin`. Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 9:
                        Console.WriteLine("Are you sure? (Y): Yes; (N): No;");
                        Console.Write("Enter: ");
                        if (Console.ReadLine()?.Trim().ToUpper() == "Y") break;
                        continue;
                    default:
                        Console.WriteLine("Unknown option, please try again.");
                        Console.ReadLine();
                        break;
                }

                if (choice == 9) break;
            }
        }

        bool AscendingSort(Book a, Book b) => a > b;

        bool DescendingSort(Book a, Book b) => a < b;

        void PrintState()
        {
            Console.WriteLine($"Current State: {_currentState.ToString().Replace('_', ' ')}");
            Console.WriteLine($"Last edit is: {_lastMutation?.ToString() ?? "NULL"}");
        }

        void PrintMainMenu()
        {
            Console.WriteLine(@"
1) Print Books
2) Add Books
3) Copy Last Book
4) Ascending Sort
5) Descending Sort
6) Save as JSON
7) Save as XML
8) Save as Binary
9) Exit
");
        }

        void PrintLoadMain()
        {
            Console.WriteLine(@"
1) From JSON
2) From XML
3) From Binary
4) Skip
");
        }

        int GetChoice()
        {
            int choice;

            Console.Write("Your choice is: ");
            while (!int.TryParse(Console.ReadLine(), out choice))
                Console.Write("Invalid Choice value. Please re-enter new number: ");
            return choice;
        }
    }

    public static class Program
    {
        static void Main() => new BookManager().Run();
    }
}