using System;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Game.Api.Storage.Repositories;

namespace Game.Api.Services
{
    public class NewsFeedOptions
    {
        public string[] RssUrls { get; set; } = new[] {
            "https://www.reutersagency.com/feed/?best-topics=world",
            "http://feeds.bbci.co.uk/news/world/rss.xml"
        };

        // Maximum items to fetch per source
        public int MaxItems { get; set; } = 10;

        // Map fetched headlines to game event templates
        public string TemplateFallbackFaction { get; set; } = "Consortium";
    }

    public class NewsFeedService : BackgroundService
    {
        private readonly ILogger<NewsFeedService> _logger;
        private readonly SystemEventRepository _eventRepo;
        private readonly NewsFeedOptions _options;

        public NewsFeedService(ILogger<NewsFeedService> logger, SystemEventRepository eventRepo, IOptions<NewsFeedOptions> options)
        {
            _logger = logger;
            _eventRepo = eventRepo;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NewsFeedService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await FetchAndSeedAsync(stoppingToken);

                    // Run once per accelerated game day; for simplicity sleep 1 hour real-time (configurable by GameTime/MarketTick options)
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (TaskCanceledException) { break; }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "NewsFeedService error");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private async Task FetchAndSeedAsync(CancellationToken cancellationToken)
        {
            foreach (var url in _options.RssUrls)
            {
                try
                {
                    using var http = new HttpClient();
                    using var stream = await http.GetStreamAsync(url);
                    using var reader = XmlReader.Create(stream);
                    var feed = SyndicationFeed.Load(reader);
                    if (feed == null) continue;

                    int count = 0;
                    foreach (var item in feed.Items)
                    {
                        if (++count > _options.MaxItems) break;
                        var ev = new
                        {
                            id = Guid.NewGuid().ToString("N"),
                            title = item.Title.Text,
                            summary = item.Summary?.Text ?? string.Empty,
                            published = item.PublishDate.UtcDateTime,
                            source = url,
                            mappedFaction = _options.TemplateFallbackFaction
                        };

                        var ent = new Game.Api.Storage.Entities.SystemEventEntity
                        {
                            PartitionKey = "SYSTEM_EVENT",
                            RowKey = $"{ev.id}",
                            SystemId = "global",
                            EventJson = System.Text.Json.JsonSerializer.Serialize(ev)
                        };

                        await _eventRepo.UpsertEventAsync(ent);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to fetch RSS from {Url}", url);
                }
            }

            _logger.LogInformation("NewsFeedService seeded events from RSS feeds");
        }
    }
}