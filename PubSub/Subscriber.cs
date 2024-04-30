namespace Sandbox
{
    public class EventSubscriber
    {
        public void HandleEvent(object sender, EventDataArgs eventArgs)
        {
            Console.WriteLine($"Received event: {eventArgs.Message}");
        }
    }
}
