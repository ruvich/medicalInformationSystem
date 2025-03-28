Описание лабораторной работы №2
Введение
В данном задании необходимо реализовать медицинскую информационную систему.
Требования, макеты и функционал
Документация API - https://mis-api.kreosoft.space/swagger/index.html 
Для Frontend части: Для получения оценки 4+ достаточно использование стандартных стилей из библиотеки bootstrap, главное корректно повторить UX системы. Для оценки 5 читайте требования к доп. заданию. 

Диаграмма классов предметной области: https://docs.google.com/document/d/1aWehfTOY9S-M5WnLujPcFHy5X3x8vgkRPenTn8cv_Cs/edit?hl=ru&tab=t.0
 
Поскольку множество функционала сайта завязано на пользователе, необходимо предусмотреть возможность регистрации пользователей и их дальнейшей авторизации. В системе предусмотрена регистрация и авторизация только для пользователей, являющихся врачами.
Авторизация
 
Адрес, который должен быть у вас в адресной строке при открытии страницы авторизации: http://localhost/login/
По данному адресу нужно отправить POST запрос с необходимыми данными. В ответ будет отправлен TOKEN авторизации зарегистрированного пользователя (если авторизация прошла успешно).
Для авторизации нужно использовать email как логин.
 
Обратите внимание: после успешной авторизации в навбаре справа кнопка “Вход” меняется на кнопку с ФИО пользователя, при нажатии на которую вываливается список с кнопками “Профиль” и “Выход”
Обратите внимание! Если имя пользователя слишком длинное, оно отображается не полностью.
Регистрация
 

Адрес, который должен быть у вас в адресной строке при открытии страницы регистрации: http://localhost/registration/
По данному адресу нужно отправить POST запрос с необходимыми данными. В ответ будет отправлен TOKEN авторизации зарегистрированного пользователя (если регистрация прошла успешно).
Обратите внимание, на страницах, где пользователь вводит номер телефона должна присутствовать валидация по следующей маске +7 (xxx) xxx-xx-xx. 
Профиль
 
Страница должна быть доступна по ссылке http://localhost/profile
Редактировать можно email, ФИО, номер телефона, пол и дату рождения
Список пациентов
 

На данной странице отображается список всех пациентов, зарегистрированных в системе. 
На больших экранах пациентов необходимо выводить в 2 колонки, при небольшом экране следует отображать данные в одну колонку.
 
Список пациентов на широком экране

Поскольку на сайте присутствует пагинация, нужно предусмотреть механизм, который не просто отправляет запрос на сервер с номером страницы и количеством элементов на ней и обновляет список пациентов, нужно сделать так, чтобы при изменении страницы, номер и размер отображался в URL и при перезагрузке страницы отображался корректный список постов для последней выбранной страницы. 
Как должен выглядеть URL при пагинации по блогу:
●	localhost/patients - первая страница, количество отображаемых пациентов 5 (по умолчанию)
●	localhost/patients/?page=1&pageSize=5 - также первая страница
●	localhost/patients/?page=N&pageSizeM - страница N, количество постов M
Чтобы реализовать подобный механизм, элементы пагинации должны быть не просто кнопками с обработчиками событий, а полноценными элементами навигации.
На сайте присутствует механизм фильтрации и сортировки пациентов по нескольким критериям, которые могут быть скомбинированы между собой:
●	поиск по имени пациента;
●	фильтрация по имеющимся заключениям осмотров (есть хотя бы один осмотр с данным заключением): Выздоровление, Болезнь, Смерть;
●	фильтрация по критерию “Есть запланированные визиты” (показывать только пациентов, у которых запланированы дальнейшие осмотры);
●	фильтрация по критерию “Мои пациенты” (показывать пациентов, у которых есть хотя бы 1 осмотр, сделанный данным врачом);
●	сортировка пациентов (по имени пациента (от А-Я, от Я-А), по дате создания (сначала новые, сначала старые), по дате осмотров (сначала новые осмотры, сначала старые осмотры)).
Примеры URL при пагинации и фильтрации:
●	localhost/patients/?page=1 - также первая страница
●	localhost/patients/?page=N - страница N
●	localhost/patients?name=Василий&conclusions=Disease&sorting=InspectionAsc&page=1&size=5
Для PHP уместно передавать множественные параметры через param[]=xxx, param[]=yyy, … 
●	localhost/patients/?sorting=CreateDesc
●	localhost/patients/?onlyMine=true
По желанию вы можете изменить отображение блока фильтрации и сортировки при помощи использования библиотек.
При нажатии на кнопку “Регистрация нового пациента” открывается модальное окно с возможностью зарегистрировать нового пациента. Для регистрации необходимо указать его ФИО, пол и дату рождения.  
 
