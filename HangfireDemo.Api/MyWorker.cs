namespace HangfireDemo.Api;

public static class MyWorker
{
    public static void DoWork()
    {
        Console.WriteLine("Doing work! " + DateTime.Now);
    }
}