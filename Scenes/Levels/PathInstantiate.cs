using Godot;
using System;

public partial class PathInstantiate : Path3D
{
    [Export] public PackedScene SceneToInstatiate {get; set;}
    [Export] public float spawnSpacing {get; set;}

    public override void _Ready()
    {
        if(spawnSpacing == 0) return;
        base._Ready();
        float length = Curve.GetBakedLength();
        GD.Print(length);
        for(float t = 0; t < length; t+= spawnSpacing){
            Vector3 samplePos = Curve.SampleBaked(t,true);
            var node = SceneToInstatiate.Instantiate() as Node3D;
            AddChild(node);
            node.Position = samplePos;
        }
    }


}
