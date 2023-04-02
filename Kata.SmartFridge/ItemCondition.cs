using NodaTime;

namespace Kata.SmartFridge;

public class ItemCondition 
{
    public static readonly ItemCondition Opened = new(Duration.FromHours(5));
    public static readonly ItemCondition Sealed = new(Duration.FromHours(1));
    
    public Duration DegradationEverytimeFridgeIsOpened { get; }

    private ItemCondition(Duration degradation) => DegradationEverytimeFridgeIsOpened = degradation;
}

