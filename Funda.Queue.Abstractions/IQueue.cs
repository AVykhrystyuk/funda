namespace Funda.Queue.Abstractions;

public interface IQueue<T>
{
    void Enqueue(T message);

    void Dequeue(Func<T, bool> handleMessage);

    void Dequeue(Action<T> handleMessage) =>
        Dequeue(message =>
        {
            handleMessage(message);
            return true;
        });
}

