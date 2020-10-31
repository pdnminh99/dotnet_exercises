// using System;
// using System.Collections.Generic;
// using static System.Console;
//
// namespace DotnetExercises
// {
//     abstract class Shape
//     {
//         protected double _height { get; set; }
//
//         protected double _width { get; set; }
//
//         public abstract double Area { get; }
//
//         public abstract double Circumference { get; }
//
//         public Shape(double height, double width)
//         {
//             _height = height;
//             _width = width;
//         }
//
//         ~Shape() => WriteLine($"Shape instance with h: {_height} and w: {_width} is being deleted.");
//     }
//
//     class Rectangle : Shape
//     {
//         public virtual double Height
//         {
//             get => _height;
//             set => _height = value;
//         }
//
//         public virtual double Width
//         {
//             get => _width;
//             set => _width = value;
//         }
//
//         public override double Area => Height * Width;
//
//         public override double Circumference => 2 * (Height + Width);
//
//         public Rectangle(double height, double width) : base(height, width)
//         {
//         }
//
//         ~Rectangle() => WriteLine($"Rectangle instance with h: {Height} and w: {Width} is being deleted.");
//     }
//
//     class Square : Rectangle
//     {
//         public override double Height
//         {
//             get => base.Height;
//             set => Side = value;
//         }
//
//         public override double Width
//         {
//             get => base.Width;
//             set => Side = value;
//         }
//
//         public double Side
//         {
//             get => _height;
//             set
//             {
//                 _height = value;
//                 _width = value;
//             }
//         }
//
//         public Square(int side) : base(side, side)
//         {
//         }
//
//         ~Square() => WriteLine($"Square instance with s: {Side} is being deleted.");
//     }
//
//     class Circle : Shape
//     {
//         public double Radius
//         {
//             get => _height;
//             set
//             {
//                 _height = value;
//                 _width = value;
//             }
//         }
//
//         public double Diameter
//         {
//             get => _height * 2;
//             set => _height = value / 2.0;
//         }
//
//         public Circle(double radius) : base(radius, radius)
//         {
//         }
//
//         public override double Area => Math.PI * Radius * Radius;
//
//         public override double Circumference => Math.PI * Diameter;
//
//         ~Circle() => WriteLine($"Circle instance with r: {Radius}; d: {Diameter} is being deleted.");
//     }
//
//     class Week6
//     {
//         // Enclosing all Shape objects allocation in separate scope
//         static void FirstExercise()
//         {
//             // Begin scope
//             var r = new Rectangle(3, 4.5);
//             WriteLine($"Rectangle H: {r.Height}, W: {r.Width}, Area: {r.Area}, Circumference: {r.Circumference}");
//             var s = new Square(5);
//             WriteLine($"Square S: {s.Side}, Area: {s.Area}, Circumference: {s.Circumference}");
//             var c = new Circle(2.5);
//             WriteLine($"Circle R: {c.Radius}, D: {c.Diameter}, Area: {c.Area}, Circumference: {c.Circumference}");
//         }
//
//         public static void Run()
//         {
//             FirstExercise();
//             WriteLine("--------------------------------------------------");
//             // End scope, try invoking GC to clean up.
//             GC.Collect();
//
//             // Wait until GC finish cleaning up.
//             GC.WaitForPendingFinalizers();
//
//             WriteLine("--------------------------------------------------");
//             // Exercise 2.1
//             var entry = new KeyValue<int, string>(12000111, "Tom");
//
//             int phone = entry.Key;
//             string name = entry.Value;
//
//             WriteLine($"Phone = {phone} / name = {name}");
//
//             WriteLine("--------------------------------------------------");
//             // Exercise 2.2
//             var entryA = new PhoneNameEntry(12000111, "Tom");
//
//             phone = entryA.Key;
//             name = entryA.Value;
//
//             WriteLine($"Phone = {phone} / name = {name}");
//
//             WriteLine("--------------------------------------------------");
//             // Exercise 2.3
//             var entryB = new StringAndValueEntry<String>("E001", "Tom");
//
//             string empNumber = entryB.Key;
//             string empName = entryB.Key;
//
//             WriteLine($"Emp Number = {empNumber}");
//             WriteLine($"Emp Name = {empName}");
//
//             WriteLine("--------------------------------------------------");
//             // Exercise 2.4
//             try
//             {
//                 (new GenericInterfaceImpl<string>()).DoSomething();
//             }
//             catch (MyException<string> _)
//             {
//                 WriteLine("Catch generic interface type string");
//             }
//
//             WriteLine("--------------------------------------------------");
//             // Exercise 2.5
//             var entry1 = new KeyValue<int, String>(12000111, "Tom");
//             var entry2 = new KeyValue<int, String>(12000112, "Jerry");
//
//             phone = MyUtils.GetKey(entry1);
//             WriteLine($"Phone = {phone}");
//
//             var list = new List<KeyValue<int, string>>();
//             list.Add(entry1);
//             list.Add(entry2);
//
//             var firstEntry = MyUtils.GetFirstElement(list, null);
//             if (firstEntry != null) WriteLine($"Value = {firstEntry.Value}");
//
//             WriteLine("--------------------------------------------------");
//             // Exercise 2.6
//             MyException<int> exc = MyUtils.DoSomeThing<MyException<int>>();
//             exc = MyUtils.DoDefault<MyException<int>>();
//             Shape[] shapes = MyUtils.CreateArray<Circle>(10);
//
//             // Exercise 2.6
//             GenericArrayExample.Run();
//
//             WriteLine("Press any key to exit.");
//             ReadLine();
//         }
//     }
//
//     class KeyValue<K, V>
//     {
//         public K Key { get; set; }
//
//         public V Value { get; set; }
//
//         public KeyValue()
//         {
//         }
//
//         public KeyValue(K key, V value)
//         {
//             Key = key;
//             Value = value;
//         }
//     }
//
//     class PhoneNameEntry : KeyValue<int, string>
//     {
//         public PhoneNameEntry(int key, string value) : base(key, value)
//         {
//         }
//     }
//
//     class StringAndValueEntry<V> : KeyValue<string, V>
//     {
//         public StringAndValueEntry()
//         {
//         }
//
//         public StringAndValueEntry(string key, V value)
//             : base(key, value)
//         {
//         }
//     }
//
//     class KeyValueInfo<K, V, I> : KeyValue<K, V>
//     {
//         public I info { get; set; }
//
//         public KeyValueInfo(K key, V value)
//             : base(key, value)
//         {
//         }
//
//         public KeyValueInfo(K key, V value, I info)
//             : base(key, value) => this.info = info;
//     }
//
//
//     interface GenericInterface<G>
//     {
//         G DoSomething();
//     }
//
//     class GenericInterfaceImpl<G> : GenericInterface<G>
//     {
//         public G something { get; set; }
//
//         public G DoSomething() => throw new MyException<G>();
//     }
//
//     class MyException<E> : ApplicationException
//     {
//     }
//
//     class MyUtils
//     {
//         public static K GetKey<K, V>(KeyValue<K, V> entry) => entry.Key;
//
//         public static V GetValue<K, V>(KeyValue<K, V> entry) => entry.Value;
//
//         public static E GetFirstElement<E>(List<E> list, E defaultValue) =>
//             (list == null || list.Count == 0) ? defaultValue : list[0];
//
//         // Generic Init Example
//         public static T DoSomeThing<T>()
//             where T : new() => new T();
//
//         public static K ToDoSomeThing<K>()
//             where K : KeyValue<K, string>, new() => new K();
//
//         public static T DoDefault<T>() => default(T);
//
//         public static T[] CreateArray<T>(int size) => new T[size];
//     }
//
//     class GenericArrayExample
//     {
//         public static T[] FilledArray<T>(T value, int count)
//         {
//             T[] ret = new T[count];
//             for (int i = 0; i < count; i++) ret[i] = value;
//             return ret;
//         }
//
//         public static void Run()
//         {
//             string value = "Hello";
//             string[] filledArray = FilledArray<string>(value, 10);
//
//             foreach (string s in filledArray) Console.WriteLine(s);
//         }
//     }
// }