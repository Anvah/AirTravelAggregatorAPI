# AirTravelAggregatorAPI Сервис для бронирования билетов, который аггрегирует данные из двух внешних источников.
- Источники имеют разные API и модели. Для аггрегирования использован Mapster, как наиболее эффективный автоматизированный инструмент для маппинга объектов, хотя ввиду больших отличий в моделях, разумнее было бы писать его полностью вручную, если стандарты внутри компании и команды это допускают. Для работы с внешними API использован Refit.
Поскольку требований по архитектуре не было, использована стандартная 3-х уровневая архитектура. Также на мой взгляд для подобного небольшого сервиса вполне была бы уместна СQRS. Приложение выполнено как микросервис с внешней авторизацией. Авторизация происходит через JWT токен, получемый от соответсвующего сервиса.
- Доступно два метода: на получение списка перелетов с фильтацией и сортировкой и на бронирование.
Для получения билетов авторизация не требуется, чтобы любой потенциальный пользователь мог посмотреть перелеты прежде чем создавать аккаунт. Чтобы забронировать билет авторизация уже нужна. Клиент также может получить при запросе на получение и забронированные билеты. Это может понадобится например для реализации "подписки" на билет с получением уведомления если бронь будет снята.
- В сервисе реализован механизм кэширования по средствам MemoryCache. В дальнейшем в зависимости от масштабов системы и требований можно заменить данный сервис на распределенное кэширование с помощью Redis. Для наиболее оптимальной работы кэширования, несмотря на возможность получать уже отфильтрованные данные от источников, было принято решение запрашивать данные отфильтрованные только по дате, и использовать ее в качесте ключа для объекта кэша.Таким образом пользователь сможет просматривая билеты на конкретную дату применять различные фильтры без необходимости снова отправлять запросы к сервисам с данными и аггрегировать их. Время хранения кэша нужно будет настроить исходя из того, как часто оба сервиса могут обновлять данные, или же воспользоваться вебхуками, если сервисы их предоставляют. 
- Для возможности отмены запроса для каждого метода API проброшены cancelationToken, а в случае, если один из источников данных слишком долго обрабатывает ответ, запрос автоматически прерывается благодаря насройке Refit. На данный момент при таком исходе клиент просто не получит данные из этого источника, при необходимости можно будет дополнительно обработать каждую ситуацию.
- Для обработки ошибок создан ExceptionMeddleware, который на данный момент на каждую ошибку создает стандартую форму ответа для API и передает в нее информацию об ошибке. В дальнейшем можно усложнить эту логику и обрабатывать каждую ситуацию индивидуально.
Каждый ответ от API обернут в стандартую форму ответа, где есть тело ответа (если нужно), статус запроса (успешный или нет) и информация об ошибке. Это сделано с целью более удобной обработки на клиенте. В случае если запрашиваемы данные не найдены, клиент в данный также момент получает ошибку, а не пустой массив или объект. Это сделано чтобы данную ситацию в любом случе пришлось дополнительно обработать.
- В дальнейшем, если приложение предполагает развитие в виде микросервисной архитектуры данные объекты следует вынести в библиотеку, и подключать через локальный nuget, чтобы сохранить стандартизацию ответов во всей системе.

Проект развернут в docker-контейнерах в облаке Azure.
Примеры успешых запросов:

- Получение билетов:
https://flightagrregatorservice.azurewebsites.net/fligts/get?date=2024-03-16&sortProperty=ByPrice&maxPrice=15000&airlineName=Emirates&maxTransfersCount=3
- Бронирование:
https://flightagrregatorservice.azurewebsites.net/fligts/book/EK753/SecondFlightService
```
header:
    Authorization: Bearer yourtoken
```

- Авторизация:
https://flightserviceauthservice.azurewebsites.net/api/Auth/authenticate

```
body: 
{
    "username": "user1",
    "password": "password1"
}
```

- Создание аккаунта: 
https://flightserviceauthservice.azurewebsites.net/api/Auth/createAccount

