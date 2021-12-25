using static USpeak.IL2CPPOpus.OpusCodec;

//Original USpeak classes by DayOfThePlay, fixed by Rin
namespace USpeak
{
    public static class Encode
    {
        public class USpeakCodic
        {
            public virtual byte[] Encode(short[] data, BandMode mode) { return null; }

            public virtual byte[] EncodeFloat(float[] data, BandMode mode) { return null; }

            public virtual short[] Decode(byte[] data, BandMode mode) { return null; }
            public virtual float[] DecodeFloat(byte[] data, BandMode mode) { return null; }
            public virtual int GetSampleSize(int recordingFrequency) { return 0; }
        }
        public class OpusEnCodic : USpeakCodic
        {
            private IL2CPPOpus.OpusCodec _Codec;
            public OpusEnCodic()
            {
                _Codec = new IL2CPPOpus.OpusCodec();
                InitSettings(48000, 24000, 20);
            }
            public override byte[] Encode(short[] data, BandMode mode)
            {
                return _Codec.Encode(data, (IL2CPPOpus.OpusCodec.BandMode)mode);
            }
            public override byte[] EncodeFloat(float[] data, BandMode mode)
            {
                return _Codec.EncodeFloat(data, (IL2CPPOpus.OpusCodec.BandMode)mode);
            }
            public override short[] Decode(byte[] data, BandMode mode)
            {
                return _Codec.Decode(data, (IL2CPPOpus.OpusCodec.BandMode)mode);
            }
            public override float[] DecodeFloat(byte[] data, BandMode mode)
            {
                return _Codec.DecodeFloat(data, (IL2CPPOpus.OpusCodec.BandMode)mode);
            }
            public override int GetSampleSize(int recordingFrequency)
            {
                return this._segmentFrames;
            }
            private void InitSettings(int frequency, int bitrate, int delay)
            {
                if (bitrate > 48000)
                {
                }
                this._frequency = frequency;
                this._bitrate = bitrate;
                this._segmentFrames = delay * (this._frequency / 1000);
            }
            private const int OPUS_SAMPLE_FREQUENCY = 48000;
            private int _segmentFrames;
            private int _bytesPerSegment;
            private int _frequency;
            private int _bitrate;
            private bool isInitialized;
        }
    }
    public enum Opus_Delay
    {
        Delay_10ms = 10,
        Delay_20ms = 20,
        Delay_40ms = 40,
        Delay_60ms = 60
    }
    public enum BitRates
    {
        BitRate_8K = 8000,
        BitRate_10K = 10000,
        BitRate_16K = 16000,
        BitRate_18K = 18000,
        BitRate_20K = 20000,
        BitRate_24K = 24000,
        BitRate_32K = 32000,
        BitRate_48K = 48000,
        BitRate_64k = 64000,
        BitRate_96k = 96000,
        BitRate_128k = 128000,
        BitRate_256k = 256000,
        BitRate_384k = 384000,
        BitRate_512k = 512000
    }
}
