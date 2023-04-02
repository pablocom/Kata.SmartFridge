using NodaTime;

namespace Kata.SmartFridge;

public class Item
{
    public string Name { get; private set; }
    public Instant ExpirationDate { get; private set; }
    public ItemCondition Condition { get; private set; }
    public bool IsExpired { get; private set; }

    private Item(string name, Instant expirationDate, ItemCondition condition) 
        => (Name, ExpirationDate, Condition) = (name, expirationDate, condition);

    public static Item From(ItemAdded itemAdded) 
        => new Item(itemAdded.ItemName, itemAdded.ExpirationDate, itemAdded.Condition);

    public void Apply(FridgeOpened _) 
        => ExpirationDate = ExpirationDate.Minus(Condition.DegradationEverytimeFridgeIsOpened);

    public void Apply(IEvent @event)
    {
        switch (@event)
        {
            case FridgeOpened fridgeOpened:
                Apply(fridgeOpened);
                break;
        }
    }
}

