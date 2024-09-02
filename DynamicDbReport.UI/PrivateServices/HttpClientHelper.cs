using DynamicDbReport.DTO.Shared;
using System.Net;
using System.Text;

namespace DynamicDbReport.UI.PrivateServices;

public class HttpClientHelper(HttpClient _http)
{

    public async Task<T> HttpClientReceiveAsync<T>(HttpMethod httpMethod, string url, string content) where T : new()
    {
        HttpRequestMessage requestor = new(httpMethod, url) { Content = new StringContent(content) };
        requestor.Content.Headers.ContentType = new("application/json");
        var responseAPI = await _http.SendAsync(requestor);

        if (responseAPI.StatusCode == HttpStatusCode.OK)
        {
            byte[] responseBytes = await responseAPI.Content?.ReadAsByteArrayAsync();
            return Encoding.UTF8.GetString(responseBytes).JsonToObject<T>();
        }

        return new T();
    }

}
