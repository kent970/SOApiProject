namespace SOApiProject.Data;

public interface IApiService
{
    Task<HttpResponseMessage> GetResponse(int page);
}
public class ApiService:IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetResponse(int page)
    {
        string apiUrl = $"?page={page}&pagesize=100&order=desc&sort=popular&site=stackoverflow";

        var response = await _httpClient.GetAsync(apiUrl);

        return response;
    }
}