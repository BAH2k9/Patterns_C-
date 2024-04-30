namespace Sandbox
{
    public class EventPublisher : IEventPublisher
    {
        // Define the event
        public event EventHandler<EventDataArgs> EventPublished;

        // Method to publish events
        public void PublishEvent(EventDataArgs eventArgs)
        {
            EventPublished?.Invoke(this, eventArgs);
        }
    }
}


