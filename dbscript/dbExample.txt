-- =============================================
-- База данных: ispp41
-- Система торгового терминала для ТЦ
-- =============================================

USE ispp41;
GO

-- Удаляем все таблицы, если они существуют (в правильном порядке из-за внешних ключей)
IF OBJECT_ID('OrderItems', 'U') IS NOT NULL DROP TABLE OrderItems;
IF OBJECT_ID('Orders', 'U') IS NOT NULL DROP TABLE Orders;
IF OBJECT_ID('OrderStatuses', 'U') IS NOT NULL DROP TABLE OrderStatuses;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('Roles', 'U') IS NOT NULL DROP TABLE Roles;
IF OBJECT_ID('Products', 'U') IS NOT NULL DROP TABLE Products;
IF OBJECT_ID('Categories', 'U') IS NOT NULL DROP TABLE Categories;
IF OBJECT_ID('Manufacturers', 'U') IS NOT NULL DROP TABLE Manufacturers;
GO

-- =============================================
-- Таблица: Производители (Manufacturers)
-- =============================================
CREATE TABLE Manufacturers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL UNIQUE
);
GO

-- =============================================
-- Таблица: Категории товаров (Categories)
-- =============================================
CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL UNIQUE
);
GO

-- =============================================
-- Таблица: Товары (Products)
-- =============================================
CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Article NVARCHAR(50) NOT NULL UNIQUE,
    Name NVARCHAR(500) NOT NULL,
    Unit NVARCHAR(20) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    Author NVARCHAR(200) NULL,
    ManufacturerId INT NULL,
    CategoryId INT NULL,
    Discount DECIMAL(5, 2) DEFAULT 0,
    StockQuantity INT DEFAULT 0,
    Description NVARCHAR(MAX) NULL,
    Photo NVARCHAR(200) NULL,
    
    CONSTRAINT FK_Products_Manufacturers FOREIGN KEY (ManufacturerId) 
        REFERENCES Manufacturers(Id) ON DELETE SET NULL,
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) 
        REFERENCES Categories(Id) ON DELETE SET NULL
);
GO

-- =============================================
-- Таблица: Роли пользователей (Roles)
-- =============================================
CREATE TABLE Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- =============================================
-- Таблица: Пользователи (Users)
-- =============================================
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoleId INT NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    Login NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) 
        REFERENCES Roles(Id)
);
GO

-- =============================================
-- Таблица: Статусы заказов (OrderStatuses)
-- =============================================
CREATE TABLE OrderStatuses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- =============================================
-- Таблица: Заказы (Orders)
-- =============================================
CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderNumber INT NOT NULL UNIQUE,
    UserId INT NULL,
    OrderDate DATETIME NOT NULL,
    DeliveryDate DATETIME NULL,
    StatusId INT NOT NULL,
    PickupCode CHAR(3) NOT NULL,
    TotalAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) 
        REFERENCES Users(Id) ON DELETE SET NULL,
    CONSTRAINT FK_Orders_OrderStatuses FOREIGN KEY (StatusId) 
        REFERENCES OrderStatuses(Id)
);
GO

-- =============================================
-- Таблица: Товары в заказе (OrderItems)
-- =============================================
CREATE TABLE OrderItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    PriceAtOrder DECIMAL(18, 2) NOT NULL,
    DiscountAtOrder DECIMAL(5, 2) NOT NULL DEFAULT 0,
    
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) 
        REFERENCES Orders(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) 
        REFERENCES Products(Id)
);
GO

-- =============================================
-- Вставка ролей
-- =============================================
INSERT INTO Roles (Name) VALUES 
    (N'Администратор'),
    (N'Менеджер'),
    (N'Авторизованный клиент');
GO

-- =============================================
-- Вставка статусов заказов
-- =============================================
INSERT INTO OrderStatuses (Name) VALUES 
    (N'Новый'),
    (N'В обработке'),
    (N'Завершен');
GO

-- =============================================
-- Вставка производителей
-- =============================================
INSERT INTO Manufacturers (Name) VALUES 
    (N'Яуза'),
    (N'Т8 Издательские технологии'),
    (N'Прогресс книга'),
    (N'Время'),
    (N'Лениздат'),
    (N'Неолит'),
    (N'Амрита-Русь'),
    (N'Златоуст'),
    (N'Аспект Пресс'),
    (N'ВКН');
GO

-- =============================================
-- Вставка категорий
-- =============================================
INSERT INTO Categories (Name) VALUES 
    (N'Художественная литература'),
    (N'Учебник для вузов'),
    (N'Хрестоматия'),
    (N'Учебное пособие');
