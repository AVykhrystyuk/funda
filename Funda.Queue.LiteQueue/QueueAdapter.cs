using Funda.Queue.Abstractions;
using LiteQueue;

namespace Funda.Queue.LiteQueue;

public class QueueAdapter<T> : IQueue<T>
{
    private readonly LiteQueue<T> _queue;
    private readonly TimeSpan _dequeueInterval;

    public QueueAdapter(LiteQueue<T> queue, TimeSpan? dequeueInterval = null)
    {
        _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        _dequeueInterval = dequeueInterval ?? TimeSpan.FromSeconds(1);
    }

    public Task Enqueue(T message)
    {
        _queue.Enqueue(message);
        return Task.CompletedTask;
    }

    public async Task Dequeue(Func<T, bool> handleMessage)
    {
        while (true)
        {
            // Get next item from queue. Marks it as checked out such that other threads that 
            // call Checkout will not see it - but does not remove it from the queue.
            var record = _queue.Dequeue();
            if (record is null)
            {
                await Task.Delay(_dequeueInterval);
                continue;
            }
            try
            {
                var handled = handleMessage(record.Payload);
                if (!handled)
                    continue;

                // Removes record from queue
                _queue.Commit(record);
            }
            catch
            {
                // Returns the record to the queue
                _queue.Abort(record);
            }
        }
    }
}
