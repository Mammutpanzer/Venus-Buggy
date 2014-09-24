using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace VenusBuggy
{
    class Wave
    {
        int buffer;
        int source;
        int state;
        int channels, bits_per_sample, sample_rate;
        byte[] sound_data = LoadWave(File.Open(filename, FileMode.Open), out channels, out bits_per_sample, out sample_rate);
            AL.BufferData(buffer, audio.GetSoundFormat(channels, bits_per_sample), sound_data, sound_data.Length, sample_rate);
            AL.Source(source, ALSourcei.Buffer, buffer);
            AL.SourcePlay(source);
        public Wave(string filename)
        {
            buffer = AL.GenBuffer();
            source = AL.GenSource();
            int state;
            int channels, bits_per_sample, sample_rate;
            byte[] sound_data = Audio.LoadWave(File.Open(filename, FileMode.Open), out channels, out bits_per_sample, out sample_rate);
            AL.BufferData(buffer, Audio.GetSoundFormat(channels, bits_per_sample), sound_data, sound_data.Length, sample_rate);
            AL.Source(source, ALSourcei.Buffer, buffer);
            AL.SourcePlay(source);
        }
    }
}
