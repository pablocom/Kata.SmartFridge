namespace Kata.SmartFridge;

public interface IItemEvent : IEvent
{
    string ItemName { get; }
}

