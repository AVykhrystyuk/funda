namespace Funda.Queue.Abstractions;

public interface IQueue<T>
{
    Task Enqueue(T message);

    Task Dequeue(Func<T, bool> handleMessage);
}
