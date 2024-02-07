# PointLocationCalculator
## Описание
PointLocationCalculator - десктоп утилита, которая позволяет рассчитать, находится ли точка за пределами многоугольника, или в нем.
Метод решения задачи - _**трассировка лучей**_.
## Функционал
* Рассчет местоположения точки
* Импорт вершин многоугольника
* Изменение точности расчётов
## Инструкция
### Ввод вершин многоугольника
Вы можете ввести вершины многоугольника вручную или с помощью импорта.
> [!TIP]
> * Если вы хотите убрать последнюю вершину многоугольника, намжите кнопку "_**Убрать**_".
> * Если вы хотите удалить многоугольник с полотна, нажмите правой кнопкой мыши по полотну и выберите "_**Очистить**_".
#### Вручную
Под заголовком "Введите координаты точки многоугольника" расположены поля "_**X**_" и "_**Y**_"
1. В поле "_**X:**_" введите значение X точки вершины.
1. В поле "_**Y:**_" введите значение Y точки вершины.
1. Нажмите кнопку "_**Добавить**_".

На полотно добавится вершина многоугольника, чтобы построить грань, добавьте еще одну точку.
Чтобы построить многоугольник, точек должно быть больше 2.
#### Импорт
В левом верхнем углу расположено меню.

1. Нажмите "_**Импорт**_".
1. Далее нажмите "_**Выбрать файл**_".
1. Откроется диалоговое окно выбора файла.
1. Впишите название файла или выберите файл с форматом .csv.
1. Нажмите "_**Открыть**_".

Если с файлом все хорошо, программа выдаст сообщение "Импорт произошел успешно" и
отобразит многоугольник с заданными вершинами из файла.
### Ввод координат тестовой точки
Под заголовком "Введите координаты тестовой точки" расположены поля "X" и "Y"
1. В поле "_**X:**_" введите значение X тестовой точки.
1. В поле "_**Y:**_" введите значение Y тестовой точки.

Если координаты заданы правильно, на полотне отобразится красная точка, которая является тестовой точкой.
### Проверка местоположения точки
#### Основная часть
Если на полотне присутствует многоугольник с 3 и более вершинами, а также, тестовая точка, то можно запускать проверку.
> Если многоугольник состоит из 2 и менее вершин, программа фактически не будет выполнять проверку, а скажет, что точка находится снаружи

1. Для этого, под заголовком "Введите координаты тестовой точки" нажмите кнопку "_**Проверить**_".
1. Программа проанализирует местоположение точки и вернет результат.

Если точка находится снаружи, программа выдаст сообщение "Точка снаружи", если внутри - "Точка внутри".
#### Дополнительно
После завершения проверки, вы можете увидеть, как программа определяет местоположение с помощью "_**Отобразить трассировку**_".
У точки появятся лучи, с помощью которых программа понимает, где точка.

Точность расчетов отвечает за то, сколько лучей будет выпущено из точки. Изначально, точность равна 4 лучам, которые идут в 4 стороны от точки.
Необходимо это для того, чтобы быть уверенным, что в случае пересечения лучом вершины многоугольника программа не выдала неверный результат.
Если один луч скажет, что точка внутри, то остальные 3 независимых луча скажут обратное, то программа поверит мнению большинства.
## Как выглядит метод трассировки лучей в программе?
Метод трассировки лучей основан на пересечениях луча и граней многоугольника.

* Если количество пересечений кратно двум, то точка находится снаружи.
* Если пересечение одно, то точка внутри.

1. У нас имеется коллекция `PolygonPoints`, которая содержит вершины многоугольника.
1. Пара точек из коллекции `PolygonPoints` образует грань многоугольника `edge`
1. Нам необходимо пройтись по всем граням многоугольника и посчитать количество пересечений луча, идущего от точки  `testPoint`.
> В программе используются несколько лучей, значит луч будет выглядеть `tracingLine(testPoint, "Другая точка")`.
4. Чтобы найти точку пересечения двух прямых, необходимо составить систему уравнений вида:

## Архитектура приложения
Паттерн MVVM подразумевает, что программа состоит из 3 частей:
* View - пользовательскией интерфейс
* View-Model - функционал программы
* Model - бизнес логика

В проекте реализованы View и View-Model.
Если мы захотим добавить Model, то разделение компонентов позволит сделать это максимально эффективно.
## Ссылки
Проект реализован по [заданию](https://disk.yandex.ru/d/cTMQ5OgOPhadAw) от работодателя.
