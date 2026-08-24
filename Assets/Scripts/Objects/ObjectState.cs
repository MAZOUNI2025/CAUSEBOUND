namespace Causebound.Objects
{
    public enum ObjectState
    {
        Default,
        Active,
        Disabled,
        Completed
    }

    public interface IResettable
    {
        void ResetState();
    }
}
