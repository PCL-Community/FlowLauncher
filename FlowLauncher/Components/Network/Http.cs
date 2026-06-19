using System.Net;

namespace FlowLauncher.Components.Network;

[Flow.Scope("http")]
public static partial class Http
{
    private static HttpClient? _currentClient = null;

    /// <summary>
    /// 获取当前正在使用的 <see cref="HttpClient"/> 实例。
    /// </summary>
    public static HttpClient CurrentClient => _currentClient
        ?? throw new InvalidOperationException("No HttpClient is available, did http scope loaded?");

    private static HttpClient CreateClient()
    {
        var socketsHttpHandler = new SocketsHttpHandler
        {
            AllowAutoRedirect = true,
            AutomaticDecompression = DecompressionMethods.All
        };
        return new HttpClient(socketsHttpHandler);
    }

    private static readonly CancellationTokenSource _CleanTokenSource = new();

    [Flow.Task] [Flow.Run(After = "app:load")]
    private static Task _()
    {
        Task.Run(async () =>
        {
            while (!_CleanTokenSource.IsCancellationRequested)
            {
                var lastClient = _currentClient;
                _currentClient = CreateClient();
                await Task.Delay(TimeSpan.FromHours(4), _CleanTokenSource.Token);
                lastClient?.Dispose();
            }
        });
        return Task.CompletedTask;
    }

    [Flow.Task] [Flow.Run(After = "app:stop")]
    private static async Task Clean()
    {
        await _CleanTokenSource.CancelAsync();
        _currentClient?.Dispose();
        _CleanTokenSource.Dispose();
    }

    /// <summary>
    /// 发起 GET 请求。
    /// </summary>
    /// <param name="uri">请求 URI</param>
    /// <param name="completion">请求完成选项</param>
    /// <param name="cancelToken">取消令牌</param>
    /// <returns>响应内容</returns>
    public static Task<HttpResponseMessage> Get(Uri uri,
        HttpCompletionOption completion = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancelToken = default)
    {
        return CurrentClient.SendAsync(new HttpRequestMessage {
            Method = HttpMethod.Get,
            RequestUri = uri,
        }, completion, cancelToken);
    }

    /// <summary>
    /// 发起 GET 请求。
    /// </summary>
    /// <param name="uri">请求 URI</param>
    /// <param name="completion">请求完成选项</param>
    /// <param name="cancelToken">取消令牌</param>
    /// <returns>响应内容</returns>
    public static Task<HttpResponseMessage> Get(string uri,
        HttpCompletionOption completion = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancelToken = default)
    {
        return Get(new Uri(uri), completion, cancelToken);
    }

    /// <summary>
    /// 发起 POST 请求。
    /// </summary>
    /// <param name="uri">请求 URI</param>
    /// <param name="content">POST 内容</param>
    /// <param name="completion">请求完成选项</param>
    /// <param name="cancelToken">取消令牌</param>
    /// <returns>响应内容</returns>
    public static Task<HttpResponseMessage> Post(Uri uri,
        HttpContent content,
        HttpCompletionOption completion = HttpCompletionOption.ResponseHeadersRead,
        CancellationToken cancelToken = default)
    {
        return CurrentClient.SendAsync(new HttpRequestMessage {
            Method = HttpMethod.Post,
            RequestUri = uri,
            Content = content,
        }, completion, cancelToken);
    }

    /// <summary>
    /// 发起 POST 请求。
    /// </summary>
    /// <param name="uri">请求 URI</param>
    /// <param name="content">POST 内容</param>
    /// <param name="completion">请求完成选项</param>
    /// <param name="cancelToken">取消令牌</param>
    /// <returns>响应内容</returns>
    public static Task<HttpResponseMessage> Post(string uri,
        HttpContent content,
        HttpCompletionOption completion = HttpCompletionOption.ResponseHeadersRead,
        CancellationToken cancelToken = default)
    {
        return Post(new Uri(uri), content, completion, cancelToken);
    }
}
