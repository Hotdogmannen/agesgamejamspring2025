using Godot;

public partial class MainMenu : Control
{
	private PackedScene _mainScene;

    public override void _Ready()
    {
        _mainScene = ResourceLoader.Load<PackedScene>("Scenes/Levels/Main.tscn");
    }

	public void Play()
	{
		GetTree().ChangeSceneToPacked(_mainScene);
	}

	public void Quit()
	{
		GetTree().Quit();
	}
}
