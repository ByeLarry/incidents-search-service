# Сервис поиска

# Описание
Данный репозиторий содержит реализацию микросервиса поиска, входящего в состав проекта ***incidents***.
Используется фреймворк **ASP.NET Core** на платформе **.NET 8.0**. В качестве поискового движка задействован **Elasticsearch**.
Коммуникация с другими сервисами осуществляется с помощью **RabbitMQ**.

Когда сервисы запускаются, они отправляют массивы данных для индексации в **Elasticsearch**.
Во время выполнения CRUD-операций с данными сервисов происходит соответствующее обновление документов.

## Установка
### Docker 
```bash
# Переход в директорию с исходниками
cd solution

# Создание и запуск docker сервисов
docker-compose up -d
```

## Ссылки

- #### API-шлюз:  *https://github.com/ByeLarry/incidents-gateway*
- #### Сервис авторизации:  *https://github.com/ByeLarry/incidents-auth-service*
- #### Сервис марок (инцидентов): *https://github.com/ByeLarry/indcidents-marks-service*
- #### Клиентская часть:  *https://github.com/ByeLarry/incidents-frontend*
- #### Панель администратора *https://github.com/ByeLarry/incidents-admin-frontend.git*
- #### Демонастрация функционала версии 0.1.0: *https://youtu.be/H0-Qg97rvBM*