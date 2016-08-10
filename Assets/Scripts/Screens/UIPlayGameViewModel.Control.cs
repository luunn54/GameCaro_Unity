using UnityEngine;
using UnityEngine.UI;
using UIGianty;
using UIGianty.MVVM;
using System;
using CaroGame.Models;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public partial class UIPlayGameViewModel {

#region Fields
	private IBoardControl _boardController;
	private IBoardData _boardData;

	protected Dictionary<CellValue, IPlayer> _players;

#endregion

#region Custom implementation

	private void StartPlay(int index, string name){
		var board = new Board ();

		_boardData = board;
		_boardController = board;

		_boardData.Ininted += BoardInited;
		_boardData.Finished += BoardDataFinished;
		_boardData.BackedTurn += BoardBackedTurn;
		_boardData.NextedTurn += BoardNextedTurn;

		var firstPlayer = new StartGame.RandomPlayer ();
		var secondPlayer = new StartGame.RandomPlayer ();

		_players = new Dictionary<CellValue, IPlayer>() { 
			{CellValue.FirstPlayer, firstPlayer},
			{CellValue.SecondPlayer, secondPlayer }
            };

		_boardController.Init (30, 30);
	}

#endregion
}
