using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CaroGame.Models
{
    class GameController : IGameController
    {
		protected IBoardControl _boardController;
		public IBoardViewer BoardViewer{
			get{
				return _boardViewer;
			}
		}
		protected IBoardViewer _boardViewer;
        protected Dictionary<CellValue, IPlayer> _players;

		public GameController(){
			var board = new Board ();
			_boardViewer = board;
			_boardController = board;
		}

        public void Init(IPlayer player1, IPlayer player2, sbyte widthBoard, sbyte heightBoard)
        {
            _players = new Dictionary<CellValue, IPlayer>() { 
                {CellValue.FirstPlayer, player1},
                {CellValue.SecondPlayer, player2 }
            };

			_boardController.init(player1.Name, player2.Name, widthBoard, heightBoard);
            foreach (KeyValuePair<CellValue, IPlayer> entry in _players)
            {
                entry.Value.BeginGame(this, _boardViewer.WidthSize, _boardViewer.HeightSize);
            }
        }

        public void Start()
        {
            Cell? lastCell = null;

            CellValue currentPlayer = CellValue.FirstPlayer;
            Cell cellInput = default(Cell);
            while (!_boardViewer.Finish)
            {
                currentPlayer = _boardViewer.NextPlayer();
                IPlayer player = _players[currentPlayer];
                bool validCell = false;
                
                try
                {
                    cellInput = player.NextTurn(lastCell);
                    Console.WriteLine(currentPlayer.ToString() + ":" + cellInput.X + "," + cellInput.Y);
					validCell = _boardController.NextTurn(cellInput);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exe " + e.ToString());
                }

                if (validCell)
                {
                    lastCell = cellInput;
                }
                else
                {
                    Console.WriteLine("ko hop le");
					_boardController.ForceFinish(currentPlayer);
                }
            }

            log();
            Console.WriteLine("Winner: " + _boardViewer.PlayerWin.ToString());
            foreach(Cell cell in _boardViewer.WinCell){
                Console.WriteLine(cell.X + "," + cell.Y);
            }

            foreach (KeyValuePair<CellValue, IPlayer> entry in _players)
            {
                entry.Value.EndGame(_boardViewer.PlayerWin == entry.Key);
            }
        }

		public void Stop(){
			
		}

        public ReadOnlyCollection<Turn> GetTurns()
        {
            return _boardViewer.GetTurns();
        }

        public sbyte NumberWinRequest()
        {
            return Board.NUMBER_WIN_REQUEST;
        }

        public CellValue CellAt(sbyte col, sbyte row)
        {
            return _boardViewer.CellAt(col, row);
        }

        private void log()
        {
            Console.Write("  ");
            for (sbyte j = 0; j < _boardViewer.WidthSize; j++)
				Console.Write(" " + (j % 10).ToString());
            Console.WriteLine("");

			Console.Write("  ");
			for (sbyte j = 0; j < _boardViewer.WidthSize; j++)
				Console.Write("__");
			Console.WriteLine("");

            for (sbyte i = 0; i < _boardViewer.HeightSize; i++)
            {
                Console.Write((i % 10) + "|");
                for (sbyte j = 0; j < _boardViewer.WidthSize; j++)
                {
                    var cellValue = _boardViewer.CellAt(j, i);
                    switch (cellValue)
                    {
                        case CellValue.None:
                            Console.Write(" -");
                            break;
                        case CellValue.FirstPlayer:
                            Console.Write(" x");
                            break;
                        case CellValue.SecondPlayer:
                            Console.Write(" o");
                            break;
                    }
                }
                Console.WriteLine("");
            }
        }
    }
}
