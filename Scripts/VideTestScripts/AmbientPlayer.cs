using Godot;
using System;

public partial class AmbientPlayer : Node
{

    [Export] public Vector2 AudioDelayRange {get; set;}
    private AudioStreamPlayer player;

    float time;
    float curGoal;
    public override void _Ready()
    {
        base._Ready();
        player = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        curGoal = (float)GD.RandRange(AudioDelayRange.X, AudioDelayRange.Y);
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


}
