public class BypassCertificateValidationHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 确保所有请求都使用 localhost
        var uriBuilder = new UriBuilder(request.RequestUri);
        uriBuilder.Host = "localhost"; // 强制使用 localhost
        request.RequestUri = uriBuilder.Uri;

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
        };

        using (var httpClient = new HttpClient(handler))
        {
            // 复制请求头
            foreach (var header in request.Headers)
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }

            return await httpClient.SendAsync(request, cancellationToken);
        }
    }
}