При нажатии на карточку пациента  открывается страница “Карта пациента”. 
Карта пациента
Страница должна быть доступна по ссылке http://localhost/patient/{id}.
Данную страницу можно условно разделить на две части: данные пациента и список его осмотров. 
 
Обратите внимание! 
●	При большом размере экрана осмотры пациента отображаются в 2 колонки, при небольшом размере экрана контент показывается без разделения на колонки.
 Список осмотров на широком экране
●	Иконки пациентов отличаются в зависимости от пола (Мужской, Женский)
●	Осмотр с заключением “Смерть” выделяется цветом, пациент не может иметь дальнейших осмотров. 
На данной странице доступна пагинация осмотров, а также предусмотрен механизм фильтрации по следующим критериям, которые могут быть скомбинированы между собой:
●	фильтрация по корневым кодам МКБ-10 -  отображаются только те осмотры, основной диагноз которых входит в данную группу МКБ-10;
●	фильтрация по критерию “Сгруппировать по повторным” - при выборе данного критерия осмотры группируются в цепочки взаимосвязанных осмотров.
При выборе фильтра “Сгруппировать по повторным” у осмотров, имеющих цепочку появляется дополнительный UI-элемент, по нажатию на который будет показан следующий в цепочке осмотр. Обратите внимание, следующий в цепочке осмотр  имеет отступ от предыдущего (после 3 осмотра, осмотры можно выводить друг под другом).

 
При нажатии на кнопку “Добавить осмотр” открывается страница “Создания осмотра”.
Обратите внимание: на странице присутствует 2 вида кнопок “Добавить осмотр”:
●	расположена вверху страницы
 
●	расположен на карточке осмотра 
○	отображается на карточке осмотра только в том случае, если у осмотра еще нет дочерних осмотров (атрибут hasNested имеет значение false);
○	при нажатии - новый осмотр должен быть отмечен, как “Повторный осмотр”, а данный осмотр должен быть выбран, как предыдущий для создаваемого
 
При нажатии на кнопку “Детали осмотра” открывается страница “Детали осмотра”. 
Создание осмотра
Страница создания осмотра- localhost/inspection/create
 
Страница создания осмотра
Особенности создания осмотра:
●	запрещено создавать осмотры в будущем (дата создания осмотра не должна быть больше текущего времени), осмотр не может быть сделан ранее предыдущего осмотра в цепочке;
●	осмотр обязательно должен иметь один диагноз с типом диагноза “Основной”;
●	при выборе заключения “Болезнь”, необходимо указать дату и время следующего визита, при выборе заключения “Смерть”, необходимо указать дату и время смерти, при заключении “Выздоровление” эти данные вносить не требуется;
 
 

●	у пациента не может быть более одного осмотра с заключением “Смерть”;
●	осмотр не может иметь несколько консультаций с одинаковой специальностью врача;
●	при создании консультации врач-автор осмотра должен указать комментарий, описывающий проблему.
 
В select-поле Предыдущий осмотр” необходимо вывести дату осмотра и основной диагноз (см. /api/patient/{id}/inspections/search). 
Пример: 26.12.2023 10:00 J20 - Острый бронхит.

Форма выбора предыдущего осмотра
Детали осмотра
Страница  должна быть доступна по ссылке - localhost/inspection/{id}
 

