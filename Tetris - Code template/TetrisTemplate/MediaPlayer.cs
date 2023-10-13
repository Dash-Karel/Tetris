using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace TetrisTemplate
{
    internal class MediaPlayer
    {
        //this class is used instead of the built-in mediaplayer because it was buggy(happens with more people on the internet)

        bool stopped = false;
        int currentSong = 0;
        List<SoundEffectInstance> songs = new List<SoundEffectInstance>();
        
        public void Update()
        {
            //check if the current song has finished
            if (songs[currentSong] != null)
            {
                if (songs[currentSong].State == SoundState.Stopped && !stopped)
                {
                    //go to the next song and play it
                    if (++currentSong >= songs.Count)
                        currentSong = 0;
                    songs[currentSong].Play();
                }
            }
        }
        public void AddSongToQueue(SoundEffect soundEffect)
        {
            songs.Add(soundEffect.CreateInstance());
        }
        public void Play()
        {
            stopped = false;
            if (songs[currentSong].State != SoundState.Playing)
            {
                songs[currentSong].Play();
            }
        }
        public void Stop()
        {
            stopped = true;
            if (songs[currentSong].State != SoundState.Stopped)
                songs[currentSong].Stop();
        }
        public void Pause()
        {
            if (songs[currentSong].State == SoundState.Playing)
                songs[currentSong].Pause();
        }
    }
}
