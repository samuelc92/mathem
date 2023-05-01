namespace Mathem.Tests;

using Mathem.Api;

public class GetDeliveryDateServiceTest
{
    private const int POSTAL_CODE = 12345;
    private readonly IGetDeliveryDatesService _service;

    public GetDeliveryDateServiceTest()
    {
        _service = new GetDeliveryDatesService(); 
    }

    [Fact]
    public void GetDeliveryDates_DeliveredOnWeekDay_ReturnsDeliveryDatesThatMatchProductsDayOfWeek()
    {
        // Arrange
        var products = new List<Product>
        {
            new ProductBuilder()
                .WithDeliveryDays(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday })
                .Build(),
            new ProductBuilder()
                .WithDeliveryDays(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Thursday })
                .Build(),
            new ProductBuilder()
                .WithDeliveryDays(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Wednesday })
                .Build()
        };

        // Act
        var result = _service.Handle(POSTAL_CODE, products);

        // Assert
        Assert.True(result.All(p =>
            p.DeliveryDate.DayOfWeek.Equals(DayOfWeek.Monday)));
    }

    [Fact]
    public void GetDeliveryDates_DeliveredInTimeWhenThereIsExternalProduct_ReturnsTenDays()
    {
        // Arrange
        var products = new List<Product>
        {
            new ProductBuilder()
                .WithProductType(ProductType.External)
                .Build(),
            new ProductBuilder()
                .WithDaysInAdvance(3)
                .Build()
        };

        // Act
        var result = _service.Handle(POSTAL_CODE, products);

        // Asserts
        var expectedTeenDays = 10;
        Assert.Equal(result.Count(), expectedTeenDays);
        Assert.Equal(DateTime.Now.Date.AddDays(5), result.Min(p => p.DeliveryDate));
    }

    [Fact]
    public void GetDeliveryDates_WithTemporaryProduct_ReturnFiveDays()
    {
        // Arrange
        var products = new List<Product>
        {
            new ProductBuilder()
                .WithProductType(ProductType.Temporary)
                .Build(),
            new ProductBuilder()
                .WithDeliveryDays(new DayOfWeek[] { DayOfWeek.Saturday })
                .Build()
        };

        // Act
        var result = _service.Handle(POSTAL_CODE, products);

        // Asserts
        var expectedOneDay = 1;
        Assert.Equal(result.Count(), expectedOneDay);
    }

    [Fact]
    public void GetDeliveryDates_MaximumDaysInAdvanceIsGreaterThanZero_ReturnsMaximumDaysInAdvanceAsInitialDate()
    {
        // Arrange
        var maxDaysInAdvance = 3;
        var products = new List<Product>
        {
            new ProductBuilder()
                .WithDaysInAdvance(maxDaysInAdvance)
                .Build()
        };

        // Act
        var result = _service.Handle(POSTAL_CODE, products);

        // Assert
        Assert.Equal(DateTime.Now.Date.AddDays(maxDaysInAdvance), result.First().DeliveryDate);
    }
    
    [Fact]
    public void GetDeliveryDates_WhenThereIsGreenDelivery_ReturnsGreenDeliveryDateWithinNextThreeDaysFirst()
    {
        // Arrange
        var products = new List<Product>
        {
            new ProductBuilder()
                .WithDeliveryDays(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday })
                .Build(),
            new ProductBuilder()
                .WithDeliveryDays(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Saturday })
                .Build()
        };

        // Act
        var result = _service.Handle(POSTAL_CODE, products);

        // Arrange
        var expectedTotalOfDays = 6;
        Assert.Equal(expectedTotalOfDays, result.Count());
        Assert.True(result.ElementAt(0).IsGreenDelivery);
        Assert.False(result.ElementAt(1).IsGreenDelivery);
        Assert.False(result.ElementAt(2).IsGreenDelivery);

        var remainingDates = result.Skip(3).ToList();
        for (int i = 0; i < remainingDates.Count - 1; i++)
            Assert.True(remainingDates[i].DeliveryDate <= remainingDates[i + 1].DeliveryDate);
    }
}