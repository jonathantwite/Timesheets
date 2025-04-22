# TimeAggregator

The `TimeAggregator` is a C# Console app that receives messages from the RabbitMQ message broker and aggregates the time entries into the total time that has been spent on a job.

The `NightlyCleanup` project is a azure function that runs every night and calculates the overtime that has been recorded by each user.