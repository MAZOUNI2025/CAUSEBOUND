namespace Causebound.Monetization
{
    public interface IMonetizationService
    {
        bool IsAvailable { get; }
        void ShowRewarded(string placementId);
    }

    public sealed class NullMonetizationService : IMonetizationService
    {
        public bool IsAvailable => false;
        public void ShowRewarded(string placementId) { }
    }
}