GO

-- =============================================
-- Вставка товаров
-- =============================================
INSERT INTO Products (Article, Name, Unit, Price, Author, ManufacturerId, CategoryId, Discount, StockQuantity, Description, Photo) VALUES 
    (N'А112Т4', N'Прокляты и убиты', N'шт.', 585, N'Виктор Астафьев', 
     (SELECT Id FROM Manufacturers WHERE Name = N'Яуза'),
     (SELECT Id FROM Categories WHERE Name = N'Художественная литература'), 
     25, 6, 
     N'Роман-эпопею "Прокляты и убиты" Виктора Астафьева по праву считают одним из самых сильных и пронзительных произведений отечественной военной прозы.', 
     N'1.jpg'),
    
    (N'G843H5', N'Тайны и загадки отца Брауна', N'шт.', 193, N'Гилберт Кит Честертон',
     (SELECT Id FROM Manufacturers WHERE Name = N'Яуза'),
     (SELECT Id FROM Categories WHERE Name = N'Художественная литература'),
     30, 9,
     N'Гилберт Кит Честертон — признанный классик английской литературы, один из самых ярких писателей первой половины XX века. Классикой стали его романы и многочисленные эссе, однако любовь массового читателя принесли ему рассказы об отце Брауне, тихом, застенчивом священнике, мастерски раскрывающем наиболее запутанные загадки и преступления.',
     N'2.jpg'),
    
    (N'D325D4', N'Девайс', N'шт.', 1599, N'Кирилл Каланджи',
     (SELECT Id FROM Manufacturers WHERE Name = N'Т8 Издательские технологии'),
     (SELECT Id FROM Categories WHERE Name = N'Художественная литература'),
     5, 12,
     N'Молодой фрилансер Захар Скаро устраивается на очередную подработку. Задача, казалось бы, тривиальная: тестирование нового устройства. Вот только вопрос в том, тестированием какой реальности занимается этот новый Девайс?',
     N'3.jpg'),
    
    (N'S432T5', N'Необыкновенное обыкновенное чудо. Школьные истории', N'шт.', 549, N'Людмила Улицкая',
     (SELECT Id FROM Manufacturers WHERE Name = N'Т8 Издательские технологии'),
     (SELECT Id FROM Categories WHERE Name = N'Художественная литература'),
     15, 15, NULL, N'4.jpg'),
    
    (N'F325D4', N'Чук и Гек', N'шт.', 209, N'Аркадий Гайдар',
     (SELECT Id FROM Manufacturers WHERE Name = N'Т8 Издательские технологии'),
     (SELECT Id FROM Categories WHERE Name = N'Художественная литература'),
     18, 3,
     N'В книгу вошли повести и рассказы Аркадия Петровича Гайдара: "Чук и Гек", "Горячий камень" и "Сказка о военной тайне, о Мальчише-Кибальчише и его твердом слове"',
     N'5.jpg'),
    
    (N'G432G6', N'Информационная безопасность. Национальные стандарты Российской Федерации. 3-е издание. Учебное пособие', N'шт.', 3899, N'Юрий Родичев',
     (SELECT Id FROM Manufacturers WHERE Name = N'Прогресс книга'),
     (SELECT Id FROM Categories WHERE Name = N'Учебник для вузов'),
     22, 3,
     N'В учебном пособии рассмотрено более 300 действующих открытых документов национальной системы стандартизации Российской Федерации, включая международные и межгосударственные стандарты в области информационной безопасности по состоянию на начало 2023 года.',
     N'6.jpg'),
    
    (N'H542F5', N'Linux. Командная строка. Лучшие практики', N'шт.', 1799, N'Дэниел Джей Барретт',
     (SELECT Id FROM Manufacturers WHERE Name = N'Прогресс книга'),
     (SELECT Id FROM Categories WHERE Name = N'Учебник для вузов'),
     4, 5,
     N'Перейдите на новый уровень работы в Linux! Если вы системный администратор, разработчик программного обеспечения, SRE-инженер или пользователь Linux, книга поможет вам работать быстрее, элегантнее и эффективнее.',
     N'7.jpg'),
    
    (N'C346F5', N'Квантовые миры и возникновение пространства-времени', N'шт.', 1349, N'Шон Кэрролл',
     (SELECT Id FROM Manufacturers WHERE Name = N'Прогресс книга'),
     (SELECT Id FROM Categories WHERE Name = N'Учебник для вузов'),
     5, 4,
     N'Шон Кэрролл — физик-теоретик и один из самых известных в мире популяризаторов науки — заставляет нас по-новому взглянуть на физику. Столкновение с главной загадкой квантовой механики полностью поменяет наши представления о пространстве и времени.',
     N'8.jpg'),
    
    (N'F256G6', N'Вселенная. Происхождение жизни, смысл нашего существования и огромный космос', N'шт.', 1799, N'Шон Кэрролл',
     (SELECT Id FROM Manufacturers WHERE Name = N'Прогресс книга'),
     (SELECT Id FROM Categories WHERE Name = N'Учебник для вузов'),
     6, 2,
     N'Знаменитый физик Шон Кэрролл в свойственной ему увлекательной манере объясняет принципы, которые лежат в основах научных революций от Дарвина до Эйнштейна, и показывает как невероятные научные открытия последнего столетия изменили наш мир.',
     NULL),
    
    (N'J532V5', N'Пушкин. Бродский. Империя и судьба. В 2-х томах (комплект из 2-х книг)', N'шт.', 529, N'Яков Гордин',
     (SELECT Id FROM Manufacturers WHERE Name = N'Время'),
     (SELECT Id FROM Categories WHERE Name = N'Хрестоматия'),
     8, 6,
     N'Первая книга двухтомника «Пушкин. Бродский. Империя и судьба» пронизана пушкинской темой. Пушкин — «певец империи и свободы» — присутствует даже там, где он впрямую не упоминается, ибо его судьба, как и судьба других героев книги, органично связана с трагедией великой империи.',
     N'10.jpg'),
    
    (N'G643F4', N'Иосиф Бродский. Избранные эссе (комплект из 6-ти книг)', N'шт.', 4925, N'Иосиф Бродский',
     (SELECT Id FROM Manufacturers WHERE Name = N'Лениздат'),
     (SELECT Id FROM Categories WHERE Name = N'Хрестоматия'),
     2, 24,
     N'Шесть сборников избранных эссе Иосифа Бродского (1940-1996), великого поэта, драматурга, мыслителя, лауреата Нобелевской премии по литературе (1987): «Будущее или далекое прошлое», «Верь своей боли», «Как читать книгу», «О русской литературе», «О тирании», «Путеводитель по переименованному городу». Все тексты представлены на английском языке и в переводе на русский и открывают автора не только как поэта, но как историка, критика, и глубокого и ироничного мыслителя.',
     N'11.jpg'),
    
    (N'J326V5', N'Тысячелетие императорской керамики', N'шт.', 2599, N'Янь Чуннянь',
     (SELECT Id FROM Manufacturers WHERE Name = N'Лениздат'),
     (SELECT Id FROM Categories WHERE Name = N'Хрестоматия'),
     5, 4,
     N'Фарфор стал величайшим символом китайской культуры. Это одно из выдающихся изобретений, внесших неоценимый вклад в мировую цивилизацию.',
     N'12.jpg'),
    
    (N'J632F6', N'Вечные спутники: Портреты из всемирной литературы', N'шт.', 1599, N'Дмитрий Мережковский',
     (SELECT Id FROM Manufacturers WHERE Name = N'Лениздат'),
     (SELECT Id FROM Categories WHERE Name = N'Хрестоматия'),
     0, 6,
     N'Книга "Вечные спутники" - это цикл критических очерков о культуре и великих литераторах, сопровождавших жизнь и творчество русского писателя, поэта, литературного критика и общественного деятеля Дмитрия Мережковского (1865–1941).',
     N'13.jpg'),
    
    (N'G632H6', N'Формирование литературной репутации Н.Г.Чернышевского в XIX-XXI веках', N'шт.', 1349, N'Дмитрий Щербаков',
     (SELECT Id FROM Manufacturers WHERE Name = N'Неолит'),
     (SELECT Id FROM Categories WHERE Name = N'Хрестоматия'),
     2, 8,
     N'Монография Д. А. Щербакова - новаторская. Поэтапно рассмотрены не только многочисленные суждения известных отечественных и зарубежных критиков, литературоведов, философов и политиков, различным образом характеризовавших Н. Г. Чернышевского в связи и вне связи со знаменитым романом "Что делать?',
     N'14.jpg'),
    
    (N'M642E5', N'Теория искусства. Краткий путеводитель', N'шт.', 879, N'Роджер Осборн, Дэн Стерджис',
     (SELECT Id FROM Manufacturers WHERE Name = N'Неолит'),
     (SELECT Id FROM Categories WHERE Name = N'Хрестоматия'),
     3, 2, NULL, N'15.jpg'),
    
    (N'G543F5', N'Религиозные верования с древнейших времен до наших дней', N'шт.', 879, N'Дмитрий Щербаков',
     (SELECT Id FROM Manufacturers WHERE Name = N'Амрита-Русь'),
     (SELECT Id FROM Categories WHERE Name = N'Хрестоматия'),
     4, 6,
     N'Настоящее издание представляет собой сборник переводов лекций по истории дохристианских и нехристианских религий, прочитанных в Лондоне в период с 1888 по 1891 гг. авторитетными исследователями данного раздела религиоведения.',
     N'16.jpg'),
    
    (N'B653G6', N'Русский язык: Первые шаги. Часть 3. Учебное пособие', N'шт.', 2699, N'Любовь Беликова, Инна Ерофеева, Татьяна Шутова',
     (SELECT Id FROM Manufacturers WHERE Name = N'Златоуст'),
     (SELECT Id FROM Categories WHERE Name = N'Учебное пособие'),
     8, 9,
     N'Пособие является завершающей частью учебного комплекса. Третья часть содержит 10 уроков (21-30, последний-повторительный). Усвоение лексико-грамматического материала рассчитано примерно на 200-240 часов аудиторных занятий.',
     N'17.jpg'),
    
    (N'J735J7', N'Синтетический образ индивидуального психического мира', N'шт.', 1099, N'Сергей Моргачев',
     (SELECT Id FROM Manufacturers WHERE Name = N'Златоуст'),
     (SELECT Id FROM Categories WHERE Name = N'Хрестоматия'),
     9, 4,
     N'Психика подобна определенным объектам, это фиксируют сами люди в языке и искусстве. В данном исследовании рассматриваются в этом плане образы сосуда, воронки, дерева и крепости.',
     N'18.jpg'),
    
    (N'H436H7', N'Английский язык в спорте: Учебное пособие', N'шт.', 1999, N'Екатерина Габарта, Ирина Игнатьева',
     (SELECT Id FROM Manufacturers WHERE Name = N'Аспект Пресс'),
     (SELECT Id FROM Categories WHERE Name = N'Учебное пособие'),
     2, 0,
     N'Учебное пособие подготовлено для слушателей, изучающих английский язык как язык специальности',
     N'19.jpg'),
    
    (N'H475R5', N'Лексика и грамматика современного китайского языка (к тому II учебника «Новый практический курс китайского языка» под редакцией Лю Сюня): учебное пособие', N'шт.', 608, N'Татьяна Лопаткина, Софья Маннапова',
     (SELECT Id FROM Manufacturers WHERE Name = N'ВКН'),
     (SELECT Id FROM Categories WHERE Name = N'Учебное пособие'),
     25, 12,
     N'Пособие выступает дополнением ко второму тому учебника «Новый практический курс китайского языка» (под редакцией Лю Сюня).',
     N'20.jpg');
