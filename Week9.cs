using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace DotnetExercises
{
    public class Category
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }
    }

    public class Food
    {
        public int FoodId { get; set; }

        public string Name { get; set; }

        public float Calorie { get; set; }

        public float KCalorie => Calorie / 1000;

        public float Fat { get; set; }

        public int CategoryId { get; set; }

        public static Food FoodFactory(int id, string name, int cid)
        {
            Random rnd = new Random();
            return new Food()
            {
                Name = name,
                Calorie = (float)rnd.NextDouble() * 1_000_000,
                Fat = (float)rnd.NextDouble() * 1_000
            };
        }

        public override string ToString() => $"{Name} with {KCalorie} KCalorie and {Fat} fat.";
    }

    public class Week9
    {
        private readonly List<Food> _foods;

        private readonly List<Category> _categories;

        public Week9()
        {
            _foods = new List<Food>();
            _categories = new List<Category>();
        }

        private void Setup()
        {
            _categories.Add(new Category()
            {
                CategoryId = 0,
                Name = "Drink"
            });

            _categories.Add(new Category()
            {
                CategoryId = 1,
                Name = "Fast food"
            });

            _categories.Add(new Category()
            {
                CategoryId = 2,
                Name = "Meat"
            });

            _categories.Add(new Category()
            {
                CategoryId = 3,
                Name = "Grains"
            });

            _categories.Add(new Category()
            {
                CategoryId = 4,
                Name = "Milk"
            });

            _foods.AddRange(new List<Food>()
            {
                Food.FoodFactory(0, "Pork", 2),
                Food.FoodFactory(1, "Hamburger", 1),
                Food.FoodFactory(2, "Pizza", 1),
                Food.FoodFactory(3, "Beef", 1),
                Food.FoodFactory(4, "Beer", 0),
                Food.FoodFactory(5, "Spagetti", 1),
                Food.FoodFactory(6, "Coffee", 0),
                Food.FoodFactory(7, "Tea", 0),
                Food.FoodFactory(8, "Chicken Soup", 2),
                Food.FoodFactory(9, "Rice", 1),
                Food.FoodFactory(10, "Maccaroni", 2)
            });
        }

        private void Aggregation()
        {
            WriteLine("--- Aggregation 1");
            var result = _foods.Max(f => f.KCalorie);
            WriteLine("Highest calories in menu is " + result + " KCalorie");

            WriteLine("--- Aggregation 2");
            result = _foods.Count(f => f.KCalorie > 300);
            WriteLine("Total foods with over 300 kcalories in the menu is " + result);
        }

        private void Conversion()
        {
            WriteLine("--- Conversion 1: Convert to array");
            Food[] foodsArray = _foods.ToArray();

            WriteLine("--- Conversion 2: Convert to dictionary");
            var categorisedFoods =
                foodsArray.ToDictionary(k => k.Name, v => v.KCalorie > 300 ? "not diet food" : "diet food");
            foreach (KeyValuePair<string, string> food in categorisedFoods)
                WriteLine(food.Key + " is " + food.Value);
        }

        private void Element()
        {
            WriteLine("--- Element 1: Select random");
            var randomIndex = new Random().Next() % _foods.Count;
            WriteLine("Element at " + randomIndex + " is " + _foods.ElementAtOrDefault(randomIndex));

            WriteLine("--- Element 2: Last food in the menu is " + _foods.LastOrDefault());
        }

        private void Generation()
        {
            WriteLine("--- Generation 1: Range");
            foreach (var index in Enumerable.Range(0, _foods.Count))
                WriteLine(_foods[index]);

            WriteLine("--- Generation 2: DefaultIfEmpty");

            Food[] foods = { };
            WriteLine("Food is " + foods.DefaultIfEmpty(Food.FoodFactory(11, "Yogurt", 1)));
        }

        private void Grouping()
        {
            WriteLine("--- Grouping 1: Group by");
            foreach (IGrouping<string, Food> category in _foods.GroupBy(f => f.KCalorie > 300 ? "unhealthy" : "healthy")
            )
            {
                Write($"List of {category.Key} food: ");
                foreach (Food food in category)
                    Write($"{food.Name}, ");
            }

            WriteLine("--- Grouping 2: Group by");
            foreach (IGrouping<string, Food> category in _foods.GroupBy(f => f.Fat > 300 ? "fat" : "diet"))
            {
                Write($"List of {category.Key} food: ");
                foreach (Food food in category)
                    Write($"{food.Name}, ");
                WriteLine();
            }
        }

        private void Join()
        {
            WriteLine("--- Grouping 1: Join");
            var categoriesAvailableInMenu = _categories.Join(_foods, c => c.CategoryId, f => f.CategoryId,
                (category, food) => category);
            Write("Categories available in menu: ");
            foreach (var category in categoriesAvailableInMenu.Distinct())
                Write(category.Name + ", ");
            WriteLine();

            WriteLine("--- Grouping 2: Group Join");
            var groupJoin = _categories.GroupJoin(_foods, category => category.CategoryId, food => food.CategoryId,
                (category, foods) => new { Category = category.Name, Food = foods });
            foreach (var category in groupJoin)
            {
                Write($"> Category {category.Category}: ");
                foreach (Food food in category.Food)
                    Write($"{food.Name}, ");
                WriteLine();
            }
        }

        private void Ordering()
        {
            WriteLine("--- Ordering 1: Order by fat");
            foreach (var food in _foods.OrderBy(f => f.Fat))
                WriteLine(food);

            WriteLine("--- Ordering 2: Order by fat desc");
            foreach (var food in _foods.OrderByDescending(f => f.Fat))
                WriteLine(food);
        }

        private void Other()
        {
            WriteLine("--- Other 1: Concat list");
            List<Food> tmp = new List<Food>()
            {
                Food.FoodFactory(0, "A", 2),
                Food.FoodFactory(1, "B", 1),
                Food.FoodFactory(2, "C", 1),
            };

            foreach (Food food in tmp.Concat(_foods.GetRange(0, 3))) WriteLine(food);

            WriteLine("--- Other 2: Zipping even and odd numbers");
            int[] even = { 1, 3, 5 };
            int[] odd = { 2, 4, 6 };

            var result = even.Zip(odd, (a, b) => a * b);
            foreach (int number in result)
                WriteLine(number);
        }

        private void Partitioning()
        {
            WriteLine("--- Partitioning 1: Take the last 3 foods from menu");
            foreach (Food food in _foods.TakeLast(3))
                WriteLine(food);


            WriteLine("--- Partitioning 2: Take 3 foods from menu");
            foreach (Food food in _foods.Take(3))
                WriteLine(food);
        }

        private void Projection()
        {
            WriteLine("--- Projection 1: Select food names only");
            string[] foodNames = _foods.Select(f => f.Name).ToArray();
            foreach (string foodName in foodNames) WriteLine(foodName);

            WriteLine("--- Projection 1: Select index");
            Tuple<Food, int>[] foodWithIndex = _foods.Select((f, i) => new Tuple<Food, int>(f, i)).ToArray();
            foreach (var foodName in foodWithIndex) WriteLine($"{foodName.Item1.Name} at index {foodName.Item2}");
        }

        private void Quantifiers()
        {
            string allAreHealthy = _foods.All(f => f.KCalorie > 300) ? "are healthy" : "are not healthy";
            WriteLine($"--- Quantifiers 1: All food {allAreHealthy}");

            string existFoodHealthy = _foods.Any(f => f.KCalorie < 300) ? "exist" : "not exist";
            WriteLine($"--- Quantifiers 1: Any - Healthy food does {existFoodHealthy}");
        }

        private void Restriction()
        {
            WriteLine("--- Restriction 1: Simple where");
            foreach (Food food in _foods.Where(f => f.Fat > 300 && f.Fat < 800))
                WriteLine(food);

            WriteLine("--- Restriction 2: Simple where with index");
            foreach (Food food in _foods.Where((f, i) => f.Fat > 300 && i % 2 == 0))
                WriteLine(food);
        }

        private void Set()
        {
            WriteLine("--- Set 1: Distinc");
            int[] categoriesInMenu = _foods.Select(f => f.CategoryId).Distinct().ToArray();
            foreach (int i in categoriesInMenu) Write(i + ", ");
            WriteLine();

            int[] categories = _categories.Select(c => c.CategoryId).Distinct().ToArray();

            WriteLine("--- Set 1: Except");
            int[] notInMenu = categories.Except(categoriesInMenu).ToArray();
            foreach (int i in notInMenu) Write(i + ", ");
            WriteLine();
        }

        public void Run()
        {
            Setup();
            Aggregation();
            Conversion();
            Element();
            Generation();
            Grouping();
            Join();
            Ordering();
            Other();
            Partitioning();
            Projection();
            Quantifiers();
            Restriction();
            Set();
        }
    }
}