```
body:
{ 
    "username": "user3", 
    "password": "password3"
}
```
Сущнности котырые иожно получить в результате запросов:
- Билеты из первого первого сервисa:
```
                new FirstFlight
                {
                    Id = "NYC-LON-PAR-1",
                    Airline = "Example Airways",
                    DeparturePoint = new FirstFlightTransfer
                    {
                        Airport = "JFK JFK JFK",
                        DepartureDataTime = DateTime.UtcNow,
                    },
                    ArrivalPoint = new FirstFlightTransfer
                    {
                        Airport = "LHR JFK JFK",
                        ArrivalDataTime = DateTime.UtcNow.AddHours(8),
                    },
                    Transfers = new FirstFlightTransfer[]
                    {
                        new FirstFlightTransfer
                        {
                            Airport = "CDG JFK JFK",
                            ArrivalDataTime = DateTime.UtcNow.AddHours(3),
                            DepartureDataTime = DateTime.UtcNow.AddHours(4),
                        }
                    },
                    Price = 1200.00m
                },
               new FirstFlight
               {
                   Id = "PAR-TYO-1",
                   Airline = "Example Airways",
                   DeparturePoint = new FirstFlightTransfer
                   {
                       Airport = "CDG JFK JFK",
                       DepartureDataTime = DateTime.UtcNow,
                   },
                   ArrivalPoint = new FirstFlightTransfer
                   {
                       Airport = "HND JFK JFK",
                       ArrivalDataTime = DateTime.UtcNow.AddHours(14),
                   },
                   Transfers = new FirstFlightTransfer[] { },
                   Price = 1800.00m
               },
               new FirstFlight
               {
                   Id = "LAX-SYD-AKL-1",
                   Airline = "Example Airways",
                   DeparturePoint = new FirstFlightTransfer
                   {
                       Airport = "LAX JFK JFK",
                       DepartureDataTime = DateTime.UtcNow,
                   },
                   ArrivalPoint = new FirstFlightTransfer
                   {
                       Airport = "SYD JFK JFK",
                       ArrivalDataTime = DateTime.UtcNow.AddHours(20),
                   },
                   Transfers = new FirstFlightTransfer[]
                    {
                        new FirstFlightTransfer
                        {
                            Airport = "AKL JFK JFK",
                            ArrivalDataTime = DateTime.UtcNow.AddHours(15),
                            DepartureDataTime = DateTime.UtcNow.AddHours(16),
                        }
                    },
                   Price = 2500.00m
               },
               new FirstFlight
               {
                   Id = "SHA-ICN-1",
                   Airline = "Example Airways",
                   DeparturePoint = new FirstFlightTransfer
                   {
                       Airport = "PVG JFK JFK",
                       DepartureDataTime = DateTime.UtcNow,
                   },
                   ArrivalPoint = new FirstFlightTransfer
                   {
                       Airport = "ICN JFK JFK",
                       ArrivalDataTime = DateTime.UtcNow.AddHours(2),
                   },
                   Transfers = new FirstFlightTransfer[] { },
                   Price = 900.00m
               }                
```
- Билеты из второго сервисa:
```
                new SecondFlight
                {
                    Id = "SU123",
                    Airline = "Aeroflot",
                    DepartureAirport = "SVO",
                    DepartureCity = "Moscow",
                    DepartureCountry = "Russia",
                    ArrivalAirport = "CDG",
                    ArrivalCity = "Paris",
                    ArrivalCountry = "France",
                    DepartureTime = "2024-03-16 09:00",
                    ArrivalTime = "2024-03-16 15:30",
                    Transfres = new string[] { "Frankfurt Frankfurt Frankfurt" },
                    TransfersArivalDateTime = new string[] { "2024-03-16 11:30" },
                    TransfersDepartureDateTime = new string[] { "2024-03-16 12:30" },
                    Price = 300.50m,
                    IsBaggageAvaible = true,
                    BaggagePrice = 25.00m
                },
                new SecondFlight
                {
                    Id = "DL456",
                    Airline = "Delta Airlines",
                    DepartureAirport = "JFK",
                    DepartureCity = "New York",
                    DepartureCountry = "USA",
                    ArrivalAirport = "LAX",
                    ArrivalCity = "Los Angeles",
                    ArrivalCountry = "USA",
                    DepartureTime = "2024-03-16 08:30",
                    ArrivalTime = "2024-03-16 11:00",
                    Transfres = Array.Empty<string>(),
                    TransfersArivalDateTime = Array.Empty<string>(),
                    TransfersDepartureDateTime = Array.Empty<string>(),
                    Price = 200.00m,
                    IsBaggageAvaible = true,
                    BaggagePrice = 20.00m
                },
                new SecondFlight
                {
                    Id = "BA789",
                    Airline = "British Airways",
                    DepartureAirport = "LHR",
                    DepartureCity = "London",
                    DepartureCountry = "UK",
                    ArrivalAirport = "HND",
                    ArrivalCity = "Tokyo",
                    ArrivalCountry = "Japan",
                    DepartureTime = "2024-03-16",
                    ArrivalTime = "2024-03-17 08:00",
                    Transfres = new string[] { "Amsterdam Amsterdam Amsterdam", "Singapore Singapore Singapore" },
                    TransfersArivalDateTime = new string[] { "2024-03-16 13:30", "2024-03-17 02:30" },
                    TransfersDepartureDateTime = new string[] { "2024-03-16 14:30", "2024-03-17 04:30" },
                    Price = 500.00m,
                    IsBaggageAvaible = true,
                    BaggagePrice = 30.00m
                },
                new SecondFlight
                {
                    Id = "CA101",
                    Airline = "Air China",
                    DepartureAirport = "PEK",
                    DepartureCity = "Beijing",
                    DepartureCountry = "China",
                    ArrivalAirport = "PVG",
                    ArrivalCity = "Shanghai",
                    ArrivalCountry = "China",
                    DepartureTime = "2024-03-16 12:00",
                    ArrivalTime = "2024-03-16 14:30",
                    Transfres = Array.Empty<string>(),
                    TransfersArivalDateTime = Array.Empty<string>(),
                    TransfersDepartureDateTime = Array.Empty<string>(),
                    Price = 150.00m,
                    IsBaggageAvaible = false,
                    BaggagePrice = 0.00m
                },
                new SecondFlight
                {
                    Id = "QF321",
                    Airline = "Qantas Airways",
                    DepartureAirport = "SYD",
                    DepartureCity = "Sydney",
                    DepartureCountry = "Australia",
                    ArrivalAirport = "MEL",
                    ArrivalCity = "Melbourne",
                    ArrivalCountry = "Australia",
                    DepartureTime = "2024-03-16 07:30",
                    ArrivalTime = "2024-03-16 10:00",
                    Transfres = new string[] { "Canberra Canberra Canberra" },
                    TransfersArivalDateTime = new string[] { "2024-03-16 08:30" },
                    TransfersDepartureDateTime = new string[] { "2024-03-16 09:00" },
                    Price = 180.00m,
                    IsBaggageAvaible = true,
                    BaggagePrice = 15.00m
                },
                new SecondFlight
                {
                    Id = "IB567",
                    Airline = "Iberia",
                    DepartureAirport = "FCO",
                    DepartureCity = "Rome",
                    DepartureCountry = "Italy",
                    ArrivalAirport = "MAD",
                    ArrivalCity = "Madrid",
                    ArrivalCountry = "Spain",
                    DepartureTime = "2024-03-16 11:00",
                    ArrivalTime = "2024-03-16 14:00",
                    Transfres = Array.Empty<string>(),
                    TransfersArivalDateTime = Array.Empty<string>(),
                    TransfersDepartureDateTime = Array.Empty<string>(),
                    Price = 220.00m,
                    IsBaggageAvaible = true,
                    BaggagePrice = 25.00m
                },
                new SecondFlight
                {
                    Id = "LH654",
                    Airline = "Lufthansa",
                    DepartureAirport = "TXL",
                    DepartureCity = "Berlin",
                    DepartureCountry = "Germany",
                    ArrivalAirport = "MUC",
                    ArrivalCity = "Munich",
                    ArrivalCountry = "Germany",
                    DepartureTime = "2024-03-16 11:00",
                    ArrivalTime = "2024-03-16 14:00",
                    Transfres = new string[] { "Frankfurt Frankfurt Frankfurt" },
                    TransfersArivalDateTime = new string[] { "2024-03-16 08:30" },
                    TransfersDepartureDateTime = new string[] { "2024-03-16 08:30" },
                    Price = 120.00m,
                    IsBaggageAvaible = true,
                    BaggagePrice = 20.00m
                },
                new SecondFlight
                {
                    Id = "IB987",
                    Airline = "Iberia",
                    DepartureAirport = "FCO",
                    DepartureCity = "Rome",
                    DepartureCountry = "Italy",
                    ArrivalAirport = "MAD",
                    ArrivalCity = "Madrid",
                    ArrivalCountry = "Spain",
                    DepartureTime = "2024-03-16 11:00",
                    ArrivalTime = "2024-03-16 14:00",
                    Transfres = new string[0],
                    TransfersArivalDateTime = new string[0],
                    TransfersDepartureDateTime = new string[0],
                    Price = 220.00m,
                    IsBaggageAvaible = true,
                    BaggagePrice = 25.00m
                },
                new SecondFlight
                {
                    Id = "EK753",
                    Airline = "Emirates",
                    DepartureAirport = "DXB",
                    DepartureCity = "Dubai",
                    DepartureCountry = "UAE",
                    ArrivalAirport = "DEL",
                    ArrivalCity = "Delhi",
                    ArrivalCountry = "India",
                    DepartureTime = "2024-03-16 11:00",
                    ArrivalTime = "2024-03-16 14:00",
                    Transfres = new string[] { "Mumbai Mumbai Mumbai" },
                    TransfersArivalDateTime = new string[] { "2024-03-16 08:30" },
                    TransfersDepartureDateTime = new string[] { "2024-03-16 08:30" },
                    Price = 280.00m,
                    IsBaggageAvaible = true,
                    BaggagePrice = 30.00m
                },
                new SecondFlight
                {
                    Id = "KE888",
                    Airline = "Korean Air",
                    DepartureAirport = "ICN",
                    DepartureCity = "Seoul",
                    DepartureCountry = "South Korea",
                    ArrivalAirport = "HKG",
                    ArrivalCity = "Hong Kong",
                    ArrivalCountry = "China",
                    DepartureTime = "2024-03-17 11:00",
                    ArrivalTime = "2024-03-16 14:00",
                    Transfres = new string[0],
                    TransfersArivalDateTime = new string[0],
                    TransfersDepartureDateTime = new string[0],
                    Price = 200.00m,
                    IsBaggageAvaible = true,
                    BaggagePrice = 25.00m
                }
```
- пользователи из автаризационнгого сервиса:
```
            new User { Id = 1, Username = "user1", Password = "password1", Roles = new List<string> { "Admin" } },
            new User { Id = 2, Username = "user2", Password = "password2", Roles = new List<string> { "User" } }
```
