using UnityEngine;
using UnityEngine.UI;
using UIGianty;
using UIGianty.MVVM;
using System;
using CaroGame.Models;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public partial class UIPlayGameViewModel : UIManScreen {

#region Fields
	public GameObject _boardObject;
#endregion

#region Built-in Events
	public override void OnShow (params object[] args)
	{
		ShowInputName ();
		base.OnShow (args);
	}

	public override void OnShowComplete ()
	{
		base.OnShowComplete ();
	}

	public override void OnHide ()
	{
		base.OnHide ();
	}

	public override void OnHideComplete ()
	{
		base.OnHideComplete ();
	}
#endregion

#region Custom implementation

	private void ShowInputName(){
		ClearBoard ();

		Action<object[]> onDoneInput = (object[] outputs)=>{
			int index = (int)outputs[0];
			string name = outputs[1] as string;

			StartPlay(index, name);
		};
		UIMan.Instance.ShowDialog (UIContentType.DIALOG_INPUTNAME, new UICallback(onDoneInput));
	}

	private void ClearBoard(){
		foreach (Transform child in _boardObject.transform) {
			GameObject.Destroy(child.gameObject);
		}
	}

	public void OnBack(){
		UIMan.Instance.BackScreen ();
	}

#endregion

#region Board Hander

	void BoardDataFinished (IBoardData boardData)
	{
		if (boardData.IsFinished) {
			UIMan.Instance.ShowMessageDialog (string.Empty, _players[boardData.PlayerWin].Name + " Win!");
		}
	}

	void BoardTurnChanged (IBoardData boardData, Turn? lastTurn)
	{
		var playerType = boardData.NextPlayer();
		StartCoroutine(DelayAndPlay(playerType, lastTurn));
	}

	void BoardNextedTurn (IBoardData boardData, Turn turn)
	{
		UpdateBoard (turn);
	}

	void BoardBackedTurn (IBoardData boardData, Turn turn)
	{
		Debug.LogError ("Back..");
		UpdateBoard (turn);
	}

	private IEnumerator DelayAndPlay(CellValue playerType, Turn? lastTurn){
		yield return null;
		PlayerTurn (playerType, lastTurn);
	}

	private void PlayerTurn(CellValue playerType, Turn? lastTurn){
		Cell cell = _players [playerType].NextTurn (lastTurn);
		bool success = _boardController.NextTurn (playerType, cell);
		if (!success) {
			// process exception
			Debug.LogError("error " + playerType);
		}
	}

	private void BoardInited(IBoardData boardData){
		foreach (KeyValuePair<CellValue, IPlayer> entry in _players)
            {
				entry.Value.BeginGame(boardData, entry.Key);
            }

		PlayerFirstName = _players[CellValue.FirstPlayer].Name;
		PlayerSecondName = _players [CellValue.SecondPlayer].Name;

		PlayerTurn (CellValue.FirstPlayer, null);
	}
#endregion

#region Board Update UI

	private void UpdateBoard(Turn turn){
		string imageName = null;
		switch (turn.CellValue) {
			case CellValue.FirstPlayer:
				imageName = "cell_x";
				break;
			case CellValue.SecondPlayer:
				imageName = "cell_o";
				break;
			default:
				imageName = null;
				break;
		}

		if (imageName == null) {
			// remove game object
		} else {
			// add new game object
			var cell = new UnityEngine.GameObject ();
			var img = cell.AddComponent<Image> ();

			var pathImg = "Imgs/" + imageName;
			var sprite = Resources.Load<Sprite> (pathImg);
			img.sprite = sprite;

			cell.transform.SetParent (_boardObject.transform, false);
			RectTransform recttransform = cell.transform as RectTransform;
			recttransform.pivot = Vector2.zero;

			recttransform.sizeDelta = new Vector2 (34, 34);
			recttransform.localPosition = new Vector3 (34 * turn.Cell.X, 34 * turn.Cell.Y, 0);
		}
	}
#endregion

#region Override animations
	/* Uncommend this for override show/hide animation of Screen/Dialog use tweening code
	public override IEnumerator AnimationShow ()
	{
		return base.AnimationShow ();
	}

	public override IEnumerator AnimationHide ()
	{
		return base.AnimationHide ();
	}

	public override IEnumerator AnimationIdle ()
	{
		return base.AnimationHide ();
	}
	*/
#endregion
}
