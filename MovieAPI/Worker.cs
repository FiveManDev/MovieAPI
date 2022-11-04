using Amazon.S3;
using MovieAPI.Controllers;
using MovieAPI.Data.DbConfig;
using MovieAPI.Services;
using MovieAPI.Services.AWS;
using Serilog;

namespace MovieAPI
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<MovieController> logger;
        private readonly IAmazonS3 s3Client;
        private readonly int ThreadDelay = 1;
        public Worker(ILogger<MovieController> logger, IAmazonS3 s3Client)
        {
            this.logger = logger;
            this.s3Client = s3Client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation(new String('*', 50));
                logger.LogInformation("Server check at {time}", DateTimeOffset.Now);
                try
                {
                    // var listFile = await AmazonS3Bucket.IsBuckerExist(s3Client);
                    // if (listFile == null)
                    // {
                    //     throw new Exception("Server currently has no data");
                    // }

                    // foreach (var file in listFile)
                    // {
                    //     logger.LogInformation($"File {file.Keys.First()}: {file.Values.First()} Kb");
                    // }
                }
                catch (Exception ex)
                {
                    logger.LogInformation(new String('*', 50));
                    logger.LogCritical(ex.Message);
                }
                await Task.Delay(ThreadDelay* 60000, stoppingToken);
            }
        }
    }
}
