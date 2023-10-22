using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

internal class MediaPlayer
{
    //this class is used instead of the built-in mediaplayer because it was buggy(happens with more people on the internet)

    bool stopped = false;
    bool paused = false;
    int currentSong = 0;
    float playingForSeconds = 0;
    List<SoundEffectInstance> songs = new List<SoundEffectInstance>();

    public void Update(GameTime gameTime)
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
                playingForSeconds = 0;
            }
        }
        if(!paused && !stopped)
            playingForSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    public void AddSongToQueue(SoundEffect soundEffect)
    {
        songs.Add(soundEffect.CreateInstance());
    }
    public void Play()
    {
        stopped = false;
        paused  = false;
        if (songs[currentSong].State != SoundState.Playing)
        {
            songs[currentSong].Play();
        }
    }
    void Stop()
    {
        playingForSeconds = 0;
        stopped = true;
        paused =false;
        if (songs[currentSong].State != SoundState.Stopped)
            songs[currentSong].Stop();
    }
    public void Pause()
    {
        paused = true;
        if (songs[currentSong].State == SoundState.Playing)
            songs[currentSong].Pause();
    }
    public void Skip()
    {
        Stop();
        if (++currentSong >= songs.Count)
            currentSong = 0;
        Play();
    }
    public void Previous()
    {
        double currentPlayingForSeconds = playingForSeconds;
        Stop();
        if (currentPlayingForSeconds < 2)
        {
            if(--currentSong < 0)
                currentSong = songs.Count - 1;
        }    
        Play();
    }
}
