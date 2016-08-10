using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CaroGame.Models
{
	public partial class Board : IBoardData, IBoardControl
	{
        public const sbyte NUMBER_WIN_REQUEST = 3;
        public static readonly CellValue[] PlayerTurns = new CellValue[]{
            CellValue.FirstPlayer,
            CellValue.SecondPlayer
        };

		// size of board
		public sbyte WidthSize{ get; protected set;}
		public sbyte HeightSize{ get; protected set;}

		// matix of cells
		protected CellValue[,] CellMatrix;

		// list turn of user play
		protected IList<Turn> _turns;

        // did finish
		private bool _isFinished;
        public Cell[] WinCell { get; protected set; }
        public CellValue PlayerWin { get; protected set; }

		// event
		public event InitHanlder Inited;

		public event TurnChangeHanlder TurnChanged;

		public event NextTurnHanlder NextedTurn;
		public event BackTurnHanlder BackedTurn;
		public event FinishHanlder Finished;
	}
}
