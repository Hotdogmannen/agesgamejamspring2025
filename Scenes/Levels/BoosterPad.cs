using Godot;
using System;
using System.Collections.Generic;

public partial class BoosterPad : Area3D
{
    [Export] public float Force {get; set;}

    private List<RigidBody3D> collidingBodies = new List<RigidBody3D>();
    public void OnBodyEntered(Node body){
        if(body is RigidBody3D rb){
            collidingBodies.Add(rb);
        }
    }
    public void OnBodyExited(Node body){
        if(body is RigidBody3D rb){
            collidingBodies.Remove(rb);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        foreach (var rb in collidingBodies)
        {
            rb.ApplyForce(Transform.Basis.Z * Force);
        }
    }

}