GO

-- =============================================
-- Вставка пользователей
-- =============================================
DECLARE @AdminRoleId INT = (SELECT Id FROM Roles WHERE Name = N'Администратор');
DECLARE @ManagerRoleId INT = (SELECT Id FROM Roles WHERE Name = N'Менеджер');
DECLARE @ClientRoleId INT = (SELECT Id FROM Roles WHERE Name = N'Авторизованный клиент');

INSERT INTO Users (RoleId, FullName, Login, Password) VALUES 
    (@AdminRoleId, N'Никифорова Анна Семеновна', N'94d5ous@gmail.com', N'uzWC67'),
    (@AdminRoleId, N'Стелина Евгения Петровна', N'uth4iz@mail.com', N'2L6KZG'),
    (@AdminRoleId, N'Михайлюк Анна Вячеславовна', N'5d4zbu@tutanota.com', N'rwVDh9'),
    (@ManagerRoleId, N'Ситдикова Елена Анатольевна', N'ptec8ym@yahoo.com', N'LdNyos'),
    (@ManagerRoleId, N'Ворсин Петр Евгеньевич', N'1qz4kw@mail.com', N'gynQMT'),
    (@ManagerRoleId, N'Старикова Елена Павловна', N'4np6se@mail.com', N'AtnDjr'),
    (@ClientRoleId, N'Никифорова Весения Николаевна', N'yzls62@outlook.com', N'JlFRCZ'),
    (@ClientRoleId, N'Сазонов Руслан Германович', N'1diph5e@tutanota.com', N'8ntwUp'),
    (@ClientRoleId, N'Одинцов Серафим Артёмович', N'tjde7c@yahoo.com', N'YOyhfR'),
    (@ClientRoleId, N'Степанов Михаил Артёмович', N'wpmrc3do@tutanota.com', N'RSbvHv');
