Описание входных/выходных данных для api в файле api.docx

Запуск:
Использована СУБД Postgresql, схема Scheme.backup находится в корне репозитория
Строка подключения к БД находится в файле App.config
в файле HomeController.cs в  строке 
            _api.BaseAddress = new Uri("https://localhost:44321/");
нужно заменить адрес на Ваш.

Приложение разрабатывал пользуясь паттерном(?) MVC
В приложении есть 2 контроллера: HomeController и ValueController
ValueController - отвечает за функции api
HomeController - отправляет запросы к api и html документы с результатами этих запросов пользователю

