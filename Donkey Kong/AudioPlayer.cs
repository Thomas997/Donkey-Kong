using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donkey_Kong
{
    public class AudioPlayer
    {
        private WaveOut outputDevice;
        private WaveFileReader reader;

        public AudioPlayer(Stream stream)
        {
            reader = new WaveFileReader(stream);
            outputDevice = new WaveOut();
            outputDevice.Init(new WaveChannel32(reader));
        }

        public void Play()
        {
            outputDevice.Play();
        }

        public void Stop()
        {
            outputDevice.Stop();
            reader.Position = 0;
        }

        public void Dispose()
        {
            outputDevice.Dispose();
            reader.Dispose();
        }
    }
}
