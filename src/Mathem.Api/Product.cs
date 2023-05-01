namespace Mathem.Api;

public record Product(
    int ProductId,
    string Name,
    DayOfWeek[] DeliveryDays,
    ProductType ProductType,
    int DaysInAdvance
);

public enum ProductType
{
    Normal,
    External,
    Temporary
}