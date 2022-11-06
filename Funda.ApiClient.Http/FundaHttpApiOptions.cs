namespace Funda.ApiClient.Http;

public class FundaHttpApiOptions
{
    public string ApiKey { get; set; } = "";
    public int MaxPageSize { get; set; } = 25;
    public string FeedsBaseUrl { get; set; } = "https://partnerapi.funda.nl/feeds/Aanbod.svc/json/";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(3);
}
