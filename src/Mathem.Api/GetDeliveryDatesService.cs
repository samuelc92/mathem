namespace Mathem.Api;

using System.Linq;

public class GetDeliveryDatesService : IGetDeliveryDatesService
{
    private const int COUNT_DAYS_OF_WEEK = 7;
    private const int UPCOMING_DAYS = 14;
    private const int EXTERNAL_PRODUCT_DAYS_IN_ADVANCE = 5;
    private const int DAYS_PRIORITY_ORDER = 3;
    
    public IEnumerable<PostalCodeDeliveryDate> Handle(
        int postalCode,
        IEnumerable<Product> products)
    {
        var availableDaysOfWeek = GetAvailableWeekDay(); 
        var deliveryDates = GetPostalCodeDeliveryDates(
            postalCode,
            products,
            availableDaysOfWeek);
        return OrderFirstThreeDatesByGreenDelivery(); 

        // Local functions
        IEnumerable<PostalCodeDeliveryDate> OrderFirstThreeDatesByGreenDelivery() =>
            deliveryDates
                .Take(DAYS_PRIORITY_ORDER)
                .OrderByDescending(p => p.IsGreenDelivery)
                .ThenBy(p => p.DeliveryDate)
                .Concat(deliveryDates.Skip(DAYS_PRIORITY_ORDER))
                .ToList();

        IEnumerable<DayOfWeek> GetAvailableWeekDay() =>
            Enum.GetValues<DayOfWeek>()
                .Where(d => products.All(p => p.DeliveryDays.Contains(d)))
                .ToList();
    }

    private IEnumerable<PostalCodeDeliveryDate> GetPostalCodeDeliveryDates(
        int postalCode,
        IEnumerable<Product> products,
        IEnumerable<DayOfWeek> availableDaysOfWeek)
    {
        var today = DateTime.Now.Date;
        var startDate = GetStartDate();
        return Enumerable.Range(0, 1 + GetEndDate().Subtract(startDate).Days)
            .Select(i => startDate.AddDays(i))
            .Where(IsDayOfWeekAvailable)
            .Select(date => new PostalCodeDeliveryDate(
                postalCode,
                date,
                IsGreenDelivery(date)
            ));

        // Local functions
        DateTime GetStartDate() => today.AddDays(GetMaxDaysInAdvance());

        int GetMaxDaysInAdvance() =>
            products.Max(p => p.ProductType.Equals(ProductType.External)
                ? EXTERNAL_PRODUCT_DAYS_IN_ADVANCE 
                : p.DaysInAdvance);

        DateTime GetEndDate() =>
            products.Any(p => p.ProductType.Equals(ProductType.Temporary))
                ? today.AddDays(COUNT_DAYS_OF_WEEK - (int) today.DayOfWeek)
                : today.AddDays(UPCOMING_DAYS);

        bool IsDayOfWeekAvailable(DateTime date) =>
            availableDaysOfWeek.Any(p => p.Equals(date.DayOfWeek));

        bool IsGreenDelivery(DateTime date) =>
            date.DayOfWeek.Equals(DayOfWeek.Wednesday);
    }
}

public record PostalCodeDeliveryDate(
    int PostalCode,
    DateTime DeliveryDate,
    bool IsGreenDelivery
);
