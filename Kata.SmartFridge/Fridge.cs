using NodaTime;

namespace Kata.SmartFridge;

public class Fridge
{
    private bool _isOpen = false;

    private readonly ICollection<IEvent> _events = new List<IEvent>();
    private readonly IClock _clock;
    private readonly FridgePrinter _fridgePrinter;

    public Fridge(IClock clock, FridgePrinter fridgePrinter)
    {
        _clock = clock;
        _fridgePrinter = fridgePrinter;
    }

    public void AddItem(string name, Instant expirationDate, ItemCondition condition)
    {
        if (!_isOpen)
            throw new InvalidOperationException();

        _events.Add(new ItemAdded(_clock.GetCurrentInstant(), name, expirationDate, condition));
    }

    public void Open()
    {
        _isOpen = true;
        _events.Add(new FridgeOpened(_clock.GetCurrentInstant()));
    }

    public void Close()
    {
        _isOpen = false;
    }

    public Item[] GetItems()
    {
        var itemsByName = new Dictionary<string, Item>();
        foreach (var @event in _events)
        {
            switch (@event)
            {
                case ItemAdded itemAdded:
                    var item = Item.From(itemAdded);
                    itemsByName[item.Name] = item;
                    break;
                case IItemEvent itemEvent:
                    itemsByName[itemEvent.ItemName].Apply(itemEvent);
                    break;
                case IEvent globalEvent:
                    foreach (var existingItem in itemsByName.Values)
                        existingItem.Apply(globalEvent);
                    break;
            }
        }

        return itemsByName.Values.ToArray();
    }

    public void PrintItems()
    {
        _fridgePrinter.Print(GetItems());
    }
}
