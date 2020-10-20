/**
 * Họ và tên: Phạm Đỗ Nhật Minh
 * MSSV: 217 2259
 * Github: https://github.com/pdnminh99/dotnet_w05_exercise
 */

using System;
using System.Collections.Generic;

namespace ExerciseWeek5
{
    delegate bool ShouldSwap<T>(T current, T next);

    enum State
    {
        IN_STOCK,
        ALMOST_OUT,
        OUT_OF_STOCK
    }

    interface ISortable<T> where T : IComparable<T>
    {
        public void BubbleSort(ShouldSwap<T> shouldSwap);
    }

    class MutationEventPayload : EventArgs
    {
        public DateTime OccurrenceTime = DateTime.Now;
    }

    class SuperList<T> : List<T>, ISortable<T> where T : IComparable<T>
    {
        public event EventHandler OnAdd;

        public event EventHandler<MutationEventPayload> OnMutate;

        public static SuperList<T> New() => new SuperList<T>();

        public void OnAddEmitted() => OnAdd?.Invoke(this, EventArgs.Empty);

        public void OnMutateEmitted() => OnMutate?.Invoke(this, new MutationEventPayload());

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

        public override string ToString() => $@"Title: {Title},
Author: {Author},
Price: {Price}$.
";

        public int CompareTo(Book other) => Price.CompareTo(other.Price);
    }

    class BookManager
    {
        SuperList<Book> Books = SuperList<Book>.New();

        State CurrentState = State.OUT_OF_STOCK;

        DateTime? LastMutation = null;

        void Book_OnMutate(object sender, EventArgs args)
        {
            var payload = (MutationEventPayload)args;
            LastMutation = payload.OccurrenceTime;
        }

        void Book_OnAdd(object sender, EventArgs args)
        {
            SuperList<Book> manager = (SuperList<Book>)sender;
            CurrentState = manager.Count switch
            {
                0 => State.OUT_OF_STOCK,
                1 => State.ALMOST_OUT,
                2 => State.IN_STOCK,
                _ => State.IN_STOCK
            };
        }

        public void Run()
        {
            Books.OnAdd += Book_OnAdd;
            Books.OnMutate += Book_OnMutate;

            while (true)
            {
                Console.Clear();
                PrintState();
                Console.WriteLine("---------");
                PrintMenu();
                int choice = GetChoice();
                switch (choice)
                {
                    case 1:
                        Console.WriteLine(Books);
                        Console.WriteLine("----------------");
                        Console.Write("Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 2:
                        bool isContinue = false;
                        SuperList<Book> newBooks = SuperList<Book>.New();
                        do
                        {
                            Console.WriteLine("------------------------");
                            Console.WriteLine("Please enter book info.");
                            newBooks.Add(Book.Create());
                            Console.WriteLine("----------");
                            Console.Write("New book added successfully.");
                            Console.WriteLine("Do you want to continue adding new book? (Y): Yes; (N): No;");
                            Console.Write("Your choice: ");
                            isContinue = Console.ReadLine().Trim().ToUpper() == "Y";
                        } while (isContinue);

                        Books += newBooks;
                        Console.Write("Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 3:
                        Console.WriteLine("----------");
                        if (Books.Count == 0)
                            Console.Write("Cannot copy since books list is empty. Press any key to continue.");
                        else
                        {
                            Books++;
                            Console.Write("New book added successfully. Press any key to continue.");
                        }
                        Console.ReadLine();
                        break;
                    case 4:
                        Books.BubbleSort(AscendingSort);
                        Console.WriteLine("Books are ascending sorted by Price. Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 5:
                        Books.BubbleSort(DescendingSort);
                        Console.WriteLine("Books are ascending sorted by Price. Press any key to continue.");
                        Console.ReadLine();
                        break;
                    case 6:
                        Console.WriteLine("Are you sure? (Y): Yes; (N): No;");
                        Console.Write("Enter: ");
                        if (Console.ReadLine().Trim().ToUpper() == "Y") break;
                        continue;
                    default:
                        Console.WriteLine("Unknown option, please try again.");
                        Console.ReadLine();
                        break;
                }
                if (choice == 6) break;
            }
        }

        bool AscendingSort(Book a, Book b) => a > b;

        bool DescendingSort(Book a, Book b) => a < b;

        void PrintState()
        {
            Console.WriteLine($"Current State: {CurrentState.ToString().Replace('_', ' ')}");
            Console.WriteLine($"Last edit is: {LastMutation?.ToString() ?? "NULL"}");
        }

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
