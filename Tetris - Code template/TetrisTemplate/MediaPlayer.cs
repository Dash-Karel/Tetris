using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
/// <summary>
/// This class is used instead of the built-in mediaplayer because it was buggy(happens with more people on the internet)
/// </summary>
internal class MediaPlayer
{
    //bool for keeping track if the song was intentionally stopped
    bool stopped = false;

    //bool for checking if the song is in the paused state
    bool paused = false;

    //variable to keep track of which song is playing
    int currentSong = 0;

    //variable to keep track of how long a song has been playing
    float playingForSeconds = 0;

    //a list of sound effect instances representing the songs
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

        //increases the playingForSecond if the song is playing
        if(!paused && !stopped)
            playingForSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    //Ads a song/soundeffect to the list of songs
    public void AddSongToQueue(SoundEffect soundEffect)
    {
        songs.Add(soundEffect.CreateInstance());
    }
    //Starts playing or resumes the current song.
    public void Play()
    {
        stopped = false;
        paused  = false;
        if (songs[currentSong].State != SoundState.Playing)
        {
            songs[currentSong].Play();
        }
    }
    //Stops the current song.
    public void Stop()
    {
        playingForSeconds = 0;
        stopped = true;
        paused =false;
        if (songs[currentSong].State != SoundState.Stopped)
            songs[currentSong].Stop();
    }
    //Pauses the current song.
    public void Pause()
    {
        paused = true;
        if (songs[currentSong].State == SoundState.Playing)
            songs[currentSong].Pause();
    }

    //Plays or pauses the song based on the current state
    public void TogglePlaying()
    {
        if (paused)
            Play();
        else
            Pause();
    }

    //skips the current song
    public void Skip()
    {
        Stop();
        if (++currentSong >= songs.Count)
            currentSong = 0;
        Play();
    }
    
    //Goes to the beginning of the song if it has been playing for more than 2 seconds, else the player goes to the previous song in the queue
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
