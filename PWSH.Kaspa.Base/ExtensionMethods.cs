using System.Management.Automation;
using System.Text;
using System.Text.Json;

namespace PWSH.Kaspa.Base
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Constructs a properly formatted URI by combining the API address and query path.
        /// </summary>
        /// <param name="api_address">Base API address, with or without trailing slash</param>
        /// <param name="query">Query path, with or without leading slash</param>
        /// <returns>A properly formatted URI combining the API address and query</returns>
        private static string BuildRequestUri(string api_address, string query)
        {
            if (api_address.EndsWith('/'))
                api_address = api_address.Substring(0, api_address.Length - 1);

            if (query.StartsWith('/'))
                query = query.Substring(1);

            return $"{api_address}/{query}";
        }

        /// <summary>
        /// Sends an HTTP request to the specified API endpoint with optional request body.
        /// </summary>
        /// <param name="me">The HttpClient instance</param>
        /// <param name="obj">The calling object (for error context)</param>
        /// <param name="api_address">Base API address</param>
        /// <param name="query">API endpoint query</param>
        /// <param name="method">HTTP method (GET, POST, etc.)</param>
        /// <param name="body">Optional request body object to be serialized as JSON</param>
        /// <param name="timeout_seconds">Request timeout in seconds</param>
        /// <param name="cancellation_token">Token to monitor for cancellation requests</param>
        /// <returns>Tuple containing the HTTP response message (if successful) and an error record (if failed)</returns>
        public static async Task<(HttpResponseMessage?, ErrorRecord?)> SendRequestAsync(this HttpClient me, object? obj, string api_address, string query, HttpMethod method, object? body, ulong timeout_seconds, CancellationToken cancellation_token)
        {
            try
            {
                var uri = BuildRequestUri(api_address, query);
                using var request = new HttpRequestMessage(method, uri);

                if (body is not null)
                {
                    if (method == HttpMethod.Get || method == HttpMethod.Head)
                        return (null, new ErrorRecord(new InvalidOperationException($"HTTP method {method} does not support a request body."), "HttpRequestFailed", ErrorCategory.ConnectionError, obj));

                    var json = JsonSerializer.Serialize(body);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                // Set custom timeout.
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellation_token);
                cts.CancelAfter(TimeSpan.FromSeconds(timeout_seconds));

                return (await me.SendAsync(request, cts.Token), null);
            }
            catch (Exception e)
            { return (null, new ErrorRecord(e, "HttpRequestFailed", ErrorCategory.ConnectionError, obj)); }
        }

         /// <summary>
        /// Processes an HTTP response by returning its raw content without deserialization.
        /// Useful for debugging or when raw response is needed.
        /// </summary>
        /// <param name="me">The HTTP response message to process</param>
        /// <param name="obj">The calling object (for error context)</param>
        /// <param name="timeout_seconds">Operation timeout in seconds</param>
        /// <param name="cancellation_token">Token to monitor for cancellation requests</param>
        /// <returns>Tuple containing the response content as a string (if successful) and an error record (if failed)</returns>
        /// Will output data as RAW string, with no deserialization. Useful for debuging.
        /// </summary>
        public static async Task<(string?, ErrorRecord?)> ProcessResponseRAWAsync(this HttpResponseMessage me, object? obj, ulong timeout_seconds, CancellationToken cancellation_token)
        {
            try
            {
                // Set custom timeout.
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellation_token);
                cts.CancelAfter(TimeSpan.FromSeconds(timeout_seconds));

                return me.IsSuccessStatusCode
                    ? (await me.Content.ReadAsStringAsync(cts.Token), null)
                    : (null, new ErrorRecord(new HttpRequestException($"API request failed with status code {me.StatusCode}."), "HttpRequestFailed", ErrorCategory.InvalidResult, me.StatusCode));
            }
            catch (Exception e)
            {  return (null, new ErrorRecord(e, "UnhandledException", ErrorCategory.NotSpecified, obj)); }
        }

        /// <summary>
        /// Processes an HTTP response by deserializing its content into the specified type.
        /// </summary>
        /// <typeparam name="T">Type to deserialize the response content into</typeparam>
        /// <param name="me">The HTTP response message to process</param>
        /// <param name="options">JSON serializer options for deserialization</param>
        /// <param name="obj">The calling object (for error context)</param>
        /// <param name="timeout_seconds">Operation timeout in seconds</param>
        /// <param name="cancellation_token">Token to monitor for cancellation requests</param>
        /// <returns>Tuple containing the deserialized object (if successful) and an error record (if failed)</returns>
        /// Will output data as deserialized schema.
        /// </summary>
        public async static Task<(T?, ErrorRecord?)> ProcessResponseAsync<T>(this HttpResponseMessage me, JsonSerializerOptions? options, object? obj, ulong timeout_seconds, CancellationToken cancellation_token)
            where T : class
        {
            try
            {
                if (!me.IsSuccessStatusCode) 
                    return (null, new ErrorRecord(new HttpRequestException($"API request failed with status code {me.StatusCode}."), "HttpRequestFailed", ErrorCategory.InvalidResult, me.StatusCode));

                // Set custom timeout.
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellation_token);
                cts.CancelAfter(TimeSpan.FromSeconds(timeout_seconds));

                var body = await me.Content.ReadAsStringAsync(cts.Token);
                return (JsonSerializer.Deserialize<T>(body, options), null);
            }
            catch (Exception e)
            { return (null, new ErrorRecord(e, "UnhandledException", ErrorCategory.NotSpecified, obj)); }
        }

        /// <summary>
        /// Creates a CancellationToken that will be canceled when the PowerShell cmdlet's Stopping property becomes true.
        /// This allows for proper cancellation handling of asynchronous operations in PowerShell cmdlets.
        /// </summary>
        /// <param name="cmdlet">The PowerShell cmdlet instance</param>
        /// <param name="polling_interval_ms">How frequently to check the Stopping property (in milliseconds)</param>
        /// <returns>A CancellationToken that will be canceled when the cmdlet is stopping</returns>
        public static CancellationToken CreateStoppingToken(this PSCmdlet cmdlet, int polling_interval_ms = 100)
        {
            var cts = new CancellationTokenSource();

            // Create a timer that periodically checks if the cmdlet is being stopped.
            var timer = new Timer(state =>
            {
                if (state is PSCmdlet psCmdlet && psCmdlet.Stopping)
                {
                    try
                    { cts.Cancel(); }
                    catch (ObjectDisposedException)
                    {
                        // CTS might already be disposed, which is fine.
                    }
                }
            }, cmdlet, 0, polling_interval_ms);

            // Ensure the timer is disposed when the token is canceled.
            cts.Token.Register(() => { timer.Dispose(); });

            return cts.Token;
        }

        /// <summary>
        /// Compares two lists to check if they have the same elements in the same order.
        /// </summary>
        /// <typeparam name="T">Type of elements in the lists</typeparam>
        /// <param name="me">The first list</param>
        /// <param name="other">The second list to compare with</param>
        /// <returns>True if both lists are null or if they contain the same elements in the same order; false otherwise</returns>
        public static bool CompareList<T>(this List<T>? me, List<T>? other)
            => me is null && other is null || me is not null && other is not null && me.SequenceEqual(other);

        /// <summary>
        /// Compares two strings for exact equality using ordinal comparison.
        /// </summary>
        /// <param name="me">The first string</param>
        /// <param name="other">The second string to compare with</param>
        /// <returns>True if both strings are null or if they are exactly equal; false otherwise</returns>
        public static bool CompareString(this string? me, string? other)
            => me is null && other is null || me is not null && other is not null && me.Equals(other, StringComparison.Ordinal);

        /// <summary>
        /// Generates a hash code for a list that is independent of the order of elements.
        /// Useful for implementing GetHashCode() in classes containing lists.
        /// </summary>
        /// <typeparam name="T">Type of elements in the list</typeparam>
        /// <param name="me">The list to generate a hash code for</param>
        /// <param name="previous_hash">Initial hash code to combine with the list's hash</param>
        /// <returns>A hash code that combines the previous hash with a hash of all list elements</returns>
        public static int GenerateHashCode<T>(this List<T>? me, int previous_hash)
        {
            if (me is null) return previous_hash;

            var hash = previous_hash;

            // Sort or use XOR to make order-independent.
            foreach (var element in me.OrderBy(a => a)) hash = HashCode.Combine(hash, element);

            return hash;
        }
    }
}
