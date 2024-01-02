namespace Sample.Api.Infrastructure.Repositories;

public sealed class TestHttpClient
{
    private readonly HttpClient _httpClient;

    public TestHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int> Get(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(new Uri("https://httpbin.org/get"), cancellationToken);
        return (int)response.StatusCode;
    }  
}