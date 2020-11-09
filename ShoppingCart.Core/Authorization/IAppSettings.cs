namespace ShoppingCart.Core.Authorization
{
    public interface IAppSettings
    {
        string Secret { get; set; }
        int Expires { get; set; }
    }
}