GO

-- =============================================
-- Вставка заказов
-- =============================================
DECLARE @StatusNew INT = (SELECT Id FROM OrderStatuses WHERE Name = N'Новый');
DECLARE @StatusProcessing INT = (SELECT Id FROM OrderStatuses WHERE Name = N'В обработке');
DECLARE @StatusCompleted INT = (SELECT Id FROM OrderStatuses WHERE Name = N'Завершен');

-- Заказ №1
INSERT INTO Orders (OrderNumber, UserId, OrderDate, DeliveryDate, StatusId, PickupCode, TotalAmount)
SELECT 1, 
    (SELECT Id FROM Users WHERE FullName = N'Степанов Михаил Артёмович'),
    '2025-02-27 00:00:00',
    '2025-04-20 00:00:00',
    @StatusCompleted,
    '901',
    0;

DECLARE @OrderId INT = SCOPE_IDENTITY();

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 2, Price, Discount
FROM Products WHERE Article = N'А112Т4';

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 2, Price, Discount
FROM Products WHERE Article = N'G843H5';

UPDATE Orders SET TotalAmount = (
    SELECT SUM(Quantity * PriceAtOrder * (1 - DiscountAtOrder/100))
    FROM OrderItems WHERE OrderId = @OrderId
) WHERE Id = @OrderId;

