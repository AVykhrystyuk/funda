namespace Funda.Queue.Abstractions;

public interface IQueue<T>
{
    Task Enqueue(T message);

    Task Dequeue(Func<T, bool> handleMessage);
}

public static class IQueueExtensions
{
    public static Task Dequeue<T>(this IQueue<T> self, Action<T> handleMessage) =>
        self.Dequeue(message =>
        {
            handleMessage(message);
            return true;
        });
}
