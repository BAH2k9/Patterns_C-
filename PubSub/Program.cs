using Sandbox;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create Publishers
            var Publisher = new EventPublisher();

            // Create subscribers
            var subscriber1 = new EventSubscriber();
            var subscriber2 = new EventSubscriber();

            // Subscribe to publisher's event
            Publisher.EventPublished += subscriber1.HandleEvent;
            Publisher.EventPublished += subscriber2.HandleEvent;

            // Publish an event
            Publisher.PublishEvent(new EventDataArgs { Message = "Hello, World!" });

            // Unsubscribe
            Publisher.EventPublished -= subscriber1.HandleEvent;

            // Publish an event
            Publisher.PublishEvent(new EventDataArgs { Message = "Goodbye!" });
        }
    }
}
