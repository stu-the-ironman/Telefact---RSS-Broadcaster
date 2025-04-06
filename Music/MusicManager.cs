using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NAudio.Wave;
using Telefact.Config;

namespace Telefact.Music
{
    public class MusicManager
    {
        private readonly List<string> musicFiles;
        private WaveOutEvent? waveOut;
        private AudioFileReader? audioFileReader;
        private int currentTrackIndex;
        private readonly Random random;
        private readonly ConfigSettings config;

        public MusicManager(ConfigSettings configSettings)
        {
            config = configSettings;
            string musicDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Music");
            random = new Random();

            if (!Directory.Exists(musicDirectory))
            {
                Console.WriteLine("[MusicManager] WARNING: Music directory not found.");
                musicFiles = new List<string>();
                return;
            }

            musicFiles = Directory.GetFiles(musicDirectory, "*.mp3", SearchOption.AllDirectories).ToList();

            if (musicFiles.Count == 0)
            {
                Console.WriteLine("[MusicManager] WARNING: No music files found.");
                return;
            }

            Console.WriteLine($"[MusicManager] INFO: Found {musicFiles.Count} MP3 file(s).");

            currentTrackIndex = random.Next(musicFiles.Count);
        }

        public void Play()
        {
            if (musicFiles.Count == 0)
                return;

            waveOut = new WaveOutEvent();
            string file = musicFiles[currentTrackIndex];
            audioFileReader = new AudioFileReader(file);
            waveOut.Init(audioFileReader);
            waveOut.Play();

            Console.WriteLine($"[MusicManager] INFO: Now playing: {Path.GetFileName(file)}");

            waveOut.PlaybackStopped += (s, e) =>
            {
                audioFileReader?.Dispose();
                waveOut.Dispose();

                currentTrackIndex = (currentTrackIndex + 1) % musicFiles.Count;
                Play(); // play next
            };
        }
    }
}
