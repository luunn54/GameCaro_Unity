using System;

namespace CaroGame.Models
{
	public struct Cell
	{
		public sbyte X, Y;
	}

	public struct Turn
	{
		public CellValue CellValue;
		public Cell Cell;
	}
}
