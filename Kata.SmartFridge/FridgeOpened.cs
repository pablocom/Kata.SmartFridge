using NodaTime;

namespace Kata.SmartFridge;

public record FridgeOpened(Instant Timestamp) : IEvent;

