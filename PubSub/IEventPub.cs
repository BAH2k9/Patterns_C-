namespace Sandbox
{
    public interface IEventPublisher
    {
        event EventHandler<EventDataArgs> EventPublished;
        void PublishEvent(EventDataArgs eventArgs);
    }


    public class EventDataArgs : EventArgs
    {
        public string Message { get; set; }
    }
}
