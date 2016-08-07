using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CaroGame.Models
{
	public partial class Board : IBoardViewer
	{
		public CellValue NextPlayer()
		{
			if (Finish) {
				return CellValue.None;
			}
			return PlayerTurns[_turns.Count % PlayerTurns.Length];
		}

		public ReadOnlyCollection<Turn> GetTurns(){
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
	}
}
