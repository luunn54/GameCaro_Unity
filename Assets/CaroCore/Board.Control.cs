using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CaroGame.Models
{
	public partial class Board : IBoardControl
	{
		void IBoardControl.Init(sbyte width, sbyte height)
		{
			// set width, height
			this.WidthSize = width;
			this.HeightSize = height;

			// init matrix cells
			CellMatrix = new CellValue[WidthSize, HeightSize];

			// reset value
			for (sbyte x = 0; x <= CellMatrix.GetUpperBound (0); x++)
				for (sbyte y = 0; y <= CellMatrix.GetUpperBound (1); y++)
					CellMatrix [x, y] = CellValue.None;

			_turns = new List<Turn> ();
			_isFinished = false;

			Ininted (this);
		}

		bool IBoardControl.NextTurn(CellValue player, Cell cell) {
			if (IsFinished)
				return false;

			CellValue nextPlayer = NextPlayer();
			if (player != nextPlayer)
				return false;

			if (cell.X < 0 || cell.Y < 0 || cell.X >= WidthSize || cell.Y >= HeightSize || CellMatrix[cell.X, cell.Y] != CellValue.None) {
                Console.WriteLine(cell.X + "," + cell.Y + " : " + CellMatrix[cell.X, cell.Y].ToString());
				return false;
			}

			Turn turn = new Turn () {
				Cell = cell,
				CellValue = player
			};

			_turns.Add(turn);
			CellMatrix[cell.X, cell.Y] = player;
			NextedTurn (this, turn);

            CheckFinish();
			return true;
		}

		bool IBoardControl.BackTurn()
        {
            if (_turns.Count <= 0)
                return false;
            var turn = _turns[_turns.Count - 1];

            CellMatrix[turn.Cell.X, turn.Cell.Y] = CellValue.None;
            _turns.RemoveAt(_turns.Count - 1);
			BackedTurn (this, turn);

            CheckFinish();
            return true;
        }

        private Cell[] CheckFinishFromCell(Cell cell)
        {
            sbyte X = cell.X, Y = cell.Y;
            CellValue player = CellAt(X, Y);
            if (player == CellValue.None)
                return null;

            // -X+ Y
            sbyte count = 0;
            sbyte XMax = X, XMin = X;
            for (++XMax; XMax < WidthSize; XMax++)
            {
                if (CellAt(XMax, Y) != player)
                {
                    break;
                }

                count++;
            }

            for (--XMin; XMin >= 0; XMin--)
            {
                if (CellAt(XMin, Y) != player)
                {
                    break;
                }

                count++;
                if (XMin == 0)
                    break;
            }

            if (count >= NUMBER_WIN_REQUEST)
            {
                Cell []cells = new Cell[XMax - XMin - 1];
                sbyte i = 0;
                for (XMin++; XMin < XMax; XMin++)
                {
                    cells[i].X = XMin;
                    cells[i].Y = Y;
                    i++;
                }
                return cells;
            }

            // X -Y+ ------------------
            count = 0;
            sbyte YMax = Y, YMin = Y;
            for (++YMax; YMax < HeightSize; YMax++)
            {
                if (CellAt(X, YMax) != player)
                {
                    break;
                }

                count++;
            }

            for (--YMin; YMin >= 0; YMin--)
            {
                if (CellAt(X, YMin) != player)
                {
                    break;
                }

                count++;
            }

            if (count >= NUMBER_WIN_REQUEST)
            {
                Cell[] cells = new Cell[YMax - YMin - 1];
                sbyte i = 0;
                for (YMin++; YMin < YMax; YMin++)
                {
                    cells[i].X = X;
                    cells[i].Y = YMin;
                    i++;
                }
                return cells;
            }

            // -X+ -Y+ ---------
            count = 0;
            YMax = YMin = Y;
            XMax = XMin = X;
            for (++YMax, ++XMax; YMax < HeightSize && XMax < WidthSize; YMax++, XMax++)
            {
                if (CellAt(XMax, YMax) != player)
                {
                    break;
                }

                count++;
            }

            for (--YMin, --XMin; YMin >= 0 && XMin >= 0; YMin--, XMin--)
            {
                if (CellAt(XMin, YMin) != player)
                {
                    break;
                }

                count++;
            }

            if (count >= NUMBER_WIN_REQUEST)
            {
                Cell[] cells = new Cell[XMax - XMin - 1];
                sbyte i = 0;
                for (XMin++, YMin++; XMin < XMax && YMin < YMax; XMin++, YMin++)
                {
                    cells[i].X = XMin;
                    cells[i].Y = YMin;
                    i++;
                }
                return cells;
            }

            // -X+ +Y- -------------
            count = 0;
            YMax = YMin = Y;
            XMax = XMin = X;
            for (++YMax, --XMin; YMax < HeightSize && XMin >= 0; YMax++, XMin--)
            {
                if (CellAt(XMin, YMax) != player)
                {
                    break;
                }

                count++;
            }

            for (--YMin, ++XMax; YMin >= 0 && XMax < WidthSize; YMin--, XMax++)
            {
                if (CellAt(XMax, YMin) != player)
                {
                    break;
                }

                count++;
            }

            if (count >= NUMBER_WIN_REQUEST)
            {
                Cell[] cells = new Cell[XMax - XMin - 1];
                sbyte i = 0;
                for (XMin++, YMin++; XMin < XMax && YMin < YMax; XMin++, YMin++)
                {
                    cells[i].X = XMin;
                    cells[i].Y = YMin;
                    i++;
                }
                return cells;
            }

            return null;
        }

        private void CheckFinish()
        {
			if (IsFinished)
            {
                return;
            }

            // check end game, player win, update finish value
            // bla bla...
            if(_turns.Count == 0)
            {
                return;
            }
            
            Turn lastTurn = _turns[_turns.Count - 1];
            WinCell = CheckFinishFromCell(lastTurn.Cell);
            bool didFinish = (WinCell != null);
			if (didFinish)
            {
                PlayerWin = CellMatrix[lastTurn.Cell.X, lastTurn.Cell.Y];
				IsFinished = didFinish;
            }
        }

		void IBoardControl.ForceFinish (CellValue playerLose)
        {
            WinCell = null;
            foreach (CellValue playerTurn in PlayerTurns)
            {
                if (playerTurn != playerLose)
                {
                    PlayerWin = playerTurn;
                    break;
                }
            }

			IsFinished = true;
        }
	}
}
