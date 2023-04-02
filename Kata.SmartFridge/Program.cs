using Kata.SmartFridge;
using NodaTime;

var fridge = new Fridge(SystemClock.Instance, new FridgePrinter(SystemClock.Instance));

fridge.Open();
fridge.AddItem("Chicken", SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromHours(3)), ItemCondition.Sealed);
fridge.AddItem("Eggs", SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromHours(10)), ItemCondition.Sealed);
fridge.AddItem("Salmoreho mi arma", SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromHours(10)), ItemCondition.Sealed);
fridge.AddItem("Pasta rellena", SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromHours(5)), ItemCondition.Opened);
fridge.Close();

fridge.Open();
fridge.Close();

fridge.PrintItems();

Console.ReadLine();
