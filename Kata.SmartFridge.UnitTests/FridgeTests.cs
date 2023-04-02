using FluentAssertions;
using NodaTime;
using NSubstitute;

namespace Kata.SmartFridge.UnitTests;

public class FridgeTests
{
    private readonly IClock _clock = Substitute.For<IClock>();

    [Fact]
    public void ItemCannotBeAddedIfFridgeIsClosed()
    {
        var fridge = new Fridge(_clock, new FridgePrinter(_clock));

        var action = () => fridge.AddItem("anyName", Instant.FromUtc(2023, 2, 4, 20, 17), ItemCondition.Sealed);

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ItemsAreAddedToTheFridge()
    {
        var now = Instant.FromUtc(2021, 10, 18, 20, 17);
        AssumeCurrentDateIs(now);
        var fridge = new Fridge(_clock, new FridgePrinter(_clock));

        fridge.Open();
        fridge.AddItem("Milk", Instant.FromUtc(2021, 10, 21, 0, 0), ItemCondition.Sealed);
        fridge.AddItem("Cheese", Instant.FromUtc(2021, 10, 18, 0, 0), ItemCondition.Sealed);
        fridge.AddItem("Beef", Instant.FromUtc(2021, 10, 20, 0, 0), ItemCondition.Sealed);
        fridge.AddItem("Lettuce", Instant.FromUtc(2021, 10, 22, 0, 0), ItemCondition.Sealed);

        var itemsInFridge = fridge.GetItems();
        itemsInFridge.Should().NotBeNull();
        itemsInFridge.Should().HaveCount(4);
        
        itemsInFridge[0].Name.Should().Be("Milk");
        itemsInFridge[0].ExpirationDate.Should().Be(Instant.FromUtc(2021, 10, 21, 0, 0));
        
        itemsInFridge[1].Name.Should().Be("Cheese");
        itemsInFridge[1].ExpirationDate.Should().Be(Instant.FromUtc(2021, 10, 18, 0, 0));
        
        itemsInFridge[2].Name.Should().Be("Beef");
        itemsInFridge[2].ExpirationDate.Should().Be(Instant.FromUtc(2021, 10, 20, 0, 0));
       
        itemsInFridge[3].Name.Should().Be("Lettuce");
        itemsInFridge[3].ExpirationDate.Should().Be(Instant.FromUtc(2021, 10, 22, 0, 0));
    }

    [Fact]
    public void SealedItemsExpirationDateDegradeBy1HourEverytimeFridgeIsOpened()
    {
        var now = Instant.FromUtc(2021, 10, 18, 20, 17);
        var originalExpirationDate = Instant.FromUtc(2021, 10, 21, 0, 0);
        var expectedExpirationDate = originalExpirationDate.Minus(Duration.FromHours(2));
        AssumeCurrentDateIs(now);
        var fridge = new Fridge(_clock, new FridgePrinter(_clock));

        fridge.Open();
        fridge.AddItem("Milk", originalExpirationDate, ItemCondition.Sealed);
        fridge.Close();
        fridge.Open();
        fridge.Close();
        fridge.Open();

        var itemsInFridge = fridge.GetItems();
        itemsInFridge.Single().Name.Should().Be("Milk");
        itemsInFridge.Single().ExpirationDate.Should().Be(expectedExpirationDate);
    }

    [Fact]
    public void OpenedItemsExpirationDateDegradeBy5HoursEverytimeFridgeIsOpened()
    {
        var now = Instant.FromUtc(2021, 10, 18, 20, 17);
        var originalExpirationDate = Instant.FromUtc(2021, 10, 21, 0, 0);
        var expectedExpirationDate = originalExpirationDate.Minus(Duration.FromHours(10));
        AssumeCurrentDateIs(now);
        var fridge = new Fridge(_clock, new FridgePrinter(_clock));

        fridge.Open();
        fridge.AddItem("Milk", originalExpirationDate, ItemCondition.Opened);
        fridge.Close();
        fridge.Open();
        fridge.Close();
        fridge.Open();

        var itemsInFridge = fridge.GetItems();
        itemsInFridge.Single().Name.Should().Be("Milk");
        itemsInFridge.Single().ExpirationDate.Should().Be(expectedExpirationDate);
    }

    [Fact]
    public void ItemsAreReportedCorrectly()
    {
        var now = Instant.FromUtc(2021, 10, 18, 0, 0);
        var fridge = new Fridge(_clock, new FridgePrinter(_clock));

        fridge.Open();
        fridge.AddItem("Lettuce", Instant.FromUtc(2021, 10, 20, 0, 0), ItemCondition.Sealed);
        fridge.AddItem("Beef", Instant.FromUtc(2021, 10, 19, 0, 0), ItemCondition.Sealed);
        fridge.AddItem("Cheese", Instant.FromUtc(2021, 10, 18, 0, 0), ItemCondition.Sealed);
        fridge.AddItem("Milk", Instant.FromUtc(2021, 10, 17, 0, 0), ItemCondition.Sealed);


        var reporter = new FridgePrinter(_clock);

        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        AssumeCurrentDateIs(now);
        fridge.PrintItems();

        var reportOutput = stringWriter.ToString();
        var expectedOutput =
            "EXPIRED: Milk" + Environment.NewLine +
            "Cheese: 0 days remaining" + Environment.NewLine +
            "Beef: 1 day remaining" + Environment.NewLine +
            "Lettuce: 2 days remaining" + Environment.NewLine;

        reportOutput.Should().Be(expectedOutput);
    }

    private void AssumeCurrentDateIs(Instant instant)
    {
        _clock.GetCurrentInstant().Returns(instant);
    }
}
