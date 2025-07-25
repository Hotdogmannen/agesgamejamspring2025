using Godot;
using System;
using System.Collections.Generic;

public partial class BoosterPad : Area3D
{
    [Export] public float Force {get; set;}
    [Export] public float EffectSharpness {get; set;}
    private MeshInstance3D mesh;
    private List<RigidBody3D> collidingBodies = new List<RigidBody3D>();

    private AudioStreamPlayer3D audioPlayer;


    public override void _Ready()
    {
        base._Ready();
        mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        audioPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
    }

    public void OnBodyEntered(Node body){
        if(body is RigidBody3D rb){
            collidingBodies.Add(rb);
            rb.ApplyImpulse(Transform.Basis.Z * Force);
            if(mesh.GetActiveMaterial(0) is StandardMaterial3D material){
                material.AlbedoColor = new Color(1f,0.5f,0.5f);
            }
            Scale = Vector3.One * 1.1f;
            audioPlayer.Play();
            
        }
    }
    public void OnBodyExited(Node body){
        if(body is RigidBody3D rb){
            collidingBodies.Remove(rb);
        }
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        if(mesh.GetActiveMaterial(0) is StandardMaterial3D material){
                material.AlbedoColor = material.AlbedoColor.Lerp(new Color(1f,1f,1f),(float)delta * EffectSharpness);
            }
        Scale = Scale.Lerp(Vector3.One,(float)delta*EffectSharpness);
        
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        foreach (var rb in collidingBodies)
        {
            rb.ApplyForce(Transform.Basis.Z * Force);
            float angle = Mathf.RadToDeg(rb.Transform.Basis.Z.SignedAngleTo(-Transform.Basis.Z,Vector3.Up));
            Vector3 rot = Vector3.Up * Math.Sign(angle);
            if(Math.Abs(angle) > 15) rb.ApplyTorque(rot);
        }
    }

}