При нажатии на кнопку редактирования осмотра должно быть открыто модальное окно, предоставляющее возможность отредактировать основные поля осмотра: жалобы, анамнез, рекомендации по лечению, заключение, диагнозы. Редактирование осмотра доступно только врачу, создавшему данный осмотр. Основные правила для валидации осмотра остаются аналогичны правилам создания осмотра.   
 
Обратите внимание на блоки консультации:
●	Изначально  карточка консультации содержит только информацию о требуемом специалисте,комментарий врача-автора осмотра и количество ответов врачей-специалистов. Если консультация имеет вложенные комментарии-ответы, то при нажатии на карточку должны быть показаны сообщения-ответы.
●	Возле комментариев, написанных пользователем, должны быть кнопки редактирования. При нажатии на кнопку изменения комментария, его текст должен замениться формой для редактирования текста этого комментария.
●	При нажатии на кнопку “Ответить” под комментарием должна появляться форма для написания ответа. Кнопка “Ответить” должна быть доступна врачу-автору осмотра и врачам, имеющим требуемую для консультации специальность.
●	Если комментарий был ранее изменен, то после его текста должна присутствовать пометка об этом. При наведении на эту пометку, должны отображаться дата и время последнего изменения.
 
Список консультаций
Страница  должна быть доступна по ссылке - localhost/consultations
На данной странице отображаются осмотры, содержащие консультации, соответствующие специальности пользователя.
На больших экранах осмотры стоит выводить в 2 колонки, при небольшом экране следует отображать данные в одну колонку.
 
Страница “Консультации” на широком экране
 
Страница “Консультации” на небольшом экране
В системе предусмотрена пагинация осмотров, а также имеется механизм фильтрации по следующим критериям, которые могут быть скомбинированы между собой:
●	фильтрация по корневым кодам МКБ-10 -  отображаются только те осмотры, основной диагноз который входит в данную группу МКБ-10;
●	фильтрация по критерию “Сгруппировать по повторным” - при выборе данного критерия осмотры группируются в цепочки взаимосвязанных осмотров.
По нажатию на карточку осмотра открывается страница “Детали осмотра”.
Генерация отчёта
Страница  должна быть доступна по ссылке - localhost/reports
На данной странице содержится форма для генерации отчета - статистики осмотров.
В фильтрах отчета задается диапазон дат для построения отчета и выбираются корневые элементы справочника МКБ-10. Если корневые элементы не были выбраны, то отчет строится без фильтрации по корням справочника МКБ-10 (то есть, по всем осмотрам в системе, попадающим в заданный диапазон дат).
 
После получения данных отчета от backend, необходимо вывести его в виде таблицы на данной странице под формой с выбранными параметрами фильтрации (дизайн таблицы необходимо разработать самостоятельно - это тоже часть задания).
Каждая колонка отчета - это корень МКБ-10.
Каждая строка отчета представляет статистику посещений пациента за выбранный период.
Учитывая вышесказанное, в ячейке отчета содержится количество посещений пациента, связанных с болезнью из поддерева корня МКБ-10, за выбранный период.
Ответ от сервера содержит выбранные фильтры для формирования отчета, строки отчета, отсортированные по имени пациента и итог всего отчета с разбивкой по корням МКБ-10. Корни в объекте фильтров и в объекте итогов должны быть отсортированы по коду МКБ-10. Детальнее смотрите документацию и ответы сервера в swagger /api/report/icdrootsreport
Справочник МКБ-10
Для работы с диагнозами пациентов и осмотрами в данной системе необходимо использовать справочник МКБ-10. Актуальные данные справочника можно получить по данной ссылке:
https://nsi.rosminzdrav.ru/dictionaries/1.2.643.5.1.13.13.11.1005/passport/latest
Также по ссылке доступно скачивание справочника в разных форматах, что вы можете использовать для импорта.
В системе должен быть предусмотрен эндпоинт для поиска по справочнику. В качестве поискового запроса передается строка, которая может содержать, как полный код записи из справочника, так и по названию диагноза.
Подсказка. Справочник МКБ-10 - это древовидная структура. Подумайте как адаптировать его для задачи фильтрации осмотров по корням МКБ-10.
Frontend
Для получения максимальной оценки за работу необходимо реализовать описанный выше функционал системы, а также выполнить задание, описанное в этом разделе. 

