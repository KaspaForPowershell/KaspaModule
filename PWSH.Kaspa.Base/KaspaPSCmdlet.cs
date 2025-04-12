using System.Management.Automation;
using System.Text.Json;

namespace PWSH.Kaspa.Base
{
    public abstract class KaspaPSCmdlet : PSCmdlet
    {
        protected HttpClient? _httpClient;
        protected HttpResponseMessage? _response;
        protected JsonSerializerOptions? _deserializerOptions;

        protected virtual string BuildQuery()
            => string.Empty;
    }
}