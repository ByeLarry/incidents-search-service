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
cd IncidentsSearchSolution

# Создание и запуск docker сервисов
docker-compose -f "docker-compose.yml" -f "docker-compose.override.yml" -p "incidents-search-service" up -d
```

## Проектирование

_Диаграммы можно сохранять и редактировать в ***[draw.io](https://app.diagrams.net/)***_

- ### Компоненты микросервиса поиска
     ![Компоненты микросервиса поиска](https://github.com/user-attachments/assets/5536543f-f588-4772-a20c-a1f001643201)

## Ссылки

### Репозитории
- #### Клиентская часть:  *https://github.com/ByeLarry/incidents-frontend*  [![incidents-frontend](https://github.com/ByeLarry/incidents-frontend/actions/workflows/incidents-frontend.yml/badge.svg)](https://github.com/ByeLarry/incidents-frontend/actions/workflows/incidents-frontend.yml)
- #### API-шлюз:  *https://github.com/ByeLarry/incidents-gateway*  [![incidents-gateway](https://github.com/ByeLarry/incidents-gateway/actions/workflows/incidents-gateway.yml/badge.svg)](https://github.com/ByeLarry/incidents-gateway/actions/workflows/incidents-gateway.yml)
- #### Сервис авторизации:  *https://github.com/ByeLarry/incidents-auth-service*  [![incidents-auth](https://github.com/ByeLarry/incidents-auth-service/actions/workflows/incidents-auth.yml/badge.svg)](https://github.com/ByeLarry/incidents-auth-service/actions/workflows/incidents-auth.yml)
- #### Сервис марок (инцидентов): *https://github.com/ByeLarry/indcidents-marks-service*  [![incidents-marks](https://github.com/ByeLarry/incidents-marks-service/actions/workflows/incidents-marks.yml/badge.svg)](https://github.com/ByeLarry/incidents-marks-service/actions/workflows/incidents-marks.yml)
- #### Сервис поиска *https://github.com/ByeLarry/incidents-search-service*  [![incidents-search](https://github.com/ByeLarry/incidents-search-service/actions/workflows/incidents-search.yml/badge.svg)](https://github.com/ByeLarry/incidents-search-service/actions/workflows/incidents-search.yml)
- #### Панель администратора *https://github.com/ByeLarry/incidents-admin-frontend.git*  [![incidents-admin-frontend](https://github.com/ByeLarry/incidents-admin-frontend/actions/workflows/incidents-admin-frontend.yml/badge.svg)](https://github.com/ByeLarry/incidents-admin-frontend/actions/workflows/incidents-admin-frontend.yml)
- #### Сервис мониторинга состояния системы: *https://github.com/ByeLarry/incidents-healthcheck*  [![incidents-healthcheck](https://github.com/ByeLarry/incidents-healthcheck/actions/workflows/incidents-healthcheck.yml/badge.svg)](https://github.com/ByeLarry/incidents-healthcheck/actions/workflows/incidents-healthcheck.yml)
- #### Telegram бот для уведомления о состоянии системы *https://github.com/ByeLarry/incidents-healthcheck-bot*
- #### Сквозные (end-to-end) тесты *https://github.com/ByeLarry/incidents-playwright*

### Демонстрация функционала
- #### Демонстрация функционала пользовательской части версии 0.1.0: *https://youtu.be/H0-Qg97rvBM*
- #### Демонстрация функционала пользовательской части версии 0.2.0: *https://youtu.be/T33RFvfTxNU*
- #### Демонстрация функционала панели администратора версии 0.1.0: *https://youtu.be/7LTnEMYuzUo*
- #### Демонстрация функционала сервиса мониторинга состояния системы (панель администратора 0.1.1) *https://youtu.be/TeEc9W89igI*
