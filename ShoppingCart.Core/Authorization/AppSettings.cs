namespace ShoppingCart.Core.Authorization
{
    public class AppSettings: IAppSettings
    {
        public string Secret { get; set; }
        public int Expires { get; set; }
    }
}
