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

		_boardData.Inited += BoardInited;
		_boardData.TurnChanged += BoardTurnChanged;
		_boardData.NextedTurn += BoardNextedTurn;
		_boardData.BackedTurn += BoardBackedTurn;
		_boardData.Finished += BoardDataFinished;

		IPlayer firstPlayer = new RandomPlayer ();
		firstPlayer.Init (name);

		IPlayer secondPlayer = new RandomPlayer ();
		secondPlayer.Init ("Random player");

		if (index != 0) {
			IPlayer temp = firstPlayer;
			firstPlayer = secondPlayer;
			secondPlayer = temp;
		}

		_players = new Dictionary<CellValue, IPlayer>() { 
			{CellValue.FirstPlayer, firstPlayer},
			{CellValue.SecondPlayer, secondPlayer }
            };

		_boardController.Init (20, 20);
	}

#endregion
}
