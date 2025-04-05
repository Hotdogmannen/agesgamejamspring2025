using Godot;

public partial class LevelEnededScreen : Control
{
	[Export] private PackedScene gameScene { get; set; }
	[Export] private PackedScene menuScene { get; set; }
	[Export] private Label bestTimeLabel { get; set; }
	
	public void Init(bool isNewBestTime)
	{
		if (!FileAccess.FileExists(SaveAndLoad.TIME_SAVE_FILE_PATH))
    	{
			GD.PrintErr("Save file not found at: " + SaveAndLoad.TIME_SAVE_FILE_PATH);
        	return;
    	}
		
		using FileAccess file = FileAccess.Open(SaveAndLoad.TIME_SAVE_FILE_PATH, FileAccess.ModeFlags.Read);

        Json json = new Json();
        Error parseResult = json.Parse(file.GetLine());
        if (parseResult != Error.Ok)
        {
            GD.PrintErr($"JSON Parse Error: {json.GetErrorMessage()} in {file.GetLine()} at line {json.GetErrorLine()}");
            return;
        }

		float bestTime = (float)json.Data;

		string bestTimeText =  "Best time: " + (int)(bestTime / 60) + ":" + (int)(bestTime % 60);

		bestTimeLabel.Text = bestTimeText;
	}

	public void OnRestart()
	{
		GetTree().ChangeSceneToPacked(gameScene);
	}

	public void OnMainMenu()
	{
		GetTree().ChangeSceneToPacked(menuScene);
	}
}
