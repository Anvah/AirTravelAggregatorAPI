# AirTravelAggregatorAPI
Сервис для бронирования билетов, который аггрегирует данные из двух внешних источников.
Источники имеют разные API и модели. Для аггрегирования использован Mapster, как наиболее эффективный автоматизированный инструмент для маппинга объектов, хотя ввиду больших отличий в моделях, разумнее было бы писать его полностью вручную, если стандарты внутри компании и команды это допускают.
Поскольку требований по архитектуре не было, использована стандартная 3-х уровневая архитектура. Также на мой взгляд для подобного небольшого сервиса вполне была бы уместна SQRS.
Приложение выполнено как микросервис с внешней авторизацией.
Доступно два метода: на получение списка перелетов с фильтацией и сортировкой и на бронирование.
Для получения билетов авторизация не требуется, чтобы любой потенциальный пользователь мог посмотреть перелеты прежде чем создавать аккаунт.
Чтобы забронировать билет авторизация уже разумеется нужна.
В сервисе реализован механизм кэширования по средствам MemoryCache. В дальнейшем в зависимости от масштабов системы и требований можно заменить данный сервис на распределенное кэширование с помощью Redis.
Для наиболее оптимальной работы кэширования, несмотря на возможность получать уже отфильтрованные данные от источников, было принято решение запрашивать данные отфильтрованные только по дате, и использовать ее в качесте ключа для объекта кэша.
Таким образом пользователь сможет просматривая билеты на конкретную дату применять различные фильтры без необходимости снова отправлять запросы к сервисам с данными и аггрегировать их. 
Время хранения кэша нужно будет настроить исходя из того, как часто оба сервиса могут обновлять данные, или же воспользоваться вебхуками, если сервисы их предоставляют. 
Для возможности отмены запроса для каждого метода API проброшены cancelationToken, а в случае, если один из источников данных слишком долго обрабатывает ответ, запрос автоматически прерывается.
На данный момент при таком исходе клиент просто не получит данные из этого источника, при необходимости можно будет дополнительно обработать каждую ситуацию.
Для обработки ошибок создан ExceptionMeddleware, который на данный момент на каждую ошибку создает стандартую форму ответа для API и передает в нее информацию об ошибке. В дальнейшем можно усложнить эту логику и обрабатывать каждую ситуацию индивидуально.
Каждый ответ от API обернут в стандартую форму ответа, где есть тело ответа (если нужно), статус запроса (успешный или нет) и информация об ошибке. Это сделано с целью более удобной обработки на клиенте.
В дальнейшем, если приложение предполагает развитие в виде микросервисной архитектуры данные объекты следует вынести в библиотеку, и подключать через локальный nuget, чтобы сохранить стандартизацию ответов во всей системе.

Проект развернут в docker-контейнерах в облаке Azure.
Примеры успешых запросов:

Получение билетов:
https://flightagrregatorservice.azurewebsites.net/fligts/get?date=2024-03-16&sortProperty=ByPrice&maxPrice=15000&airlineName=Emirates&maxTransfersCount=3

Бронирование:
https://flightagrregatorservice.azurewebsites.net/fligts/book/EK753/SecondFlightService

Авторизация:
https://flightserviceauthservice.azurewebsites.net//api/Auth/authenticate
{
    "username": "user1",
    "password": "password1"
}
Создание аккаунта: https://flightserviceauthservice.azurewebsites.net//api/Auth/createAccount 
{ 
    "username": "user3", 
    "password": "password3"
}
