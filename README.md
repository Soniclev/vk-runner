# Программа для отладки VkNet
Реализует консольную авторизацию, хранения токена

# Использование

```csharp
static void Main(string[] args)
{
    var api = Authorizer.Authorize(logger: InitLogger());

    var response = api.NewsFeed.Get(new NewsFeedGetParams()
    {
        Count = 10
    });

    Console.ReadKey();
}
```
