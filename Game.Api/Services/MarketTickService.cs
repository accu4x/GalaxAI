using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Game.Api.Storage.Repositories;

namespace Game.Api.Services
{
    // Background service that runs market price ticks according to configured cadence.
    public class MarketTickService : BackgroundService
    {
        private readonly ILogger<MarketTickService> _logger;
        private readonly MarketRepository _marketRepo;
        private readonly MarketTickOptions _options;
        private readonly GameTimeService _gameTimeService;

        public MarketTickService(ILogger<MarketTickService> logger,
            MarketRepository marketRepo,
            IOptions<MarketTickOptions> options,
            GameTimeService gameTimeService)
        {
            _logger = logger;
            _marketRepo = marketRepo;
            _options = options.Value;
            _gameTimeService = gameTimeService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MarketTickService started with RealMinutesPerGameDay={RealMinutesPerGameDay} and DevMultiplier={DevMultiplier}", _options.RealMinutesPerGameDay, _options.DevMultiplier);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Compute next tick delay based on config and dev multiplier
                    var minutes = _options.RealMinutesPerGameDay / Math.Max(0.0001, _options.DevMultiplier);
                    var delay = TimeSpan.FromMinutes(minutes);

                    _logger.LogDebug("Next market tick in {Delay}", delay);

                    await Task.Delay(delay, stoppingToken);

                    // Perform a tick
                    await RunTickAsync(stoppingToken);
                }
                catch (TaskCanceledException) { break; }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "MarketTickService tick failed");
                }
            }
        }

        private async Task RunTickAsync(CancellationToken cancellationToken)
        {
            var nowGame = _gameTimeService.NowInGameString();
            _logger.LogInformation("Running market tick at game time {Now}", nowGame);

            // TODO: fetch all market entries and apply price update rules
            // For now call a single repo method to allow repo to handle logic
            await _marketRepo.RunDailyTickAsync(_options.PriceRandomness, nowGame, cancellationToken);

            _logger.LogInformation("Market tick completed");
        }
    }
}