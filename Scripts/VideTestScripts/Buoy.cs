using Godot;
using System;

public partial class Buoy : Node3D
{
    [Export] public float bounceForce {get; set;}
    [Export] public float bounceScale {get; set;}
    [Export] public float bounceScaleSharpness {get; set;}

    private Node3D graphic;
    private GpuParticles3D splashParticles;
    private AudioStreamPlayer3D audioPlayer;

    private float scale;

    public override void _Ready()
    {
        base._Ready();
        graphic = GetNode<Node3D>("Graphic");
        splashParticles = GetNode<GpuParticles3D>("SplashParticles");
        audioPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
        SetScale(1f);
    }


    public void OnBodyEntered(Node body){
        if(body is RigidBody3D rb){
            Vector3 dir = rb.GlobalPosition-GlobalPosition;
            GD.Print(rb.LinearVelocity.Dot(dir));
            float impulseForce = Mathf.Max(bounceForce, (-rb.LinearVelocity.Dot(dir))*1.2f);
            rb.ApplyImpulse(dir.Normalized() * impulseForce);
            SetScale(bounceScale);
            splashParticles.Emitting = true;
            audioPlayer.Play();
        }

    }

    public override void _Process(double delta)
    {
        if(bounceScale <= 1f) return;

        float weight = 1f - Mathf.Exp(-bounceScaleSharpness * (float)delta);

        SetScale(Mathf.Lerp(scale,1,weight));
    }

    private void SetScale(float newScale){
        scale = newScale;
        Scale = Vector3.One * scale;
    }

}
