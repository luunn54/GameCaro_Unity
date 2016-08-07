using UnityEngine;
using UnityEngine.UI;
using UIGianty;
using UIGianty.MVVM;
using System;
using CaroGame.Models;
using System.Threading;
using System.Collections;

    public class RandomPlayer : IPlayer
    {
        public static System.Random random = new System.Random();

		IGameController gameController;
		sbyte width, height;
		private string _name;
        public string Name
        {
            get
            {
				if (_name == null) {
					_name = string.Format ("Random {0}", random.Next (1000));
				}
				return _name;
            }
        }

        // Game flow
		public void BeginGame(IGameController gameController, sbyte widthBoard, sbyte heightBoard)
        {
            width = widthBoard;
            height = heightBoard;
            this.gameController = gameController;
        }
			
        public Cell NextTurn(Cell? lastCell)
        {
            int size = width * height;
            for (int i = 0; i < size; i++)
            {
                var XRandom = (sbyte)random.Next(width);
                var YRandom = (sbyte)random.Next(height);
                if (gameController.CellAt(XRandom, YRandom) == CellValue.None)
                {
                    return new Cell() { X = XRandom, Y = YRandom };
                }
            }

            Console.WriteLine("?? " + size);
            return default(Cell);
        }

        public void EndGame(bool isWin) { }
    }

public partial class UIPlayGameViewModel : UIManScreen {

#region Fields
	private GameController _gameController;
	private IBoardViewer _boardViewer;

	public GameObject _boardObject;
#endregion

#region Built-in Events
	public override void OnShow (params object[] args)
	{
		base.OnShow (args);
		ShowInputName ();
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

	private void StartPlay(int index, string name){
		_gameController = new GameController ();
		_boardViewer = _gameController.BoardViewer;
		_boardViewer.Ininted += BoardInited;
		_boardViewer.Finished += _boardViewer_Finished;
		_boardViewer.BackedTurn += _boardViewer_BackedTurn;
		_boardViewer.NextedTurn += _boardViewer_NextedTurn;

		_gameController.Init (new RandomPlayer (), new RandomPlayer (), 30, 30);
		ThreadPool.QueueUserWorkItem (delegate(object obj) {
			_gameController.Start();
		});

	//	_gameController.Start ();
	}

	void _boardViewer_Finished (IBoardViewer boardViewer)
	{
		Debug.LogError ("Finish " + boardViewer.PlayerWin);
	}

	void _boardViewer_NextedTurn (IBoardViewer boardViewer, Turn turn)
	{
		StartCoroutine (AddCell (turn));
	}

	private IEnumerator AddCell(Turn turn){
		Debug.LogError(string.Format("{0}:{1} {2}", turn.CellValue, turn.Cell.X, turn.Cell.Y));
		var cell = new UnityEngine.GameObject();
		var img = cell.AddComponent<Image> ();
		var sprite = Resources.Load<Sprite> ("Imgs/cell_x");
		yield return null;
	}

	void _boardViewer_BackedTurn (IBoardViewer boardViewer, Turn turn)
	{
		
	}

	private void BoardInited(IBoardViewer boardViewer){
		var cell = new UnityEngine.GameObject();
		var img = cell.AddComponent<Image> ();
		var x2 = cell.transform as RectTransform;

		cell.transform.SetParent (_boardObject.transform);
		//x2.position = new Vector3 (34 * 3, 34 * 5);

		x2.localPosition = new Vector3 (34 * 3, 34 * 5);
		x2.sizeDelta = new Vector2 (34 * 2, 34 * 2);
		Debug.LogError (x2.anchoredPosition);

		PlayerFirstName = boardViewer.FirstUser;
		PlayerSecondName = boardViewer.SecondUser;
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
