using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CaroGame.Models
{
	public partial class Board : IBoardData
	{
		public CellValue NextPlayer()
		{
			if (IsFinished) {
				return CellValue.None;
			}
			return PlayerTurns[_turns.Count % PlayerTurns.Length];
		}

		ReadOnlyCollection<Turn> IBoardData.GetTurns(){
			return new ReadOnlyCollection<Turn>(_turns);
		}

		public CellValue CellAt(sbyte col, sbyte row)
        {
            if (col < 0 || row < 0 || col >= WidthSize || row >= HeightSize)
            {
                return CellValue.None;
            }

            return CellMatrix[col, row];
        }

		public bool IsFinished{ 
			get{
				return _isFinished;
			}
			protected set{
				if (_isFinished != value) {
					_isFinished = value;
					Finished (this);
				}
			}
		}
	}
}
