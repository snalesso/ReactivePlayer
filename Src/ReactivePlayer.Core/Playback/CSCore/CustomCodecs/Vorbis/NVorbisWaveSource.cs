using CSCore;
using NVorbis;
using System;
using System.IO;

namespace ReactivePlayer.Playback.CSCore.CustomCodecs.Vorbis
{
    public sealed class NVorbisSource : ISampleSource
    {
        private readonly Stream _stream;
        private readonly VorbisReader _vorbisReader;

        private readonly WaveFormat _waveFormat;
        private bool _disposed;

        public NVorbisSource(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanRead)
                throw new ArgumentException("Stream is not readable.", "stream");
            this._stream = stream;
            this._vorbisReader = new VorbisReader(stream, false);
            this._waveFormat = new WaveFormat(this._vorbisReader.SampleRate, 32, this._vorbisReader.Channels, AudioEncoding.IeeeFloat);
        }

        public bool CanSeek
        {
            get { return this._stream.CanSeek; }
        }

        public WaveFormat WaveFormat
        {
            get { return this._waveFormat; }
        }

        //got fixed through workitem #17, thanks for reporting @rgodart.
        public long Length
        {
            get { return this.CanSeek ? (long)(this._vorbisReader.TotalTime.TotalSeconds * this._waveFormat.SampleRate * this._waveFormat.Channels) : 0; }
        }

        //got fixed through workitem #17, thanks for reporting @rgodart.
        public long Position
        {
            get
            {
                return this.CanSeek ? (long)(this._vorbisReader.DecodedTime.TotalSeconds * this._vorbisReader.SampleRate * this._vorbisReader.Channels) : 0;
            }
            set
            {
                if (!this.CanSeek)
                    throw new InvalidOperationException("NVorbisSource is not seekable.");
                if (value < 0 || value > this.Length)
                    throw new ArgumentOutOfRangeException("value");

                this._vorbisReader.DecodedTime = TimeSpan.FromSeconds((double)value / this._vorbisReader.SampleRate / this._vorbisReader.Channels);
            }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            return this._vorbisReader.ReadSamples(buffer, offset, count);
        }

        public void Dispose()
        {
            if (!this._disposed)
                this._vorbisReader.Dispose();
            else
                throw new ObjectDisposedException("NVorbisSource");
            this._disposed = true;
        }
    }
}