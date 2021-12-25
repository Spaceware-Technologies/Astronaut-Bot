using static USpeak.Encode;
using static USpeak.IL2CPPOpus.OpusCodec;

//Original USpeak classes by DayOfThePlay, fixed by Rin
namespace USpeak
{
    public class AudioClipCompressor
    {
        public static byte[] CompressAudioData(float[] samples, BandMode mode, USpeakCodic Codec, float gain)
        {
            ApplyGain(samples, gain);
            byte[] array = Codec.EncodeFloat(samples, mode);
            if (array == null)
            {
                return null;
            }
            return array;
        }
        public static float[] DecompressAudio(byte[] data, BandMode mode, USpeakCodic Codec, float gain)
        {
            float[] array = Codec.DecodeFloat(data, mode);
            if (array == null)
            {
                return null;
            }
            ApplyGain(array, gain);
            return array;
        }
        public static void ApplyGain(float[] data, float gain)
        {
            if (gain == 1f)
            {
                return;
            }
            for (int i = 0; i < data.Length; i++)
            {
                float num = data[i] * gain;
                data[i] = ((num >= -1f) ? ((num <= 1f) ? num : 1f) : -1f);
            }
        }
    }
}
