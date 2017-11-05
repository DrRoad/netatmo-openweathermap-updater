# NetAtmo-OpenWeatherMap-Updater
Azure Functions app to get data from a NetAtmo weather station and update it to OpenWeatherMap.

## Architecture

The application consists of two independent functions

1. **GetMeasurement**
  This function accesses the latest weather measurement from your NetAtmo weather station and publishes it to a Storage Queue. As a secondary function it posts a simple JSON message to another queue where a Logic App posts it to Slack. You can disable this by setting POST_TO_SLACK environment variable to "false". The function also stores each NetAtmo measurements message to Table Storage for later analysis.

2. **PostMeasurement**
  This functions listens on the Storage Queue. When a new measurement is published in the queue it processes it and posts the data to OpenWeatherMap.

## Environment
The application requires you to have registed an application with the [NetAtmo developer portal](https://dev.netatmo.com/myaccount/) as well as the [OpenWeatherMap API](https://home.openweathermap.org/api_keys).  For easier access to both APIs you can use my Postman Collections at https://github.com/riussi/postman-collections

You need to set the following environment variables for your Function App for it to work:

```
NETATMO_CLIENT_ID  == Client Id for your NetAtmo application
NETATMO_CLIENT_SECRET == Client secret for your NetAtmo application
NETATMO_EMAIL == your NetAtmo email address
NETATMO_PASSWORD == your NetAtmo password
NETATMO_STATION_ID == The station id for your NetAtmo station

QUEUE_MEASUREMENTS == Queue name used to publish measurements between the two functions
TABLE_MEASUREMENTS == Table Storage table name where to store each NetAtmo measurement message

OPENWEATHERMAP_KEY == OpenWeatherMap API key
OPENWEATHERMAP_STATION_ID == Station Id you get back when you create your station first via the API.

POST_TO_SLACK == true if you want to publish messages to the Slack queue as well. Notice: this only publishes a JSON-message to the queue. You still need to process the message with a Logic App for example.
QUEUE_SLACK == Queue name to use to publish the measurement JSON-message for later processing.
```

