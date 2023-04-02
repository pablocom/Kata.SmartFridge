using NodaTime;

namespace Kata.SmartFridge;

public class ItemsPrinter
{
    private readonly IClock _clock;

    public ItemsPrinter(IClock clock)
    {
        _clock = clock;
    }

    public void Print(Item[] items)
    {
        var expiredItems = items
            .Where(item => item.ExpirationDate < _clock.GetCurrentInstant());
        var nonExpiredItems = items
            .Where(item => item.ExpirationDate >= _clock.GetCurrentInstant())
            .OrderBy(item => item.ExpirationDate);

        foreach (var item in expiredItems)
        {
            Console.WriteLine($"EXPIRED: {item.Name}");
        }

        foreach (var item in nonExpiredItems)
        {
            var remainingToExpire = item.ExpirationDate - _clock.GetCurrentInstant();
            var daysString = remainingToExpire.Days == 1 ? "day" : "days";
            Console.WriteLine($"{item.Name}: {remainingToExpire.Days} {daysString} remaining");
        }
    }
}

