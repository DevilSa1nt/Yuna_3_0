using System.IO;\
using NAudio.Wave;\
using NAudio.Vorbis;\
\
namespace Voice_Core\
{\
    public static class AudioConverter\
    {\
        public static byte[] ConvertOggToWav(byte[] oggData)\
        {\
