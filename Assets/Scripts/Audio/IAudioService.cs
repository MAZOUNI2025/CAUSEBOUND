namespace Causebound.Audio
{
    public interface IAudioService
    {
        void PlayMusic(string musicId);
        void StopMusic();
    }

    public sealed class NullAudioService : IAudioService
    {
        public void PlayMusic(string musicId) { }
        public void StopMusic() { }
    }
}
