namespace Causebound.Analytics
{
    public interface IAnalyticsService
    {
        void Track(string eventName);
    }

    public sealed class NullAnalyticsService : IAnalyticsService
    {
        public void Track(string eventName) { }
    }
}
