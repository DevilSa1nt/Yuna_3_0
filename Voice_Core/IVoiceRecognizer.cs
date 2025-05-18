namespace Voice_Core
{
    public interface IVoiceRecognizer
    {
        Task<string> TranscribeAsync(byte[] audioData, string fileName);
    }
}