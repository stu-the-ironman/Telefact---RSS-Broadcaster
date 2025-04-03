using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Telefact
{
    public class MusicManager
    {
        private readonly List<string> musicFiles;
        private int currentTrackIndex;
        private WaveOutEvent waveOut;
        private AudioFileReader audioFileReader;

        public MusicManager()
        {
            Console.WriteLine("[MusicManager] DEBUG: Initializing MusicManager...");

            string musicDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Music");

            if (!Directory.Exists(musicDirectory))
            {
                Console.WriteLine($"[MusicManager] ERROR: Music directory not found: {musicDirectory}");
                return;
            }

            musicFiles = Directory.GetFiles(musicDirectory, "*.mp3", SearchOption.AllDirectories).ToList();

            if (musicFiles.Count == 0)
            {
                Console.WriteLine("[MusicManager] ERROR: No MP3 files found in the music directory or its subfolders.");
                return;
            }

            Console.WriteLine($"[MusicManager] INFO: Found {musicFiles.Count} MP3 file(s).");

            waveOut = new WaveOutEvent();
            waveOut.PlaybackStopped += WaveOutEvent_PlaybackStopped;

            ShufflePlaylist();
            PlayNextTrack();
        }

        private void ShufflePlaylist()
        {
            Console.WriteLine("[MusicManager] DEBUG: Shuffling playlist.");
            Random rng = new Random();
            musicFiles.Sort((a, b) => rng.Next(-1, 2));
            currentTrackIndex = 0;
        }

        private void PlayNextTrack()
        {
            if (musicFiles.Count == 0)
            {
                Console.WriteLine("[MusicManager] ERROR: No tracks available to play.");
                return;
            }

            if (currentTrackIndex >= musicFiles.Count)
            {
                Console.WriteLine("[MusicManager] INFO: Playlist completed. Reshuffling...");
                ShufflePlaylist();
            }

            string nextTrack = musicFiles[currentTrackIndex];
            Console.WriteLine($"[MusicManager] INFO: Now playing: {Path.GetFileName(nextTrack)}");

            try
            {
                audioFileReader?.Dispose();
                audioFileReader = new AudioFileReader(nextTrack);

                waveOut.Stop();
                waveOut.Init(audioFileReader);
                waveOut.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MusicManager] ERROR: Failed to play track: {ex.Message}");
            }

            currentTrackIndex++;
        }

        private void WaveOutEvent_PlaybackStopped(object? sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                Console.WriteLine($"[MusicManager] ERROR: Playback stopped with exception: {e.Exception.Message}");
            }
            else
            {
                Console.WriteLine("[MusicManager] DEBUG: Playback stopped. Moving to next track...");
                PlayNextTrack();
            }
        }
    }
}
