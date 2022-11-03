using Funda.Queue.Abstractions;

namespace Funda.Queue.Worker.Console.Common;

internal class Worker<T>
{
    private readonly IQueue<T> _queue;
    public Worker(IQueue<T> queue) =>
        _queue = queue ?? throw new ArgumentNullException(nameof(queue));

    public Task Run(Action<T> handleMessage, TimeSpan interval) =>
        Run(message =>
        {
            handleMessage(message);
            return true;
        }, interval);

    public async Task Run(Func<T, bool> handleMessage, TimeSpan interval)
    {
        while (true)
        {
            await _queue.Dequeue(handleMessage);
            await Task.Delay(interval);
        }
    }
}
