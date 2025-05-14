# Как запускать проект

1. Запустить `build\net9.0\publish\UserEntityWebAPI.exe`. Откроется командная строка, в которой будет что-то типа такого:

   > info: Microsoft.Hosting.Lifetime[14]<br>
   > Now listening on: http://localhost:5006<br>
   > info: Microsoft.Hosting.Lifetime[14]<br>
   > Now listening on: https://localhost:7093<br>
   > info: Microsoft.Hosting.Lifetime[0]<br>
   > Application started. Press Ctrl+C to shut down.<br>
   > info: Microsoft.Hosting.Lifetime[0]<br>
   > Hosting environment: Production<br>
   > info: Microsoft.Hosting.Lifetime[0]<br>
   > Content root path: ...\UserEntityWebAPI\build\net9.0\publish

2. Открыть ссылку https://localhost:7093 и в конце дописать https://localhost:7093/swagger/index.html. Должен открыться интерфейс swagger.

3. По умолчанию создан пользователь `admin` с паролем `admin`. Необходимо выполнить POST запрос `api/Auth/login` и ввести соответсвующие логин (admin) и пароль (admin).

4. После успешного ввода, ниже можно увидеть статус 200 и токен. Токен необходимо скопировать **без кавычек**. Этот токен - токен конкретного пользователя и он будет активен 30 минут.

5. Вверху страницы swagger есть кнопка `Autorize`. Нажмите на неё и в поле `Value` необходимо ввести следующую строку: `Bearer eyJhbGBYUEls...`. И нажать на кнопку `Authorize`. На этом этапе мы должны были авторизоваться от имени админа. Чтобы проверить, что мы успешно авторизовались, попробуйте выполнить GET запрос `api/User/acive`. Если всё успешно, то должен быть статус 200 и мы должны увидеть нашего единственного админа в массиве.

6. Всё, теперь можно создавать других пользователей и так же от их логина и пароля работать. Сначала надо выполнить POST запрос `/api/Auth/login` с логином и паролем нового пользователя, получить свой токен, потом нажать на `Authoruze`, нажать `Logout` из текущего пользователя и ввести новый токен.
