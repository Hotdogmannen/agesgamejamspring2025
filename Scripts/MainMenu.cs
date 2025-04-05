using Godot;

public partial class MainMenu : Control
{
	[Export] private PackedScene _mainScene;

	public void Play()
	{
		GetTree().ChangeSceneToPacked(_mainScene);
	}

	public void Quit()
	{
		GetTree().Quit();
	}
}