-- Заказ №2
INSERT INTO Orders (OrderNumber, UserId, OrderDate, DeliveryDate, StatusId, PickupCode, TotalAmount)
SELECT 2,
    (SELECT Id FROM Users WHERE FullName = N'Никифорова Весения Николаевна'),
    '2025-03-28 00:00:00',
    '2025-04-21 00:00:00',
    @StatusProcessing,
    '789',
    0;

SET @OrderId = SCOPE_IDENTITY();

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 1, Price, Discount
FROM Products WHERE Article = N'G843H5';

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 1, Price, Discount
FROM Products WHERE Article = N'А112Т4';

UPDATE Orders SET TotalAmount = (
    SELECT SUM(Quantity * PriceAtOrder * (1 - DiscountAtOrder/100))
    FROM OrderItems WHERE OrderId = @OrderId
) WHERE Id = @OrderId;

-- Заказ №3
INSERT INTO Orders (OrderNumber, UserId, OrderDate, DeliveryDate, StatusId, PickupCode, TotalAmount)
SELECT 3,
    NULL,
    '2026-02-20 00:00:00',
    '2026-04-22 00:00:00',
    @StatusCompleted,
    '852',
    0;

SET @OrderId = SCOPE_IDENTITY();

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 10, Price, Discount
FROM Products WHERE Article = N'D325D4';

UPDATE Orders SET TotalAmount = (
    SELECT SUM(Quantity * PriceAtOrder * (1 - DiscountAtOrder/100))
    FROM OrderItems WHERE OrderId = @OrderId
) WHERE Id = @OrderId;

-- Заказ №4
INSERT INTO Orders (OrderNumber, UserId, OrderDate, DeliveryDate, StatusId, PickupCode, TotalAmount)
SELECT 4,
    NULL,
    '2026-03-01 00:00:00',
    '2026-04-23 00:00:00',
    @StatusProcessing,
    '458',
    0;

SET @OrderId = SCOPE_IDENTITY();

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 5, Price, Discount
FROM Products WHERE Article = N'F325D4';

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 4, Price, Discount
FROM Products WHERE Article = N'D325D4';

UPDATE Orders SET TotalAmount = (
    SELECT SUM(Quantity * PriceAtOrder * (1 - DiscountAtOrder/100))
    FROM OrderItems WHERE OrderId = @OrderId
) WHERE Id = @OrderId;

-- Заказ №5
INSERT INTO Orders (OrderNumber, UserId, OrderDate, DeliveryDate, StatusId, PickupCode, TotalAmount)
SELECT 5,
    NULL,
    '2026-03-17 00:00:00',
    '2026-04-24 00:00:00',
    @StatusCompleted,
    '905',
    0;

SET @OrderId = SCOPE_IDENTITY();

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 20, Price, Discount
FROM Products WHERE Article = N'G432G6';

