# TradeTerminal - Торговый терминал

## О проекте

**TradeTerminal** — это комплексная система для торговых терминалов, устанавливаемых при входе в торговые центры. Система позволяет клиентам (авторизованным и неавторизованным) просматривать товары и оформлять заказы, а менеджерам и администраторам — управлять заказами.

---

## Основные возможности

### Для всех пользователей (Гости и авторизованные)
- Просмотр каталога товаров
- Поиск, фильтрация и сортировка товаров
- Формирование заказа (добавление товаров в корзину)

### Для авторизованных клиентов
- Отображение ФИО в интерфейсе
- Привязка заказов к пользователю

### Для менеджеров и администраторов
- Управление заказами
- Просмотр всех заказов
- Изменение статуса заказа
- Указание даты доставки

---

## Технологии

### Backend
- **.NET 9** — платформа разработки
- **ASP.NET Core Web API** — REST API
- **Entity Framework Core** — ORM
- **SQL Server** — база данных

### Desktop
- **WPF (.NET 9)** — Windows Presentation Foundation
- **XAML** — интерфейс

### Web
- **ASP.NET Core Razor Pages** — веб-интерфейс
- **HTML5 + CSS3** — верстка

---

## Руководство по стилю
Основной цвет #DAA520 (золотой) 
Акцентный цвет  #B8860B (темно-золотой) 
Цвет фона #FFFFFF (белый) 
Шрифт Calibri

## Установка и запуск

### Требования
- .NET 9 SDK
- SQL Server
- Visual Studio 2022+ / VS Code / Rider

### 1. Клонирование репозитория

```bash
git clone https://github.com/your-username/TradeTerminal.git
cd TradeTerminal

### Выполните скрипт dbscript/db.sql в SQL Server Management Studio.
Строка подключения (в appsettings.json):
json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=TradeTerminal;Trusted_Connection=True;TrustServerCertificate=True"
  }
}

### Запуск Web API
cd TradeTerminal.WebApi
dotnet restore
dotnet build
dotnet run --launch-profile https
API будет доступно по адресу: https://localhost:7000

### Запуск Desktop приложения
cd TradeTerminal.Desktop
dotnet run

### Запуск Web приложения
cd TradeTerminal.Web
dotnet restore
dotnet build
dotnet run

## Разработчики
- Трапезников В.
- Лыжин С.


