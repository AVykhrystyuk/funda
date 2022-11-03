using Funda.Queue.Abstractions;

namespace Funda.Queue.Worker.Console.Common;

internal class Worker<T>
{
    private readonly IQueue<T> _queue;
    public Worker(IQueue<T> queue) =>
        _queue = queue ?? throw new ArgumentNullException(nameof(queue));

    public void Run(Action<T> handleMessage, TimeSpan interval) => 
        Run(message => handleMessage(message), interval);

    public void Run(Func<T, bool> handleMessage, TimeSpan interval)
    {
        while (true)
        {
            _queue.Dequeue(handleMessage);
            Thread.Sleep(interval);
        }
    }
}
