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
            T temp, current, next;

            for (int j = 0; j <= Count - 2; j++)
                for (int i = 0; i <= Count - 2; i++)
                {
                    current = this[i];
                    next = this[i + 1];

                    if (shouldSwap(current, next))
                    {
                        temp = next;
                        next = current;
                        current = temp;
                    }
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
            price = int.Parse(Console.ReadLine());

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

    class Program
    {
        static readonly SuperList<Book> books = SuperList<Book>.New();

        static bool AscendingSort(Book a, Book b) => a > b;

        static bool DescendingSort(Book a, Book b) => a > b;

        static void Main(string[] args)
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
                        books.BubbleSort(AscendingSort);
                        Console.WriteLine("Books are sorted by Price. Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 4:
                        Console.WriteLine("Are you sure? (Y): Yes; (N): No;");
                        Console.Write("Enter: ");
                        if (Console.ReadLine().Trim().ToUpper() == "N") continue;
                        break;
                    default:
                        Console.WriteLine("Unknown option, please try again.");
                        Console.ReadLine();
                        break;
                }
                if (choice == 4) break;
            }
        }

        static void PrintMenu()
        {
            Console.WriteLine(@"
1) Print Books
2) Add Books
3) Sort
4) Exit
");
        }

        static int GetChoice()
        {
            Console.Write("Your choice is: ");
            return int.Parse(Console.ReadLine());
        }
    }
}
