using GeckosoftImages.Interfaces;
using GeckosoftImages.Requests;

namespace GeckosoftImages.Async
{
    public class ImageResizeBackgroundWorker : BackgroundService
    {
        private readonly IBackgroundQueue<ImageResizeRequest> _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ImageResizeBackgroundWorker> _logger;

        public ImageResizeBackgroundWorker(
            IBackgroundQueue<ImageResizeRequest> queue,
            IServiceScopeFactory scopeFactory,
            ILogger<ImageResizeBackgroundWorker> logger
            )
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{Type} is now running in the background.", nameof(ImageResizeBackgroundWorker));

            await BackgroundProcessing(stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogCritical(
                "The {Type} is stopping due to a host shutdown, queued items might not be processed anymore.",
                nameof(ImageResizeBackgroundWorker)
            );

            return base.StopAsync(cancellationToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(500, stoppingToken);
                var imageResizeRequest = _queue.Dequeue();

                if (imageResizeRequest == null) continue;

                _logger.LogInformation("Starting to process ..");

                using var scope = _scopeFactory.CreateScope();
                var imageService = scope.ServiceProvider.GetRequiredService<IImageService>();

                await imageService.ResizeImage(imageResizeRequest);
            }
        }
    }
}