Для реализации стилей использовать препроцессор Less (https://lesscss.org/), не использовать библиотеку bootstrap, реализовать необходимые классы для грида самостоятельно. Для сборки less файлов в единый bundle использовать Gulp (https://gulpjs.com/) 

Повторить внешний вид и стилистику системы как показано на макетах в основном задании.
Backend
.Net -  рассылка сообщений
Необходимо реализовать функционал отправки email-уведомлений врачам о пациентах пропустивших запланированное посещение. При реализации данного функционала необходимо добиться отказоустойчивости (подумайте, как добиться того, чтобы уведомления не были утеряны и доставлены адресату при сбоях)
Рекомендуемые технологии:
●	Quartz.NET - для реализации фоновых задач (https://www.quartz-scheduler.net/)
●	MailDev - для тестирования функционала отправки email (https://github.com/maildev/maildev), либо может быть использован настоящий SMTP-сервер.
При выборе иных технологий рекомендуем проконсультироваться и утвердить их с преподавателями направления.
PHP
Необходимо реализовать функционал отправки заявки на запись к врачу с использованием очереди RabbitMQ  и SMTP клиента. Задание предполагает следующий порядок выполнения запроса: пользователь отправляет заявку на запись, которая содержит дату+время и id врача, все заявки попадают в очередь, далее слушатель достает из очереди заявки и отправляет их на почту врачу с помощью SMTP. 
Для добавления библиотеки RabbitMQ (https://github.com/php-amqplib/php-amqplib) понадобится освоить работу с composer в PHP.
С примерами работы очередей RabbitMQ можно ознакомиться по ссылке (https://www.rabbitmq.com/tutorials/tutorial-one-php.html) 
Тестовые пользователи
В демо-приложении присутствует ряд заготовленных пользователей:

Email	Pass	FullName
testuser1@example.com	testuser1@example.com	Доктор Акушер-гинеколог
testuser2@example.com	testuser2@example.com	Доктор Анестезиолог-реаниматолог
testuser3@example.com	testuser3@example.com	Доктор Дерматовенеролог
testuser4@example.com	testuser4@example.com	Доктор Инфекционист
testuser5@example.com	testuser5@example.com	Доктор Кардиолог
testuser6@example.com	testuser6@example.com	Доктор Невролог
testuser7@example.com	testuser7@example.com	Доктор Онколог
testuser8@example.com	testuser8@example.com	Доктор Отоларинголог (имеет осмотры, консультацию)
testuser9@example.com	testuser9@example.com	Доктор Офтальмолог
testuser10@example.com	testuser10@example.com	Доктор Психиатр
testuser11@example.com	testuser11@example.com	Доктор Психолог
testuser12@example.com	testuser12@example.com	Доктор Рентгенолог
testuser13@example.com	testuser13@example.com	Доктор Стоматолог
testuser14@example.com	testuser14@example.com	Доктор Терапевт (имеет осмотры)
testuser15@example.com	testuser15@example.com	Доктор УЗИ-специалист
testuser16@example.com	testuser16@example.com	Доктор Уролог
testuser17@example.com	testuser17@example.com	Доктор Хирург
testuser18@example.com	testuser18@example.com	Доктор Эндокринолог

Система оценивания
Важно отметить, что каждый аспект, описанный ниже оценивается не бинарно, а в промежутке [0; maxi], где maxi это максимальное значение для данного критерия (указывается в квадратных скобках). 
Для ускорения оценки, во время очной сдачи сразу указывайте проверяющему преподавателю, какие разделы вами не сделаны ни в каком виде, чтобы сразу проставить “0” и сосредоточиться на проверке реализованного. 
Для клиентской части
Для серверной части
PHP

.Net
Разработка и представление результатов
Во время работы, необходимо использовать выданный GIT-репозиторий (на сайте https://git.hits.tsu.ru) и постоянно его обновлять.
