namespace Rockaway.WebApp.Services;

public static class UriExtensions {
	public static Uri Append(this Uri uri, params string[] paths)
		=> new(paths.Aggregate(uri.AbsoluteUri, (current, path)
			=> $"{current.TrimEnd('/')}/{path.TrimStart('/')}"));

	public static Uri GetWebsiteBaseUri(this HttpRequest request)
		=> new($"{request.Scheme}://{request.Host}/");
}
