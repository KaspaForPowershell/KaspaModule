using System.Management.Automation;
using System.Text.Json;

namespace PWSH.Kaspa.Verbs
{
    public sealed class KaspaModuleInitializer : IModuleAssemblyInitializer, IDisposable
    {
        private HttpClient? _httpClient;
        private JsonSerializerOptions? _responseDeserializerOptions;
        private JsonSerializerOptions? _responseSerializerOptions;

        private bool _disposed = false;

/* -----------------------------------------------------------------
ACCESSORS                                                          |
----------------------------------------------------------------- */

        public static KaspaModuleInitializer? Instance { get; private set; }

        public HttpClient? HttpClient => this._httpClient;
        public JsonSerializerOptions? ResponseDeserializer => this._responseDeserializerOptions;
        public JsonSerializerOptions? ResponseSerializer => this._responseSerializerOptions;

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

        public void OnImport()
        {
            Instance = this;
            
            this._httpClient = new HttpClient(new HttpClientHandler()); //{ Timeout = TimeSpan.FromSeconds(Globals.DEFAULT_TIMEOUT_SECONDS) };
            this._httpClient.DefaultRequestHeaders
                .Add("Access-Control-Allow-Origin", "*");

            this._responseDeserializerOptions = new() { PropertyNameCaseInsensitive = true };
            this._responseSerializerOptions = new() { WriteIndented = true }; // For response schemas ToJSON().
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects) https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
                    this._httpClient?.Dispose();
                    this._httpClient = null;

                    this._responseDeserializerOptions = null;
                    this._responseSerializerOptions = null;
                }

                this._disposed = true;
            }
        }
    }
}
