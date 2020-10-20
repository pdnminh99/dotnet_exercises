using System;
using System.Collections.Generic;

namespace ExerciseWeek5
{
    delegate bool ShouldSwap<T>(T current, T next);

    interface ISortable<T> where T : IComparable<T>
    {
        public void BubbleSort(ShouldSwap<T> shouldSwap);
    }

    class SuperList<T> : List<T>, ISortable<T> where T : IComparable<T>
    {
        public static SuperList<T> New() => new SuperList<T>();

        public void BubbleSort(ShouldSwap<T> shouldSwap)
        {
            T temp;

            for (int j = 0; j <= Count - 2; j++)
                for (int i = 0; i <= Count - 2; i++)
                    if (shouldSwap(this[i], this[i + 1]))
                    {
                        temp = this[i + 1];
                        this[i + 1] = this[i];
                        this[i] = temp;
                    }
        }

        public static SuperList<T> operator +(SuperList<T> a, SuperList<T> b)
        {
            foreach (var item in b) a.Add(item);
            return a;
        }

        public static SuperList<T> operator ++(SuperList<T> a)
        {
            int len = a.Count;

            if (len == 0) return a;
            a.Add(a[len - 1]);
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

    class Book : IComparable<Book>
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public int Price { get; set; }

        public Book(string title, string author, int price)
        {
            Title = title;
            Author = author;
            Price = price;
        }

        public static Book Create()
        {
            string title, author;
            int price;

            Console.Write("Enter Title: ");
            title = Console.ReadLine();
            Console.Write("Enter Author: ");
            author = Console.ReadLine();
            Console.Write("Enter Price: ");

            while (!int.TryParse(Console.ReadLine(), out price))
                Console.Write("Invalid Price value. Please re-enter new number: ");
            return new Book(title, author, price);
        }

        public static bool operator >(Book a, Book b) => a.CompareTo(b) > 0;

        public static bool operator <(Book a, Book b) => a.CompareTo(b) < 0;

        public override string ToString() => $@"\
Title: {Title},
Author: {Author},
Price: {Price}$.
";

        public int CompareTo(Book other) => Price.CompareTo(other.Price);
    }

    class BookManager
    {
        SuperList<Book> books = SuperList<Book>.New();

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                PrintMenu();
                int choice = GetChoice();
                switch (choice)
                {
                    case 1:
                        Console.WriteLine(books.ToString());
                        Console.WriteLine("----------------");
                        Console.Write("Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 2:
                        books.Add(Book.Create());
                        Console.WriteLine("----------");
                        Console.Write("New book added successfully. Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 3:
                        Console.WriteLine("----------");
                        if (books.Count == 0)
                        {
                            Console.Write("Cannot copy since books list is empty. Press any key to continue.");
                        }
                        else
                        {
                            books++;
                            Console.Write("New book added successfully. Press any key to continue.");
                        }
                        Console.ReadLine();
                        break;
                    case 4:
                        books.BubbleSort(AscendingSort);
                        Console.WriteLine("Books are ascending sorted by Price. Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 5:
                        books.BubbleSort(DescendingSort);
                        Console.WriteLine("Books are ascending sorted by Price. Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 6:
                        Console.WriteLine("Are you sure? (Y): Yes; (N): No;");
                        Console.Write("Enter: ");
                        if (Console.ReadLine().Trim().ToUpper() == "N") continue;
                        break;
                    default:
                        Console.WriteLine("Unknown option, please try again.");
                        Console.ReadLine();
                        break;
                }
                if (choice == 6) break;
            }
        }

        bool AscendingSort(Book a, Book b) => a > b;

        bool DescendingSort(Book a, Book b) => a > b;

        void PrintMenu()
        {
            Console.WriteLine(@"
1) Print Books
2) Add Books
3) Copy Last Book
4) Ascending Sort
5) Descending Sort
6) Exit
");
        }

        int GetChoice()
        {
            Console.Write("Your choice is: ");
            return int.Parse(Console.ReadLine());
        }
    }

    class Program
    {
        static readonly BookManager manager = new BookManager();

        static void Main(string[] args) => manager.Run();
    }
}
