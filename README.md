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
docker-compose -f "docker-compose.yml" -f "docker-compose.override.yml" -p "incidents-search-service" up -d
```

## Проектирование

_Диаграммы можно сохранять и редактировать в ***[draw.io](https://app.diagrams.net/)***_

- ### Диаграмма прецедентов
     ![Компоненты микросервиса поиска](https://github.com/user-attachments/assets/3f7bb88a-0454-40c6-b5e5-92ef1e543657)

## Ссылки

- #### API-шлюз:  *https://github.com/ByeLarry/incidents-gateway*
- #### Сервис авторизации:  *https://github.com/ByeLarry/incidents-auth-service*
- #### Сервис марок (инцидентов): *https://github.com/ByeLarry/indcidents-marks-service*
- #### Клиентская часть:  *https://github.com/ByeLarry/incidents-frontend*
- #### Панель администратора *https://github.com/ByeLarry/incidents-admin-frontend.git*
- #### Демонастрация функционала пользовательской части версии 0.1.0: *https://youtu.be/H0-Qg97rvBM*
- #### Демонастрация функционала пользовательской части версии 0.2.0: *https://youtu.be/T33RFvfTxNU*
- #### Демонастрация функционала панели администратора версии 0.1.0: *https://youtu.be/7LTnEMYuzUo*
