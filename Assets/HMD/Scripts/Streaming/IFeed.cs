namespace HMD.Scripts.Streaming
{
    using JetBrains.Annotations;
    public interface IFeed
    {
        [CanBeNull]
        public TextureView TryGetTexture([CanBeNull] TextureView existing);

        public void Stop();

        public void Play();

        public void Pause();
    }
}
