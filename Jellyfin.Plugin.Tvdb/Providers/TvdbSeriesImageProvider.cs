using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Extensions;
using MediaBrowser.Model.Globalization;
using MediaBrowser.Model.Providers;

using Microsoft.Extensions.Logging;

using Tvdb.Sdk;

namespace Jellyfin.Plugin.Tvdb.Providers;

/// <summary>
/// Tvdb series image provider.
/// </summary>
public class TvdbSeriesImageProvider : IRemoteImageProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TvdbSeriesImageProvider> _logger;
    private readonly TvdbClientManager _tvdbClientManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="TvdbSeriesImageProvider"/> class.
    /// </summary>
    /// <param name="httpClientFactory">Instance of the <see cref="IHttpClientFactory"/> interface.</param>
    /// <param name="logger">Instance of the <see cref="ILogger{TvdbSeriesImageProvider}"/> interface.</param>
    /// <param name="tvdbClientManager">Instance of <see cref="TvdbClientManager"/>.</param>
    public TvdbSeriesImageProvider(
        IHttpClientFactory httpClientFactory,
        ILogger<TvdbSeriesImageProvider> logger,
        TvdbClientManager tvdbClientManager)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _tvdbClientManager = tvdbClientManager;
    }

    /// <inheritdoc />
    public string Name => TvdbPlugin.ProviderName;

    /// <inheritdoc />
    public bool Supports(BaseItem item)
    {
        return item is Series;
    }

    /// <inheritdoc />
    public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
    {
        yield return ImageType.Primary;
        yield return ImageType.Banner;
        yield return ImageType.Backdrop;
        yield return ImageType.Logo;
        yield return ImageType.Art;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
    {
        if (!item.IsSupported())
        {
            return Enumerable.Empty<RemoteImageInfo>();
        }

        var languages = await _tvdbClientManager.GetLanguagesAsync(cancellationToken)
            .ConfigureAwait(false);
        var languageLookup = languages
            .ToDictionary(l => l.Id, StringComparer.OrdinalIgnoreCase);

        var artworkTypes = await _tvdbClientManager.GetArtworkTypeAsync(cancellationToken)
            .ConfigureAwait(false);
        var seriesArtworkTypeLookup = artworkTypes
            .Where(t => string.Equals(t.RecordType, "series", StringComparison.OrdinalIgnoreCase))
            .Where(t => t.Id.HasValue)
            .ToDictionary(t => t.Id!.Value);

        var seriesTvdbId = item.GetTvdbId();
        var seriesArtworks = await GetSeriesArtworks(seriesTvdbId, cancellationToken)
            .ConfigureAwait(false);

        var remoteImages = new List<RemoteImageInfo>();

        bool excludeTextLessImages = TvdbPlugin.Instance?.Configuration.ExcludeTextLessImages ?? false;
        _logger.LogInformation("Getting images for {ItemName} ExcludedTextLess : {ExcludedTextless}", item.Name, excludeTextLessImages);

        var needToExcludeTextLessImages = excludeTextLessImages && seriesArtworks.Any(a => a.Language is null);

        foreach (var artwork in seriesArtworks)
        {
            if (needToExcludeTextLessImages && artwork.Language is null)
            {
                continue;
            }

            var artworkType = artwork.Type is null ? null : seriesArtworkTypeLookup.GetValueOrDefault(artwork.Type!.Value);
            var imageType = artworkType.GetImageType();
            var artworkLanguage = artwork.Language is null ? null : languageLookup.GetValueOrDefault(artwork.Language);

            // only add if valid RemoteImageInfo
            remoteImages.AddIfNotNull(artwork.CreateImageInfo(Name, imageType, artworkLanguage));
        }

        return remoteImages.OrderByLanguageDescending(item.GetPreferredMetadataLanguage());
    }

    private async Task<IReadOnlyList<ArtworkExtendedRecord>> GetSeriesArtworks(int seriesTvdbId, CancellationToken cancellationToken)
    {
        try
        {
            var seriesInfo = await _tvdbClientManager.GetSeriesImagesAsync(seriesTvdbId, string.Empty, cancellationToken)
                .ConfigureAwait(false);
            return seriesInfo.Artworks;
        }
        catch (SeriesException ex) when (ex.InnerException is JsonException)
        {
            _logger.LogError(ex, "Failed to retrieve series images for {TvDbId}", seriesTvdbId);
            return Array.Empty<ArtworkExtendedRecord>();
        }
    }

    /// <inheritdoc />
    public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        return _httpClientFactory.CreateClient(NamedClient.Default).GetAsync(new Uri(url), cancellationToken);
    }

    /// <summary>
    /// Gets the countries.
    /// </summary>
    /// <param name="cancellationToken">cancellation token.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public Task<IEnumerable<CountryInfo>> GetCountries(CancellationToken cancellationToken)
    {
        return Task.FromResult(Enumerable.Empty<CountryInfo>());
    }
}
