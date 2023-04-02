using NodaTime;

namespace Kata.SmartFridge;

public interface IEvent
{
    Instant Timestamp { get; }
}

