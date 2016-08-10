using System;
using System.Collections.ObjectModel;

namespace CaroGame.Models
{
	public enum CellValue : sbyte {
		None = 0,
		FirstPlayer = 1,
		SecondPlayer = 2,
	}

	public interface IPlayer
	{
		// user info
		void Init(string name);

		string Name{ get;}
		// Game flow
		void BeginGame(IBoardData boardData, CellValue playerType);
		Cell NextTurn(Turn? lastTurn);
		void EndGame(bool isWin);
	}

	public interface IBoardControl
	{
		void Init (sbyte width, sbyte height);
		void Start ();
		bool NextTurn (CellValue player, Cell cell);
		bool BackTurn ();

		void ForceFinish (CellValue playerLose);
	}

	public interface IBoardData
	{
		// get board info
		CellValue NextPlayer ();
		ReadOnlyCollection<Turn> GetTurns ();
		CellValue CellAt (sbyte col, sbyte row);

		// size of board
		sbyte WidthSize{ get;}
		sbyte HeightSize{ get;}

		// did finish
		bool IsFinished { get;}
		Cell[] WinCell { get; }
		CellValue PlayerWin { get; }

		// event
		// did init board
		event InitHanlder Inited;
		// turn changed
		event TurnChangeHanlder TurnChanged;
		// did lay by player
		event NextTurnHanlder NextedTurn;
		// did rollback board
		event BackTurnHanlder BackedTurn;
		// finish game changed
		event FinishHanlder Finished;
	}

	// define Delegate
	public delegate void InitHanlder(IBoardData boardData);
	public delegate void TurnChangeHanlder(IBoardData boardData, Turn? lastTurn);
	public delegate void NextTurnHanlder(IBoardData boardData, Turn turn);
	public delegate void BackTurnHanlder(IBoardData boardData, Turn turn);
	public delegate void FinishHanlder(IBoardData boardData);
}