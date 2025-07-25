using Godot;
using System;

public partial class TestBoatController : RigidBody3D
{
    [ExportCategory("General")]
    [Export] public bool CanMove {get; set;} = false;
    [Export] public float ForwardForce {get; set;}
    [Export] public float RotationForce {get; set;}
    [Export] public float oarSpeed {get; set;}
    [ExportCategory("Animation")]
    [Export] public float BobbingSpeed {get; set;}
    [Export] public float BobbingVelocitySpeedupPercentage {get; set;}
    [Export] public float BobbingMagnitude {get; set;}
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

    double animationTime;
    float leftOarPower;
    private bool hasPlayedEffectLeft;
    float rightOarPower;

    
    
    private bool hasPlayedEffectRight;
    Node3D cameraNode;
    Node3D oarLeft;
    Node3D oarRight;

    GpuParticles3D backParticles;
    GpuParticles3D frontParticles;
    GpuParticles3D frontParticles2;
    GpuParticles3D oarLeftParticles;
    GpuParticles3D oarRightParticles;
    AudioStreamPlayer3D paddleAudioPlayerLeft;
    AudioStreamPlayer3D paddleAudioPlayerRight;
    AudioStreamPlayer3D windAudioPlayer;
    Node3D graphicRoot;
    Camera3D camera;

    const float margin = 0.1f;

    public override void _Ready()
    {
        base._Ready();
        cameraNode = GetNode<Node3D>("CameraController");
        camera= GetNode<Camera3D>("CameraController/CameraRig/Camera3D");
        graphicRoot = GetNode<Node3D>("GraphicRoot");
        oarLeft = GetNode<Node3D>("GraphicRoot/OarLeft");
        oarRight = GetNode<Node3D>("GraphicRoot/OarRight");

        paddleAudioPlayerLeft = GetNode<AudioStreamPlayer3D>("PaddleAudioStreamLeft");
        paddleAudioPlayerRight = GetNode<AudioStreamPlayer3D>("PaddleAudioStreamRight");
        windAudioPlayer = GetNode<AudioStreamPlayer3D>("WindAudioStream");

        backParticles = GetNode<GpuParticles3D>("SplashParticlesBack");
        frontParticles = GetNode<GpuParticles3D>("SplashParticlesFront");
        frontParticles2 = frontParticles.GetNode<GpuParticles3D>("SplashParticlesFront");
        oarLeftParticles = oarLeft.GetNode<GpuParticles3D>("SplashParticles");
        oarRightParticles = oarRight.GetNode<GpuParticles3D>("SplashParticles");
    }

    public override void _Process(double delta)
    {
        float velocityWeight = 0;
        if(CanMove){
            bool rowingLeft = Input.IsActionPressed("move_left");
            bool rowingRight = Input.IsActionPressed("move_right");
            bool rowingBack = Input.IsActionPressed("move_backward");
            
            //ApplyForce(Vector3.Right*inputAxis);

            if(rowingBack){
                ApplyForce(Transform.Basis.Z*(ForwardForce/4f*(float)delta));
            }

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
                if(rightOarPower > 0.7f){
                    oarRightParticles.Emitting = true;
                    if(!hasPlayedEffectRight) paddleAudioPlayerRight.Play();
                    hasPlayedEffectRight = true;
                }
            }
            else {
                hasPlayedEffectRight = false;
                oarRightParticles.Emitting = false;
                }
            if(rowingLeft && leftOarPower > 0.5f && leftOarPower <= 1f){
                ApplyOarPower(delta,-1);
                if(leftOarPower > 0.7f) {
                    oarLeftParticles.Emitting = true;
                    if(!hasPlayedEffectLeft) paddleAudioPlayerLeft.Play();
                    hasPlayedEffectLeft = true;
                }
                
            }
            else {
                hasPlayedEffectLeft = false;
                oarLeftParticles.Emitting = false;
                }

            velocityWeight = Fade(Math.Clamp(LinearVelocity.Length()/maxSpeedFOV,0f,1f));
            
            float targetFov = Mathf.Lerp(minFOV,maxFOV,velocityWeight);
            camera.Fov = Mathf.Lerp(camera.Fov,targetFov,(float)delta * fovSharpness);

            frontParticles.AmountRatio = velocityWeight > 0.7f ? velocityWeight : 0f;
            frontParticles2.AmountRatio = velocityWeight > 0.7f ? velocityWeight : 0f;
            backParticles.AmountRatio = velocityWeight/2f;
        }
        

        UpdateOarAnimation(delta);
        SetWindAudio(velocityWeight);
        ApplyBobbingAnimation(delta);
    }

    private void SetWindAudio(float velocityWeight){
        if(velocityWeight <= margin){
                windAudioPlayer.Playing = false;
        }
        else{
            if(!windAudioPlayer.Playing) windAudioPlayer.Playing = true;
            windAudioPlayer.VolumeDb = Mathf.Lerp(-24,6,velocityWeight);
        }
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
            0.1f,
            (float)((flipped? -1 : 1)*Math.Cos(power*2*Math.PI) * OarAnimationWidth),
            (float)((flipped? 1 : -1)*Math.Sin(power*2*Math.PI) * OarAnimationHeight));

        float weight = 1f - Mathf.Exp(-OarAnimationSharpness * (float)delta);
        oar.RotationDegrees = oar.RotationDegrees.Lerp(target,weight);

    }

    private void ApplyBobbingAnimation(double delta){
        
        float velocityWeight = Fade(Math.Clamp(LinearVelocity.Length()/maxSpeedFOV,0f,1f)*BobbingVelocitySpeedupPercentage+1);
        animationTime += delta * velocityWeight;

        var target = new Vector3(
            (float)Math.Cos(animationTime*BobbingSpeed) * BobbingMagnitude,
            0f,
            (float)Math.Sin(animationTime*BobbingSpeed+0.2f) * BobbingMagnitude);
        graphicRoot.RotationDegrees = target;
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

    public void SetCanMove() => CanMove = true;
    public void SetCantMove(int type, float time) => CanMove = false;
}
