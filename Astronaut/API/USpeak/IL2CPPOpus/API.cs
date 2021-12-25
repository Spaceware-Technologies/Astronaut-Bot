using FragLabs.Audio.Codecs.Opus;
using System;
using System.Runtime.InteropServices;
using System.Text;

//Original USpeak classes by DayOfThePlay, fixed by Rin
//This class is redundant. I'm lazy.
namespace USpeak.IL2CPPOpus
{
    class API
    {
        internal static IntPtr opus_encoder_create(int Fs, int channels, int application, out IntPtr error)
        {
            return FragLabs.Audio.Codecs.Opus.API.opus_encoder_create(Fs, channels, application, out error);
        }

        internal static void opus_encoder_destroy(IntPtr encoder)
        {
            FragLabs.Audio.Codecs.Opus.API.opus_encoder_destroy(encoder);
        }

        internal static int opus_encode(IntPtr st, short[] pcm, int frame_size, IntPtr data, int max_data_bytes)
        {
            return FragLabs.Audio.Codecs.Opus.API.opus_encode(st, pcm, frame_size, data, max_data_bytes);
        }

        internal static int opus_encode_float(IntPtr st, float[] pcm, int frame_size, IntPtr data, int max_data_bytes)
        {
            return FragLabs.Audio.Codecs.Opus.API.opus_encode_float(st, pcm, frame_size, data, max_data_bytes);
        }
        internal static IntPtr opus_decoder_create(int Fs, int channels, out IntPtr error)
        {
            return FragLabs.Audio.Codecs.Opus.API.opus_decoder_create(Fs, channels, out error);
        }

        internal static void opus_decoder_destroy(IntPtr encoder)
        {
            FragLabs.Audio.Codecs.Opus.API.opus_decoder_destroy(encoder);
        }
        internal static int opus_decode(IntPtr st, byte[] data, int len, IntPtr pcm, int frame_size, int decode_fec)
        {
            return FragLabs.Audio.Codecs.Opus.API.opus_decode(st, data, len, pcm, frame_size, decode_fec);
        }

        internal static int opus_decode_float(IntPtr st, byte[] data, int len, IntPtr pcm, int frame_size, int decode_fec)
        {
            return FragLabs.Audio.Codecs.Opus.API.opus_decode(st, data, len, pcm, frame_size, decode_fec);
        }

        internal static int opus_encoder_ctl(IntPtr st, Ctl request, int value)
        {
            return FragLabs.Audio.Codecs.Opus.API.opus_encoder_ctl(st, request, value);
        }
        internal static int opus_encoder_ctl(IntPtr st, Ctl request, out int value)
        {
            return FragLabs.Audio.Codecs.Opus.API.opus_encoder_ctl(st, request, out value);
        }

        internal static int opus_decoder_ctl(IntPtr st, Ctl request, int value)
        {
            return FragLabs.Audio.Codecs.Opus.API.opus_decoder_ctl(st, request, value);
        }
        internal static int opus_decoder_ctl(IntPtr st, Ctl request, out int value)
        {
            return FragLabs.Audio.Codecs.Opus.API.opus_decoder_ctl(st, request, out value);
        }
        internal static IntPtr opus_get_version_string()
        {
            return FragLabs.Audio.Codecs.Opus.API.opus_get_version_string();

        }
        internal static string opus_get_version_string_wrapper()
        {
            IntPtr ptr = API.opus_get_version_string();
            return API.PtrToStringUtf8(ptr);
        }

        private static string PtrToStringUtf8(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return string.Empty;
            }
            int num = 0;
            while (Marshal.ReadByte(ptr, num) != 0)
            {
                num++;
            }
            if (num == 0)
            {
                return string.Empty;
            }
            byte[] array = new byte[num];
            Marshal.Copy(ptr, array, 0, num);
            return Encoding.UTF8.GetString(array);
        }
    }
}