UPDATE Orders SET TotalAmount = (
    SELECT SUM(Quantity * PriceAtOrder * (1 - DiscountAtOrder/100))
    FROM OrderItems WHERE OrderId = @OrderId
) WHERE Id = @OrderId;

-- Заказ №6
INSERT INTO Orders (OrderNumber, UserId, OrderDate, DeliveryDate, StatusId, PickupCode, TotalAmount)
SELECT 6,
    (SELECT Id FROM Users WHERE FullName = N'Никифорова Весения Николаевна'),
    '2026-03-21 00:00:00',
    '2026-04-25 00:00:00',
    @StatusCompleted,
    '781',
    0;

SET @OrderId = SCOPE_IDENTITY();

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 2, Price, Discount
FROM Products WHERE Article = N'А112Т4';

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 2, Price, Discount
FROM Products WHERE Article = N'G843H5';

UPDATE Orders SET TotalAmount = (
    SELECT SUM(Quantity * PriceAtOrder * (1 - DiscountAtOrder/100))
    FROM OrderItems WHERE OrderId = @OrderId
) WHERE Id = @OrderId;

-- Заказ №7
INSERT INTO Orders (OrderNumber, UserId, OrderDate, DeliveryDate, StatusId, PickupCode, TotalAmount)
SELECT 7,
    NULL,
    '2026-03-31 00:00:00',
    '2026-04-26 00:00:00',
    @StatusCompleted,
    '128',
    0;

SET @OrderId = SCOPE_IDENTITY();

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 3, Price, Discount
FROM Products WHERE Article = N'C346F5';

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 3, Price, Discount
FROM Products WHERE Article = N'F256G6';

UPDATE Orders SET TotalAmount = (
    SELECT SUM(Quantity * PriceAtOrder * (1 - DiscountAtOrder/100))
    FROM OrderItems WHERE OrderId = @OrderId
) WHERE Id = @OrderId;

-- Заказ №8
INSERT INTO Orders (OrderNumber, UserId, OrderDate, DeliveryDate, StatusId, PickupCode, TotalAmount)
SELECT 8,
    (SELECT Id FROM Users WHERE FullName = N'Одинцов Серафим Артёмович'),
    '2026-04-02 00:00:00',
    '2026-04-27 00:00:00',
    @StatusNew,
    '908',
    0;

SET @OrderId = SCOPE_IDENTITY();

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 1, Price, Discount
FROM Products WHERE Article = N'F325D4';

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 1, Price, Discount
FROM Products WHERE Article = N'G432G6';

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 20, Price, Discount
FROM Products WHERE Article = N'H542F5';

UPDATE Orders SET TotalAmount = (
    SELECT SUM(Quantity * PriceAtOrder * (1 - DiscountAtOrder/100))
    FROM OrderItems WHERE OrderId = @OrderId
) WHERE Id = @OrderId;

-- Заказ №9
INSERT INTO Orders (OrderNumber, UserId, OrderDate, DeliveryDate, StatusId, PickupCode, TotalAmount)
SELECT 9,
    NULL,
    '2026-04-03 00:00:00',
    '2026-04-28 00:00:00',
    @StatusNew,
    '719',
    0;

SET @OrderId = SCOPE_IDENTITY();

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 5, Price, Discount
FROM Products WHERE Article = N'J532V5';

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 1, Price, Discount
FROM Products WHERE Article = N'F256G6';

UPDATE Orders SET TotalAmount = (
    SELECT SUM(Quantity * PriceAtOrder * (1 - DiscountAtOrder/100))
    FROM OrderItems WHERE OrderId = @OrderId
) WHERE Id = @OrderId;

-- Заказ №10
INSERT INTO Orders (OrderNumber, UserId, OrderDate, DeliveryDate, StatusId, PickupCode, TotalAmount)
SELECT 10,
    (SELECT Id FROM Users WHERE FullName = N'Степанов Михаил Артёмович'),
    '2026-05-30 00:00:00',
    '2026-04-29 00:00:00',
    @StatusNew,
    '910',
    0;

SET @OrderId = SCOPE_IDENTITY();

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 5, Price, Discount
FROM Products WHERE Article = N'F256G6';

INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder, DiscountAtOrder)
SELECT @OrderId, Id, 5, Price, Discount
FROM Products WHERE Article = N'J532V5';

UPDATE Orders SET TotalAmount = (
    SELECT SUM(Quantity * PriceAtOrder * (1 - DiscountAtOrder/100))
    FROM OrderItems WHERE OrderId = @OrderId
) WHERE Id = @OrderId;
GO