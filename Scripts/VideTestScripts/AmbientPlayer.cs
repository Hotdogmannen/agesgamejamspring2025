using Godot;
using System;

public partial class AmbientPlayer : Node
{

    [Export] public Vector2 AudioDelayRange {get; set;}
    [Export] public AudioStream winSound;
    [Export] public AudioStream loseSound;
    private AudioStreamPlayer player;
    private AudioStreamPlayer musicPlayer;
    private AudioStreamPlayer winLosePlayer;

    float time;
    float curGoal;
    public override void _Ready()
    {
        base._Ready();
        player = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
        winLosePlayer = GetNode<AudioStreamPlayer>("WinLosePlayer");
        curGoal = (float)GD.RandRange(AudioDelayRange.X, AudioDelayRange.Y);

        OnRaceStart();
    }


    public override void _Process(double delta)
    {
        base._Process(delta);
        if(player.Playing) return;
        time += (float)delta;

        if(time > curGoal){
            time = 0;
            curGoal = (float)GD.RandRange(AudioDelayRange.X, AudioDelayRange.Y);
            player.Play();
        }
    }

    public void OnRaceStart(){
        musicPlayer.Play();
    }
    public void OnRaceEnd(int type, float time){
        musicPlayer.Stop();
        if(type == 0){
            winLosePlayer.Stream = winSound;
        }
        else{
            winLosePlayer.Stream = loseSound;
        }
        winLosePlayer.Play();
    }


}
