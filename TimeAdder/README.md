# TimeAdder

The `TimeAdder` application is a web application that allows a user to add time to their website.

The application is a SPA website that communicates to the `TimeAdder.API` web API.  That project then broadcasts a message via the RabbitMQ message broker which will be picked up by the `TimeRecorder` and `TimeAggregator` applications.