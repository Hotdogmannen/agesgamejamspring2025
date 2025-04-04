using Godot;
using System.Collections.Generic;

public partial class SaveAndLoad : Node
{
	public const string SAVE_FILE_PATH = "user://savegame.save";

	[Export] private Node3D _target { get; set; }
	[Export] private Path3D _ghostpathNode { get; set; }
	[Export] private float _timeBetweenSaves { get; set; } = 1.0f;
	[Export] private GhostShip _ghostShip { get; set; }
	private List<Vector3> _playerPositions = new List<Vector3>();

	private bool _isSaving = false;

	private float _timer = 0.0f;

    public override void _Ready()
    {
		LoadData();

		_ghostShip.Init(_playerPositions, _timeBetweenSaves);
		_ghostShip.StartGhost();
    }

	public void StartSaving()
	{
		_isSaving = true;
	}

	public void StopSaving()
	{
		_isSaving = false;

		SaveData();

		ClearData();
	}

	public override void _Process(double delta)
	{
		if(!_isSaving)
			return;

		if(_timer >= _timeBetweenSaves)
		{
			_playerPositions.Add(_target.Position);
			_timer = 0.0f;
		}
		else
			_timer += (float)delta;
	}

	public void SaveData()
	{
		if(_target == null)
		{
			GD.PrintErr("No target assigned!");
			return;
		}

		using FileAccess file = FileAccess.Open(SAVE_FILE_PATH, FileAccess.ModeFlags.Write);
		
		foreach (Vector3 pos in _playerPositions)
		{
			string savedPos = Json.Stringify(pos);
			file.StoreLine(savedPos);
		}

		file.Close();
	}
	
	public void LoadData()
	{
		if(_ghostpathNode == null)
		{
			GD.PrintErr("No ghost path3d node assigned!");
			return;
		}

		if (!FileAccess.FileExists(SAVE_FILE_PATH))
    	{
			GD.PrintErr("Save file not found at: " + SAVE_FILE_PATH);
        	return;
    	}
		
		using FileAccess file = FileAccess.Open(SAVE_FILE_PATH, FileAccess.ModeFlags.Read);

		while (file.GetPosition() < file.GetLength())
		{
			string jsonString = file.GetLine();

        	Json json = new Json();
        	Error parseResult = json.Parse(jsonString);
        	if (parseResult != Error.Ok)
        	{
        	    GD.PrintErr($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
        	    continue;
        	}
			
			_playerPositions.Add((Vector3)GD.StrToVar("Vector3" + json.Data.AsString()));
		}
	}

	public void LoadCurve()
	{
		Curve3D curve = new Curve3D();
		for (int i = 0; i < _playerPositions.Count; i++)
		{
			if(i > 0)
			{
				curve.AddPoint(_playerPositions[i], _playerPositions[i - 1]);
			}
			else
				curve.AddPoint(_playerPositions[i]);
		}

		_ghostpathNode.Curve = curve;
	}

	public void ClearData()
	{
		_playerPositions.Clear();
		_timer = 0.0f;
	}
}
