using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
/// <summary>
/// This class is used instead of the built-in mediaplayer because it was buggy(happens with more people on the internet)
/// </summary>
internal class MediaPlayer
{
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
    //Ads a sound effect to list of sound effects
    public void AddSongToQueue(SoundEffect soundEffect)
    {
        songs.Add(soundEffect.CreateInstance());
    }
    //Starts playing the current song.
    public void Play()
    {
        stopped = false;
        if (songs[currentSong].State != SoundState.Playing)
        {
            songs[currentSong].Play();
        }
    }
    //Stops the current song.
    public void Stop()
    {
        stopped = true;
        if (songs[currentSong].State != SoundState.Stopped)
            songs[currentSong].Stop();
    }
    //Pauses the current song.
    public void Pause()
    {
        if (songs[currentSong].State == SoundState.Playing)
            songs[currentSong].Pause();
    }
}
