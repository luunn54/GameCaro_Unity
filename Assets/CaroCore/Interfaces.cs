using System;
using System.Collections.ObjectModel;

namespace CaroGame.Models
{
	public enum CellValue : sbyte {
		None = 0,
		FirstPlayer = 1,
		SecondPlayer = 2,
	}

    public interface IGameController
    {
        CellValue CellAt(sbyte col, sbyte row);
        ReadOnlyCollection<Turn> GetTurns();
        sbyte NumberWinRequest();

		void Start();
		void Stop();
    }

	public interface IPlayer
	{
		// user info
		string Name{ get;}

		// Game flow
		void BeginGame(IGameController gameController, sbyte widthBoard, sbyte heightBoard);
		Cell NextTurn(Cell? lastCell);
		void EndGame(bool isWin);
	}

	public interface IBoardControl
	{
		void init (String player1, String player2, sbyte width, sbyte height);
		bool NextTurn (Cell cell);
		bool BackTurn ();
		void ForceFinish (CellValue playerLose);
	}

	// define Delegate
	public delegate void InitHanlder(IBoardViewer boardViewer);
	public delegate void NextTurnHanlder(IBoardViewer boardViewer, Turn turn);
	public delegate void BackTurnHanlder(IBoardViewer boardViewer, Turn turn);
	public delegate void FinishHanlder(IBoardViewer boardViewer);

	public interface IBoardViewer
	{
		// get board info
		ReadOnlyCollection<Turn> GetTurns ();
		CellValue CellAt (sbyte col, sbyte row);
		// size of board
		sbyte WidthSize{ get;}
		sbyte HeightSize{ get;}

		// did finish
		bool Finish { get;}
		CellValue NextPlayer ();
		Cell[] WinCell { get; }
		CellValue PlayerWin { get; }

		// info user playing
		String FirstUser { get;}
		String SecondUser { get;}

		// event
		event InitHanlder Ininted;
		event NextTurnHanlder NextedTurn;
		event BackTurnHanlder BackedTurn;
		event FinishHanlder Finished;
	}
}