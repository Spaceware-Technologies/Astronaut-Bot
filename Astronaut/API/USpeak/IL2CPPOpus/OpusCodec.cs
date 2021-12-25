using FragLabs.Audio.Codecs.Opus;
using System;
using static USpeak.IL2CPPOpus.API;

//Original USpeak classes by DayOfThePlay, fixed by Rin
namespace USpeak.IL2CPPOpus
{
    public class OpusCodec
    {
        public OpusCodec()
        {
            this.InitSettings(48000, 48000, 20, Application.Voip);
            this._encoder = OpusEncoder.Create(this._frequency, 1, this._app);
            this._encoder.Bitrate = this._bitrate;
            this._encoder.ForwardErrorCorrection = false;
            this._encoder.ExpectedPacketLossPct = 0;

            this._bytesPerSegment = this._encoder.FrameByteCount(this._segmentFrames);
            this._decoder = OpusDecoder.Create(this._frequency, 1, this._bytesPerSegment);
            this._decoder.ForwardErrorCorrection = false;
        }
        public enum BandMode
        {
            Narrow,
            Wide,
            UltraWide,
            Opus48k
        }
        public int GetSampleSize(int recordingFrequency)
        {
            return this._segmentFrames;
        }
        public byte[] Encode(short[] data, BandMode mode)
        {
            if (mode != BandMode.Opus48k)
            {
                Console.WriteLine(string.Concat(new string[]
                {
                    "OpusCodec: Encode: bandwidth mode must be ",
                    BandMode.Opus48k.ToString(),
                    "! (set to ",
                    mode.ToString(),
                    ")"
                }));
                return null;
            }
            if (data.Length != this._segmentFrames)
            {
                Console.WriteLine(string.Concat(new object[]
                {
                    "OpusCodec: Encode failed! Input PCM data is ",
                    data.Length,
                    " frames, expected ",
                    this._segmentFrames
                }));
                return null;
            }
            int num = 0;
            return this._encoder.Encode(data, this._encoder.MaxDataBytes, out num);
        }
        public byte[] EncodeFloat(float[] data, BandMode mode)
        {
            if (mode != BandMode.Opus48k)
            {
                Console.WriteLine(string.Concat(new string[]
                {
                    "OpusCodec: Encode: bandwidth mode must be ",
                    BandMode.Opus48k.ToString(),
                    "! (set to ",
                    mode.ToString(),
                    ")"
                }));
                return null;
            }
            if (data.Length != this._segmentFrames)
            {
                Console.WriteLine(string.Concat(new object[]
                {
                    "OpusCodec: Encode failed! Input PCM data is ",
                    data.Length,
                    " frames, expected ",
                    this._segmentFrames
                }));
                return null;
            }
            return this._encoder.EncodeFloat(data, out _);
        }
        public short[] Decode(byte[] data, BandMode mode)
        {
            if (mode != BandMode.Opus48k)
            {
                Console.WriteLine(string.Concat(new string[]
                {
                    "OpusCodec: Decode: bandwidth mode must be ",
                    BandMode.Opus48k.ToString(),
                    "! (set to ",
                    mode.ToString(),
                    ")"
                }));
            }
            int num = 0;
            return this._decoder.Decode(data, out num);
        }
        public float[] DecodeFloat(byte[] data, BandMode mode)
        {
            if (mode != BandMode.Opus48k)
            {
                Console.WriteLine(string.Concat(new string[]
                {
                    "OpusCodec: Decode: bandwidth mode must be ",
                    BandMode.Opus48k.ToString(),
                    "! (set to ",
                    mode.ToString(),
                    ")"
                }));
                return new float[0];
            }
            int num = 0;
            return this._decoder.DecodeFloat(data, out num);
        }
        private void InitSettings(int frequency, int bitrate, int delay, Application app)
        {
            if (bitrate > 48000)
            {
                app = Application.Audio;
            }
            this._frequency = frequency;
            this._bitrate = bitrate;
            this._segmentFrames = delay * (this._frequency / 1000);
            this._app = app;
        }

        private const int OPUS_SAMPLE_FREQUENCY = 48000;

        public OpusEncoder _encoder;
        public OpusDecoder _decoder;

        private int _segmentFrames;

        private int _bytesPerSegment;

        private int _frequency;

        private int _bitrate;

        private Application _app;
    }

