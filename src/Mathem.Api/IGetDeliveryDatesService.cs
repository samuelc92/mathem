namespace Mathem.Api;

public interface IGetDeliveryDatesService
{
    IEnumerable<PostalCodeDeliveryDate> Handle(int postalCode, IEnumerable<Product> products);
}