namespace Causebound.Levels
{
    public enum LevelRuntimeState
    {
        Unloaded,
        Loading,
        Ready,
        Active,
        Completed,
        Resetting
    }

    public sealed class LevelSession
    {
        public LevelData Data { get; }
        public LevelRuntimeState State { get; private set; } = LevelRuntimeState.Unloaded;

        public LevelSession(LevelData data)
        {
            Data = data;
        }

        public void SetState(LevelRuntimeState nextState)
        {
            State = nextState;
        }
    }
}
