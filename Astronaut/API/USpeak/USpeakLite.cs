
using ExitGames.Client.Photon;
using NAudio.Wave;
using Photon.Realtime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static USpeak.Encode;
using static USpeak.IL2CPPOpus.OpusCodec;
using USpeak;

//Original USpeak classes by DayOfThePlay, fixed by Rin
namespace Astronaut
{
    public class USpeakLite
    {
        public USpeakLite()
        {
            Codec = new OpusEnCodic();
        }
        private USpeakCodic Codec;

        /// <summary>
        /// Raises Event 1 to play a frame from the Audio Queue
        /// </summary>
        /// <param name="client">The Bot which is supposed to play it</param>
        public void SendAudio(PhotonClient client)
        {
            if (queue.Count == 0)
                return;
            byte[] buffer = new byte[1022];
            int offset = 8;
            while (offset <= 1022)
            {
                USpeakFrameContainer obj;
                while (!queue.TryPeek(out obj)) ;
                var length = obj.GetByteLength();
                if (length <= 1022)
                {
                    if ((offset + length) > 1022)
                    {
                        break;
                    }
                    else
                    {
                        Buffer.BlockCopy(obj.ToByteArray(), 0, buffer, offset, length);
                        while (!queue.TryDequeue(out _)) ;
                        offset += length;
                    }
                }
                else
                {
                    while (!queue.TryDequeue(out _)) ;
                }
            }
            Buffer.BlockCopy(BitConverter.GetBytes(0), 0, buffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(client.LoadBalancingPeer.ServerTimeInMilliSeconds), 0, buffer, 4, 4);
            Array.Resize(ref buffer, offset);
            //Console.WriteLine(offset); <---- displays offset numbers when playing a song
            client.OpRaiseEvent(1, buffer, new RaiseEventOptions()
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.Others
            }, SendOptions.SendUnreliable);
        }
        private USpeakFrameContainer ProcessEncode(float[] pcmData, ushort inputFrameIndex)
        {
            byte[] data = AudioClipCompressor.CompressAudioData(pcmData, lastBandMode, Codec, 1f); //1f is your Gain. You'd want this to be a variable you can change. 0-1f is vrchat standard, earrape is anything beyond, limit is the float limit ;)
            USpeakFrameContainer item = new USpeakFrameContainer() { FrameIndex = inputFrameIndex, encodedData = data };
            item.FrameIndex = inputFrameIndex;
            item.encodedData = data;
            return item;
        }

        private ushort ind;

        /// <summary>
        /// Loads an arbitrary Audio File and queues it up.
        /// </summary>
        /// <param name="audioFile">Path to the file to be streamed</param>
        /// <returns></returns>
        public Task StreamMP3(string audioFile)
        {
            float max = 0;
            Console.WriteLine("READING");
            WaveStream reader;
            if (audioFile.EndsWith(".ogg"))
                reader = new NAudio.Vorbis.VorbisWaveReader(audioFile);
            else
                reader = new AudioFileReader(audioFile);

            var outFormat = new WaveFormat(24000, reader.WaveFormat.Channels);

            //var resampler = new MediaFoundationResampler(reader, outFormat);
            //var data = WavUtility.FromAudioClip(WavUtility.ToAudioClip(mp3Data);
            using (var resampler = new MediaFoundationResampler(reader, outFormat))
            {
                var sampleProvider = resampler.ToSampleProvider();
                int read;
                do
                {
                    float[] buffer = new float[Codec.GetSampleSize(0)];
                    read = sampleProvider.Read(buffer, 0, buffer.Length);
                    for (int n = 0; n < read; n++)
                    {
                        var abs = Math.Abs(buffer[n]);
                        if (abs > max) max = abs;
                    }
                    if (ind == ushort.MaxValue)
                        ind = 0;
                    queue.Enqueue(ProcessEncode(buffer, ind++));
                    Thread.Sleep(10);
                } while (read > 0);
            }
            reader.Dispose();
            return null;
        }

        public ConcurrentQueue<USpeakFrameContainer> queue = new ConcurrentQueue<USpeakFrameContainer>();
        private List<USpeakFrameContainer> sendBuffer = new List<USpeakFrameContainer>();

        private BandMode lastBandMode = BandMode.Opus48k;
        public BandMode BandwidthMode = BandMode.Opus48k;
    }
}