    public class OpusEncoder : IDisposable
    {
        private OpusEncoder(IntPtr encoder, int inputSamplingRate, int inputChannels, Application application)
        {
            this._encoder = encoder;
            this.InputSamplingRate = inputSamplingRate;
            this.InputChannels = inputChannels;
            this.Application = application;
            this.MaxDataBytes = 4000;
            this._encodeBuffer = new byte[this.MaxDataBytes];
        }
        public static OpusEncoder Create(int inputSamplingRate, int inputChannels, Application application)
        {
            if (inputSamplingRate != 8000 && inputSamplingRate != 12000 && inputSamplingRate != 16000 && inputSamplingRate != 24000 && inputSamplingRate != 48000)
            {
                throw new ArgumentOutOfRangeException("inputSamplingRate");
            }
            if (inputChannels != 1 && inputChannels != 2)
            {
                throw new ArgumentOutOfRangeException("inputChannels");
            }
            IntPtr value;
            IntPtr encoder = API.opus_encoder_create(inputSamplingRate, inputChannels, (int)application, out value);
            if ((int)value != 0)
            {
                throw new Exception("Exception occured while creating encoder");
            }
            return new OpusEncoder(encoder, inputSamplingRate, inputChannels, application);
        }

        public unsafe byte[] Encode(short[] inputPcmSamples, int sampleLength, out int encodedLength)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("OpusEncoder");
            }
            int frame_size = this.FrameCount(inputPcmSamples);
            byte[] @byte = USpeakPoolUtils.GetByte(this.MaxDataBytes);
            int num;
            fixed (byte* value = &(@byte != null && @byte.Length != 0 ? ref @byte[0] : ref *(byte*)null))
            {
                IntPtr data = new IntPtr((void*)value);
                num = API.opus_encode(this._encoder, inputPcmSamples, frame_size, data, sampleLength);
            }
            encodedLength = num;
            if (num < 0)
            {
                USpeakPoolUtils.Return(@byte);
                string str = "Encoding failed - ";
                Errors errors = (Errors)num;
                throw new Exception(str + errors.ToString());
            }
            byte[] byte2 = USpeakPoolUtils.GetByte(encodedLength);
            Buffer.BlockCopy(@byte, 0, byte2, 0, encodedLength);
            USpeakPoolUtils.Return(@byte);
            return byte2;
        }
        public unsafe byte[] EncodeFloat(float[] inputSamples, out int encodedLength)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("OpusEncoder");
            }
            int frame_size = inputSamples.Length / InputChannels;
            int num = 0;
            fixed (byte* value = &(_encodeBuffer != null && _encodeBuffer.Length != 0 ? ref _encodeBuffer[0] : ref *(byte*)null))
            {
                num = API.opus_encode_float(data: new IntPtr(value), st: _encoder, pcm: inputSamples, frame_size: frame_size, max_data_bytes: _encodeBuffer.Length);
            }
            encodedLength = num;
            if (num < 0)
            {
                Errors errors = (Errors)num;
                Console.WriteLine("Encoding failed - " + errors.ToString());
                return null;
            }
            byte[] @byte = new byte[encodedLength];
            Buffer.BlockCopy(_encodeBuffer, 0, @byte, 0, encodedLength);
            return @byte;
        }
        public int FrameCount(short[] pcmSamples)
        {
            int num = 16;
            int num2 = num / 8 * this.InputChannels;
            return pcmSamples.Length / num2;
        }

        public int FrameByteCount(int frameCount)
        {
            int num = 16;
            int num2 = num / 16 * this.InputChannels;
            return frameCount * num2;
        }
        public void ResetState()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("OpusEncoder");
            }
            API.opus_encoder_ctl(this._encoder, Ctl.OpusResetState, 0);
        }

        public int InputSamplingRate { get; private set; }
        public int InputChannels { get; private set; }
        public Application Application { get; private set; }
        public int MaxDataBytes { get; set; }

        public int Bitrate
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("OpusEncoder");
                }
                int result;
                int num = API.opus_encoder_ctl(this._encoder, Ctl.GetBitrateRequest, out result);
                if (num < 0)
                {
                    string str = "Encoder error - ";
                    Errors errors = (Errors)num;
                    throw new Exception(str + errors.ToString());
                }
                return result;
            }
            set
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("OpusEncoder");
                }
                int num = API.opus_encoder_ctl(this._encoder, Ctl.SetBitrateRequest, value);
                if (num < 0)
                {
                    string str = "Encoder error - ";
                    Errors errors = (Errors)num;
                    throw new Exception(str + errors.ToString());
                }
            }
        }

        public int Complexity
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("OpusEncoder");
                }
                int result;
                int num = API.opus_encoder_ctl(this._encoder, Ctl.GetComplexityRequest, out result);
                if (num < 0)
                {
                    string str = "Encoder error - ";
                    Errors errors = (Errors)num;
                    throw new Exception(str + errors.ToString());
                }
                return result;
            }
            set
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("OpusEncoder");
                }
                int num = API.opus_encoder_ctl(this._encoder, Ctl.SetComplexityRequest, value);
                if (num < 0)
                {
                    string str = "Encoder error - ";
                    Errors errors = (Errors)num;
                    throw new Exception(str + errors.ToString());
                }
            }
        }

        public bool ForwardErrorCorrection
        {
            get
            {
                if (this._encoder == IntPtr.Zero)
                {
                    throw new ObjectDisposedException("OpusEncoder");
                }
                int num2;
                int num = API.opus_encoder_ctl(this._encoder, Ctl.GetInbandFECRequest, out num2);
                if (num < 0)
                {
                    string str = "Encoder error - ";
                    Errors errors = (Errors)num;
                    throw new Exception(str + errors.ToString());
                }
                return num2 > 0;
            }
            set
            {
                if (this._encoder == IntPtr.Zero)
                {
                    throw new ObjectDisposedException("OpusEncoder");
                }
                int num = API.opus_encoder_ctl(this._encoder, Ctl.SetInbandFECRequest, (!value) ? 0 : 1);
                if (num < 0)
                {
                    string str = "Encoder error - ";
                    Errors errors = (Errors)num;
                    throw new Exception(str + errors.ToString());
                }
            }
        }

        public int ExpectedPacketLossPct
        {
            get
            {
                if (this._encoder == IntPtr.Zero)
                {
                    throw new ObjectDisposedException("OpusEncoder");
                }
                int result;
                int num = API.opus_encoder_ctl(this._encoder, Ctl.GetPacketLossPercRequest, out result);
                if (num < 0)
                {
                    string str = "Encoder error - ";
                    Errors errors = (Errors)num;
                    throw new Exception(str + errors.ToString());
                }
                return result;
            }
            set
            {
                if (this._encoder == IntPtr.Zero)
                {
                    throw new ObjectDisposedException("OpusEncoder");
                }
                int num = API.opus_encoder_ctl(this._encoder, Ctl.SetPacketLossPercRequest, value);
                if (num < 0)
                {
                    string str = "Encoder error - ";
                    Errors errors = (Errors)num;
                    throw new Exception(str + errors.ToString());
                }
            }
        }

        ~OpusEncoder()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }
            GC.SuppressFinalize(this);
            if (this._encoder != IntPtr.Zero)
            {
                API.opus_encoder_destroy(this._encoder);
                this._encoder = IntPtr.Zero;
            }
            this.disposed = true;
        }
        private IntPtr _encoder;
        private byte[] _encodeBuffer;
        private bool disposed;
    }

    public class OpusDecoder : IDisposable
    {
        private OpusDecoder(IntPtr decoder, int outputSamplingRate, int outputChannels, int expectedFramesPerSegment)
        {
            this._decoder = decoder;
            this.OutputSamplingRate = outputSamplingRate;
            this.OutputChannels = outputChannels;
            this._expectedFramesPerSegment = expectedFramesPerSegment;
        }

        public static OpusDecoder Create(int outputSampleRate, int outputChannels, int expectedFramesPerSegment)
        {
            if (outputSampleRate != 8000 && outputSampleRate != 12000 && outputSampleRate != 16000 && outputSampleRate != 24000 && outputSampleRate != 48000)
            {
                throw new ArgumentOutOfRangeException("outputSampleRate");
            }
            if (outputChannels != 1 && outputChannels != 2)
            {
                throw new ArgumentOutOfRangeException("outputChannels");
            }
            IntPtr value;
            IntPtr decoder = API.opus_decoder_create(outputSampleRate, outputChannels, out value);
            if ((int)value != 0)
            {
                throw new Exception("Exception occured while creating decoder");
            }
            return new OpusDecoder(decoder, outputSampleRate, outputChannels, expectedFramesPerSegment);
        }

        public unsafe short[] Decode(byte[] inputOpusData, out int decodedLength)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("OpusDecoder");
            }
            short[] @short = USpeakPoolUtils.GetShort(this._expectedFramesPerSegment * this.OutputChannels);
            int num;
            fixed (short* value = &(@short != null && @short.Length != 0 ? ref @short[0] : ref *(short*)null))
            {
                IntPtr pcm = new IntPtr((void*)value);
                if (inputOpusData != null)
                {
                    num = API.opus_decode(this._decoder, inputOpusData, inputOpusData.Length, pcm, this._expectedFramesPerSegment, 0);
                }
                else
                {
                    num = API.opus_decode(this._decoder, null, 0, pcm, this._expectedFramesPerSegment, (!this.ForwardErrorCorrection) ? 0 : 1);
                }
            }
            decodedLength = num;
            if (num < 0)
            {
                USpeakPoolUtils.Return(@short);
                string str = "Decoding failed - ";
                Errors errors = (Errors)num;
                Console.WriteLine(str + errors.ToString());
                return null;
            }
            if (num != this._expectedFramesPerSegment)
            {
                USpeakPoolUtils.Return(@short);
                Console.WriteLine(string.Concat(new object[]
                {
                    "Decoding failed - got unexpected number of frames ",
                    num,
                    ", expected ",
                    this._expectedFramesPerSegment
                }));
                return null;
            }
            return @short;
        }

        public unsafe float[] DecodeFloat(byte[] inputOpusData, out int decodedLength)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("OpusDecoder");
            }
            float[] @float = new float[this._expectedFramesPerSegment * this.OutputChannels];
            int num;
            fixed (float* value = &(@float != null && @float.Length != 0 ? ref @float[0] : ref *(float*)null))
            {
                IntPtr pcm = new IntPtr((void*)value);
                if (inputOpusData != null)
                {
                    num = API.opus_decode_float(this._decoder, inputOpusData, inputOpusData.Length, pcm, this._expectedFramesPerSegment, 0);
                }
                else
                {
                    num = API.opus_decode_float(this._decoder, null, 0, pcm, this._expectedFramesPerSegment, (!this.ForwardErrorCorrection) ? 0 : 1);
                }
            }
            decodedLength = num;
            if (num < 0)
            {
                USpeakPoolUtils.Return(@float);
                string str = "Decoding failed - ";
                Errors errors = (Errors)num;
                Console.WriteLine(str + errors.ToString());
                return null;
            }
            if (num != this._expectedFramesPerSegment)
            {
                USpeakPoolUtils.Return(@float);
                Console.WriteLine(string.Concat(new object[]
                {
                    "Decoding failed - got unexpected number of frames ",
                    num,
                    ", expected ",
                    this._expectedFramesPerSegment
                }));
                return null;
            }
            return @float;
        }

        public void ResetState()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("OpusDecoder");
            }
            API.opus_decoder_ctl(this._decoder, Ctl.OpusResetState, 0);
        }

        public int OutputSamplingRate { get; private set; }

        public int OutputChannels { get; private set; }
        public bool ForwardErrorCorrection { get; set; }


        public int Gain
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("OpusDecoder");
                }
                int result;
                int num = API.opus_decoder_ctl(this._decoder, Ctl.GetGainRequest, out result);
                if (num < 0)
                {
                    string str = "Decoder error - ";
                    Errors errors = (Errors)num;
                    throw new Exception(str + errors.ToString());
                }
                return result;
            }
            set
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("OpusDecoder");
                }
                int num = API.opus_decoder_ctl(this._decoder, Ctl.SetGainRequest, value);
                if (num < 0)
                {
                    string str = "Decoder error - ";
                    Errors errors = (Errors)num;
                    throw new Exception(str + errors.ToString());
                }
            }
        }

        public int Pitch
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("OpusDecoder");
                }
                int result;
                int num = API.opus_decoder_ctl(this._decoder, Ctl.GetPitchRequest, out result);
                if (num < 0)
                {
                    string str = "Decoder error - ";
                    Errors errors = (Errors)num;
                    throw new Exception(str + errors.ToString());
                }
                return result;
            }
        }

        ~OpusDecoder()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }
            GC.SuppressFinalize(this);
            if (this._decoder != IntPtr.Zero)
            {
                API.opus_decoder_destroy(this._decoder);
                this._decoder = IntPtr.Zero;
            }
            this.disposed = true;
        }

        private IntPtr _decoder;

        private int _expectedFramesPerSegment;

        private bool disposed;
    }
}
