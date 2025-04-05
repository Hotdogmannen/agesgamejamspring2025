using Godot;
using System;

public partial class CameraFollow : Camera3D
{
    [Export] Vector3 Offset {get; set;}
    [Export] float Sharpness {get; set;}
    [Export] NodePath FollowNode {get; set;}
    Node3D followNode;

    public override void _Ready()
    {
        base._Ready();
        followNode = GetNode<Node3D>(FollowNode);
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        Position = Position.Lerp(followNode.Position + Offset,(float)delta*Sharpness);
    }
    
}
