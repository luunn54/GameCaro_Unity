using UnityEngine;
using System.Collections;
using UIGianty;
using CaroGame.Models;

public class RandomPlayer : IPlayer
{
	public static System.Random random = new System.Random();

	private IBoardData _boardData;

	sbyte width, height;
	private string _name;
	string IPlayer.Name
	{
		get
		{
			if (_name == null) {
				_name = string.Format ("Random {0}", random.Next (1000));
			}
			return _name;
		}
	}

	// user info
	void IPlayer.Init(string name){
		if (!string.IsNullOrEmpty (name)) {
			_name = name;
		}
	}

	// Game flow
	void IPlayer.BeginGame(IBoardData boardData, CellValue playerType)
	{
		width = boardData.WidthSize;
		height = boardData.HeightSize;
		_boardData = boardData;
	}

	Cell IPlayer.NextTurn(Turn? lastTurn)
	{
		int size = width * height;
		for (int i = 0; i < size; i++)
		{
			var XRandom = (sbyte)random.Next(width);
			var YRandom = (sbyte)random.Next(height);
			if (_boardData.CellAt(XRandom, YRandom) == CellValue.None)
			{
				return new Cell() { X = XRandom, Y = YRandom };
			}
		}

		Debug.LogError ("??");
		return default(Cell);
	}

	void IPlayer.EndGame(bool isWin) { }
}

public class StartGame : MonoBehaviour {
	// Use this for initialization
	void Start () {
		UIMan.Instance.ShowScreen (UIContentType.SCREEN_HOME);
	}
}
