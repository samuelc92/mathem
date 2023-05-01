namespace Mathem.Tests;

using Mathem.Api;

public class ProductBuilder
{
    private int _productId = 0;

    private string _name = string.Empty;

    private DayOfWeek[] _deliveryDays = new DayOfWeek[]
    {
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday,
        DayOfWeek.Sunday
    };

    private ProductType _productType = ProductType.Normal;

    private int _daysInAdvance = 0;

    public ProductBuilder WithDeliveryDays(DayOfWeek[] deliveryDays)
    {
        _deliveryDays = deliveryDays;
        return this;
    }

    public ProductBuilder WithProductType(ProductType productType)
    {
        _productType = productType;
        return this;
    }

    public ProductBuilder WithDaysInAdvance(int daysInAdvance)
    {
        _daysInAdvance = daysInAdvance;
        return this;
    }

    public Product Build() =>
        new Product(
            _productId,
            _name,
            _deliveryDays,
            _productType,
            _daysInAdvance);
}
