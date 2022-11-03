using FundaAanbodServiceReference;
using static FundaAanbodServiceReference.AanbodClient;
using Funda.ApiClient.Abstractions.Models;
using Funda.ApiClient.Abstractions;

namespace Funda.ApiClient.Wcf;

[Obsolete(@"
Added WCF Service Reference (http://partnerapi.funda.nl/feeds/Aanbod.svc?wsdl) as a Connected Service. 
The generated client keeps failing with non-self-explanatory errors so I moved to calling the service from http client.
")]
public class FundaWcfApiClient : IFundaApiClient
{
    private readonly string _key = "ac1b0b1572524640a0ecc54de453ea9f";
    private readonly string _action = "koop";
    private readonly string _searchTerms = "/amsterdam/tuin/";

    public async Task ThrowsExceptions()
    {
        var page = "1";
        var pageSize = "25";

        using var client = new AanbodClient(EndpointConfiguration.wshttp); // EndpointConfiguration.wshttps

        var zoekAanbodJson = await client.ZoekAanbodJsonAsync(_key, aanbodType: _action, zoekPad: _searchTerms, since: "", page, pageSize, statistiekId: "", projectObjectenTonen: 0);
        var zoekRecent = await client.ZoekRecentAanbodAsync(_key, aanbodType: _action, zoekPad: _searchTerms, since: "", page, pageSize);
        var zoekRecentJson = await client.ZoekRecentAanbodJsonAsync(_key, aanbodType: _action, zoekPad: _searchTerms, since: "", page, pageSize);

        var toppositieFeed = await client.GetToppositieObjectsAsync(_key, zo: _searchTerms);
        var detailKoop = await client.DetailKoopByGlobalIdAsync(_key, globalId: "6550105", statistiekId: null);
    }

    public Task<PagedResponse<RealEstateObject>> GetRealEstateObjects(
        SearchQuery query, 
        PageRequest page, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
