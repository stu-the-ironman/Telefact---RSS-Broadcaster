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
        private readonly ConfigSettings config;

        public MusicManager(ConfigSettings config)
        {
            Console.WriteLine("[MusicManager] DEBUG: Initializing MusicManager...");
            this.config = config;

            string musicDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Music");

            if (!Directory.Exists(musicDirectory))
            {
                Console.WriteLine($"[MusicManager] ERROR: Music directory not found: {musicDirectory}");
                musicFiles = new List<string>();
                return;
            }

            // Load all MP3 files from the directory and its subdirectories
            musicFiles = Directory.GetFiles(musicDirectory, "*.mp3", SearchOption.AllDirectories).ToList();

            if (musicFiles.Count == 0)
            {
                Console.WriteLine("[MusicManager] ERROR: No MP3 files found in the music directory or its subfolders.");
                return;
            }

            Console.WriteLine($"[MusicManager] INFO: Found {musicFiles.Count} MP3 file(s).");

            waveOut = new WaveOutEvent();
            waveOut.PlaybackStopped += OnPlaybackStopped;

            currentTrackIndex = 0;
        }

        public void Play()
        {
            if (musicFiles.Count == 0)
            {
                Console.WriteLine("[MusicManager] ERROR: No tracks available to play.");
                return;
            }

            PlayTrack(currentTrackIndex);
        }

        private void PlayTrack(int trackIndex)
        {
            string trackPath = musicFiles[trackIndex];
            Console.WriteLine($"[MusicManager] INFO: Now playing: {Path.GetFileName(trackPath)}");

            try
            {
                audioFileReader?.Dispose();
                audioFileReader = new AudioFileReader(trackPath);

                waveOut?.Stop();
                waveOut?.Init(audioFileReader);
                waveOut?.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MusicManager] ERROR: Failed to play track: {ex.Message}");
            }
        }

        private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                Console.WriteLine($"[MusicManager] ERROR: Playback stopped with exception: {e.Exception.Message}");
            }
            else
            {
                Console.WriteLine("[MusicManager] INFO: Playback stopped. Moving to next track...");
                currentTrackIndex = (currentTrackIndex + 1) % musicFiles.Count;
                PlayTrack(currentTrackIndex);
            }
        }
    }
}
