using NodaTime;

namespace Kata.SmartFridge;

public record ItemAdded(Instant Timestamp, string ItemName, Instant ExpirationDate, ItemCondition Condition) : IEvent;

