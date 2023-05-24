using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Donkey_Kong
{
    public class AudioPlayer
    {
        private WaveOut outputDevice;
        private WaveFileReader reader;
        private TimeSpan audioDuration;
        private DateTime startTime;

        public AudioPlayer(Stream stream)
        {
            reader = new WaveFileReader(stream);
            outputDevice = new WaveOut();
            outputDevice.Init(new WaveChannel32(reader));

            CalculateAudioDuration();
        }

        private void CalculateAudioDuration()
        {
            audioDuration = TimeSpan.FromSeconds(reader.Length / (double)reader.WaveFormat.AverageBytesPerSecond);
        }

        public void Play()
        {
            startTime = DateTime.Now;
            outputDevice.Play();

            Task.Run(async () =>
            {
                await Task.Delay(audioDuration);
                PlayAgain();
            });
        }
        public void PlayOnce()
        {
            outputDevice.Play();
        }

        private void PlayAgain()
        {
            // Stop and reset the reader
            outputDevice.Stop();
            reader.Position = 0;

            // Play again
            startTime = DateTime.Now;
            outputDevice.Play();

            Task.Run(async () =>
            {
                await Task.Delay(audioDuration);
                PlayAgain();
            });
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