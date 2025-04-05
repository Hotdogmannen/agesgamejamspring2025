using Godot;
using System;

public partial class TestBoatController : RigidBody3D
{
    [ExportCategory("General")]
    [Export] public float ForwardForce {get; set;}
    [Export] public float RotationForce {get; set;}
    [Export] public float oarSpeed {get; set;}
    [ExportCategory("General")]
    [Export] public float OarAnimationHeight {get; set;}
    [Export] public float OarAnimationWidth {get; set;}
    [Export] public float OarAnimationSharpness {get; set;}
    [ExportCategory("Camera")]
    
    [Export] public float maxSpeedFOV {get; set;}
    [Export] public float maxFOV {get; set;}
    [Export] public float minFOV {get; set;}
    [Export] public float fovSharpness {get; set;}
    [Export] public float CameraPositionSharpness {get; set;}
    [Export] public float CameraRotationSharpness {get; set;}

    float leftOarPower;
    float rightOarPower;
    Node3D cameraNode;
    Node3D oarLeft;
    Node3D oarRight;
    Camera3D camera;

    const float margin = 0.1f;

    public override void _Ready()
    {
        base._Ready();
        cameraNode = GetNode<Node3D>("CameraController");
        camera= GetNode<Camera3D>("CameraController/CameraRig/Camera3D");
        oarLeft = GetNode<Node3D>("GraphicRoot/OarLeft");
        oarRight = GetNode<Node3D>("GraphicRoot/OarRight");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        bool rowingLeft = Input.IsActionPressed("move_left");
        bool rowingRight = Input.IsActionPressed("move_right");
        
        //ApplyForce(Vector3.Right*inputAxis);

        if(rowingLeft){
            leftOarPower += (float)delta * oarSpeed;
        }
        else{
            leftOarPower = 0;
        }
        if(rowingRight){
            rightOarPower += (float)delta * oarSpeed;
        }
        else
        {
            rightOarPower = 0;
        }

        if(rowingRight && rightOarPower > 0.5f && rightOarPower <= 1f){
            ApplyOarPower(delta,1);
        }
        if(rowingLeft && leftOarPower > 0.5f && leftOarPower <= 1f){
            ApplyOarPower(delta,-1);
        }

        float velocityWeight = Fade(Math.Clamp(LinearVelocity.Length()/maxSpeedFOV,0f,1f));
        
        float targetFov = Mathf.Lerp(minFOV,maxFOV,velocityWeight);
        camera.Fov = Mathf.Lerp(camera.Fov,targetFov,(float)delta * fovSharpness);

        UpdateOarAnimation(delta);
    }

    private void ApplyOarPower(double delta, float dir){
            ApplyForce(Transform.Basis.Z*(-ForwardForce*(float)delta));
            ApplyTorque(Vector3.Up*(RotationForce*dir*(float)delta));
    }

    private void UpdateOarAnimation(double delta)
    {
        if(leftOarPower > margin && leftOarPower <= 1f){
            SetOarRotation(delta, oarLeft,leftOarPower);
        }
        else{
            SetOarRotation(delta, oarLeft,0);
        }
        if(rightOarPower > margin && rightOarPower <= 1f){
            SetOarRotation(delta, oarRight,rightOarPower, true);
        }
        else{
            SetOarRotation(delta, oarRight,0, true);
            
        }
    }

    private void SetOarRotation(double delta,Node3D oar, float power,bool flipped = false){
        var target = new Vector3(
            0f,
            (float)((flipped? -1 : 1)*Math.Cos(power*2*Math.PI) * OarAnimationWidth),
            (float)((flipped? 1 : -1)*Math.Sin(power*2*Math.PI) * OarAnimationHeight));

        float weight = 1f - Mathf.Exp(-OarAnimationSharpness * (float)delta);
        oar.RotationDegrees = oar.RotationDegrees.Lerp(target,weight);

    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        
        float posWeight = 1f - Mathf.Exp(-CameraPositionSharpness * (float)delta);
        float rotWeight = 1f - Mathf.Exp(-CameraRotationSharpness * (float)delta);
        cameraNode.Position = cameraNode.Position.Lerp(Position,posWeight);
        var q1 = new Quaternion(cameraNode.Basis);
        var q2 = new Quaternion(Basis);
        cameraNode.Basis = new Basis(q1.Slerp(q2,rotWeight));
    }

    public static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);       
}
