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
		Action<object[]> onDoneInput = (object[] outputs)=>{
			int index = (int)outputs[0];
			string name = outputs[1] as string;

			StartPlay(index, name);
		};
		UIMan.Instance.ShowDialog (UIContentType.DIALOG_INPUTNAME, new UICallback(onDoneInput));
	}

	private IEnumerator AddCell(Turn turn){
		Debug.LogError(string.Format("{0}:{1} {2}", turn.CellValue, turn.Cell.X, turn.Cell.Y));
		var cell = new UnityEngine.GameObject();
		var img = cell.AddComponent<Image> ();
		var sprite = Resources.Load<Sprite> ("Imgs/cell_x");
		yield return null;
	}

#endregion

#region Board Hander

	void BoardDataFinished (IBoardData boardData)
	{
		if (boardData.IsFinished) {
//			UIMan.Instance.ShowMessageDialog (string.Empty, "Finish");
			Debug.LogError ("Finished..");
		}
	}

	void BoardNextedTurn (IBoardData boardData, Turn turn)
	{
		Debug.LogError ("Next..");

		var playerType = boardData.NextPlayer();
//		PlayerTurn (playerType, turn);
		StartCoroutine(DelayAndPlay(playerType, turn));
	}

	void BoardBackedTurn (IBoardData boardData, Turn turn)
	{
		Debug.LogError ("Back..");
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

//		var cell = new UnityEngine.GameObject();
//		var img = cell.AddComponent<Image> ();
//		var x2 = cell.transform as RectTransform;
//
//		cell.transform.SetParent (_boardObjec
//		//x2.position = new Vector3 (34 * 3, 34 * 5);
//
//		x2.localPosition = new Vector3 (34 * 3, 34 * 5);
//		x2.sizeDelta = new Vector2 (34 * 2, 34 * 2);
//		Debug.LogError (x2.anchoredPosition);
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
