using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Tvdb.Configuration
{
    /// <summary>
    /// Configuration for tvdb.
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        /// <summary>
        /// Gets the tvdb api key for project.
        /// </summary>
        public const string ProjectApiKey = "__TVDB_API_KEY__";
        private int _cacheDurationInHours = 1;
        private int _cacheDurationInDays = 7;
        private int _metadataUpdateInHours = 2;

        /// <summary>
        /// Gets or sets the tvdb api key for user.
        /// </summary>
        /// <remarks>
        /// This is the subscriber's pin.
        /// </remarks>
        public string SubscriberPIN { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the cache in hours.
        /// </summary>
        public int CacheDurationInHours
        {
            get => _cacheDurationInHours;
            set => _cacheDurationInHours = value < 1 ? 1 : value;
        }

        /// <summary>
        /// Gets or sets the cache in days.
        /// </summary>
        public int CacheDurationInDays
        {
            get => _cacheDurationInDays;
            set => _cacheDurationInDays = value < 1 ? 7 : value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether if the plugin excludes textless images.
        /// </summary>
        public bool ExcludeTextLessImages { get; set; } = true;

        /// <summary>
        /// Gets or sets the fallback languages.
        /// </summary>
        public string FallbackLanguages { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to import season name.
        /// </summary>
        public bool ImportSeasonName { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to fallback to original language.
        /// </summary>
        public bool FallbackToOriginalLanguage { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to include missing specials for Missing Episode Provider.
        /// </summary>
        public bool IncludeMissingSpecials { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to remove all missing episodes on refresh for Missing Episode Provider.
        /// </summary>
        public bool RemoveAllMissingEpisodesOnRefresh { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to include original country in tags.
        /// </summary>
        public bool IncludeOriginalCountryInTags { get; set; } = false;

        /// <summary>
        /// Gets or sets the metadata update in hours for the Check for Metadata Updates Scheduled Task.
        /// </summary>
        public int MetadataUpdateInHours
        {
            get => _metadataUpdateInHours;
            set => _metadataUpdateInHours = value < 1 ? 1 : value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to update series for the Check for Metadata Updates Scheduled Task.
        /// </summary>
        public bool UpdateSeriesScheduledTask { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to update season for the Check for Metadata Updates Scheduled Task.
        /// </summary>
        public bool UpdateSeasonScheduledTask { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to update episode for the Check for Metadata Updates Scheduled Task.
        /// </summary>
        public bool UpdateEpisodeScheduledTask { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to update movie for the Check for Metadata Updates Scheduled Task.
        /// </summary>
        public bool UpdateMovieScheduledTask { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to update person for the Check for Metadata Updates Scheduled Task.
        /// </summary>
        public bool UpdatePersonScheduledTask { get; set; } = false;
    }
}
