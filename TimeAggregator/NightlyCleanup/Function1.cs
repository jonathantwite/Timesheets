using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using TimeAggregator.Services;

namespace NightlyCleanup
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly ITimeAggregatorService _timeAggregatorService;

        public Function1(ILoggerFactory loggerFactory, ITimeAggregatorService timeAggregatorService)
        {
            _timeAggregatorService = timeAggregatorService ?? throw new ArgumentNullException(nameof(timeAggregatorService));
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        public void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            Task.Run(async () => await _timeAggregatorService.CleanUpAsync());
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
