namespace DynamicDbReport.API.PrivateServices;

internal class CORSAuthorization
{
    internal static bool IsOriginAllowed(string OriginURL)
    {
        var uri = new Uri(OriginURL);
        return uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) || uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase) || uri.Host.EndsWith("DynamicDbReport.com", StringComparison.OrdinalIgnoreCase);
    }
}
