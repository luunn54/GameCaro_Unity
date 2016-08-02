/// <summary>
/// UnuGames - UI System
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
/// @This plugin is completely free for all purpose, please do not remove the author's information
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UIGianty;
using UnityEngine.SceneManagement;

namespace UIGianty
{
	[StartupAttribute(StartupType.PREFAB, null)]
	public class UIMan : SystemSingleton<UIMan>
	{

		// Configuration
		UIManConfig config;

		// Caches
		Dictionary<UIContentType, UIManScreen> screenDict = new Dictionary<UIContentType, UIManScreen> ();
		Dictionary<UIContentType, UIManDialog> dialogDict = new Dictionary<UIContentType, UIManDialog> ();

		// Transition queue
		List<UIManScreen> screenQueue = new List<UIManScreen> ();
		Queue<UIDialogQueueData> dialogQueue = new Queue<UIDialogQueueData> ();
		Queue<UIContentType> activeDialog = new Queue<UIContentType> ();

		// Assignable field
		public Transform uiRoot;
		public Transform screenRoot;
		public Transform dialogRoot;
		public Image background;
		Transform bgTrans;
		RectTransform bgRectTrans;
		public Transform cover;

		// Properties
		public bool IsInDialogTransition { get; set; }

		public bool IsLoadingDialog { get; set; }

		UIManScreen _mCurrentScreen;

		public UIManScreen CurrentScreen {
			get {
				return _mCurrentScreen;
			}
			set {
				_mCurrentScreen = value;
			}
		}

		UnityScene _mCurrentUnityScene;

		public UnityScene CurrentUnityScene {
			get {
				return _mCurrentUnityScene;
			}
			set {
				_mCurrentUnityScene = value;
			}
		}

		static UILoading _uiLoading;
		static public UILoading Loading {
			get {
				if (_uiLoading == null)
					_uiLoading = Instance.GetComponentInChildren<UILoading> ();
				return _uiLoading;
			}
		}

		// Initialize

		public override void Init () {
			_uiLoading = GetComponentInChildren<UILoading> ();
			config = Resources.Load<UIManConfig> ("UIManConfig");
			bgTrans = background.GetComponent<Transform> ();
			bgRectTrans = background.GetComponent<RectTransform> ();
		}

	#region Layer indexer

		/// <summary>
		/// Brings to front.
		/// </summary>
		/// <param name="root">Root.</param>
		/// <param name="ui">User interface.</param>
		/// <param name="step">Step.</param>
		static private void BringToFront (Transform root, Transform ui, int step)
		{
			int uiCount = root.transform.childCount;
			ui.SetSiblingIndex (uiCount + step);
		}

		/// <summary>
		/// Brings to layer.
		/// </summary>
		/// <param name="root">Root.</param>
		/// <param name="ui">User interface.</param>
		/// <param name="step">Step.</param>
		static private void BringToLayer (Transform root, Transform ui, int layer)
		{
			ui.SetSiblingIndex (layer);
		}

		/// <summary>
		/// Sends to back.
		/// </summary>
		/// <param name="root">Root.</param>
		/// <param name="ui">User interface.</param>
		static private void SendToBack (Transform root, Transform ui)
		{
			ui.SetSiblingIndex (0);
		}

	#endregion

	#region Features

		/// <summary>
		/// 
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="seal">If set to <c>true</c> seal.</param>
		/// <param name="args">Arguments.</param>
		public void ShowScreen (UIContentType content, bool seal, params object[] args)
		{
			if (CurrentScreen != null && CurrentScreen.ContentType == content)
				return;

			if (CurrentScreen != null && CurrentScreen.State != UIState.BUSY && CurrentScreen.State != UIState.HIDE)
				CurrentScreen.HideMe ();

			UIManScreen screen = null;
			if (!screenDict.TryGetValue (content, out screen)) {
				ResourceFactory.LoadAsync<GameObject> (config.screenPrefabFolder + content.ToName (), PreprocessUI, content, seal, args);
				return;
			}

			if (screen.useBackground) {
				background.gameObject.SetActive (true);
				string bgName = config.backgroundRootFolder + screen.backgroundType.ToString ();
				ResourceFactory.LoadAsync<Texture2D> (bgName, SetScreenBackground);

				BringToFront (screenRoot, bgTrans, 1);
			}

			BringToFront (screenRoot, screen.transform, 2);

			screen.OnShow (args);
			OnShowUI (content, args);
			DoAnimShow (screen);

			CurrentScreen = screen;
			if (!seal)
				screenQueue.Add (screen);
		}

		/// <summary>
		/// Shows the screen.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="args">Arguments.</param>
		public void ShowScreen (UIContentType content, params object[] args)
		{
			ShowScreen (content, false, args);
		}

		/// <summary>
		/// Backs the screen.
		/// </summary>
		/// <param name="args">Arguments.</param>
		public void BackScreen (params object[] args)
		{
			if (screenQueue.Count <= 1) {
				Logger.LogWarning ("UI Error: There are no scene has been loaded before this scene!");
				return;
			}
        
			CurrentScreen.HideMe ();
			UIManScreen beforeScreen = screenQueue [screenQueue.Count - 2];

			OnBack (CurrentScreen.ContentType, beforeScreen.ContentType, args);

			screenQueue.RemoveAt (screenQueue.Count - 1);
			ShowScreen (beforeScreen.ContentType, true, args);
		}

		/// <summary>
		/// Hides the screen.
		/// </summary>
		/// <param name="content">Content.</param>
		public void HideScreen (UIContentType content)
		{
			UIManScreen screen = null;
			if (screenDict.TryGetValue (content, out screen)) {
				screen.OnHide ();
				OnHideUI (content);
				DoAnimHide (screen);
			} else {
				Logger.LogFormatWarning ("There are no UI of {0} has been show!", content.ToString ());
				return;
			}
		}

		/// <summary>
		/// Shows the dialog.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="callbacks">Callbacks.</param>
		/// <param name="args">Arguments.</param>
		public void ShowDialog (UIContentType content, UICallback callbacks, params object[] args)
		{
			if (IsInDialogTransition || IsLoadingDialog) {
				EnqueueDialog (content, UITransitionType.SHOW, args, callbacks);
				return;
			}

			UIManDialog dialog = null;
			if (!dialogDict.TryGetValue (content, out dialog)) {
				IsLoadingDialog = true;
				ResourceFactory.LoadAsync<GameObject> (config.dialogPrefabFolder + content.ToName (), PreprocessUI, content, callbacks, args);
				return;
			}

			if (dialog.IsActive)
				return;

			if (dialog.useCover) {
				cover.gameObject.SetActive (true);
				BringToFront (dialogRoot, cover, 1);
			}

			BringToFront (dialogRoot, dialog.transform, 2);
			activeDialog.Enqueue (content);
			IsInDialogTransition = true;
			dialog.SetCallbacks (callbacks);
			dialog.OnShow (args);
			OnShowUI (content, args);
			DoAnimShow (dialog);
		}

		/// <summary>
		/// Shows the dialog.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="args">Arguments.</param>
		public void ShowDialog (UIContentType content, params object[] args)
		{
			ShowDialog (content, null, args);
		}

		/// <summary>
		/// Shows the message dialog.
		/// </summary>
		/// <param name="Title">Title.</param>
		/// <param name="Content">Content.</param>
		/// <param name="callbacks">Callbacks.</param>
		public void ShowMessageDialog (string title, string message, System.Action<object[]> onOK = null, object[] callbackArgs = null)
		{
			UICallback uiCallbacks = new UICallback (onOK);
			ShowDialog (UIContentType.MESSAGE_DIALOG, uiCallbacks, title, message, callbackArgs); 
		}

		/// <summary>
		/// Shows the confirm dialog.
		/// </summary>
		/// <param name="Title">Title.</param>
		/// <param name="Content">Content.</param>
		/// <param name="Button1">Button1.</param>
		/// <param name="Button2">Button2.</param>
		/// <param name="callbacks">Callbacks.</param>
		public void ShowConfirmDialog (string title, string message, string textButton1 = "Yes", string textButton2 = "No",
	                              System.Action<object[]> onButton1Click = null, System.Action<object[]> onButton2Click = null,
	                              object[] callbackArgs1 = null, object[] callbackArgs2 = null)
		{
			UICallback uiCallbacks = new UICallback (onButton1Click, onButton2Click);
			ShowDialog (UIContentType.CONFIRM_DIALOG, uiCallbacks, title, message, textButton1, textButton2, callbackArgs1, callbackArgs2); 
		}

		/// <summary>
		/// Hides the dialog.
		/// </summary>
		/// <param name="content">Content.</param>
		public void HideDialog (UIContentType content)
		{
			if (IsInDialogTransition) {
				EnqueueDialog (content, UITransitionType.HIDE, null, null);
				return;
			}
			UIManDialog dialog = null;
			if (dialogDict.TryGetValue (content, out dialog)) {
				if (activeDialog.Count > 0)
					activeDialog.Dequeue ();
				if (dialog.useCover) {
					UIManDialog prevDialog = null;
					if (dialogQueue.Count > 0)
						dialogDict.TryGetValue (dialogQueue.Peek ().Content, out prevDialog);
					if (activeDialog.Count > 0 && prevDialog != null && prevDialog.useCover) {
						BringToLayer (dialogRoot, cover, cover.GetSiblingIndex () - 1);
					} else {
						cover.gameObject.SetActive (false);
					}
				}
				IsInDialogTransition = true;
				dialog.OnHide ();
				OnHideUI (content);
				DoAnimHide (dialog);
			} else {
				Logger.LogFormatWarning ("There are no UI of {0} has been show!", content.ToString ());
				return;
			}
		}

		/// <summary>
		/// Loads the unity scene.
		/// </summary>
		/// <param name="scene">Scene.</param>
		public void LoadUnityScene (UnityScene scene, UIContentType screen, bool showLoading, params object[] args)
		{
			Instance.activeDialog.Clear ();
			Instance.CurrentUnityScene = scene;
			if (Instance.CurrentScreen != null)
				Instance.CurrentScreen.HideMe ();
			Instance.LoadUnityScene (scene.ToName (), screen, showLoading, args);
		}

		/// <summary>
		/// Loads the unity scene.
		/// </summary>
		/// <param name="name">Name.</param>
		void LoadUnityScene (string name, UIContentType screen, bool showLoading, params object[] args)
		{
			Instance.cover.gameObject.SetActive (false);
			if (showLoading)
				Loading.Show (SceneManager.LoadSceneAsync (name), true, "", OnLoadUnitySceneComplete, screen, args);
			else
				StartCoroutine (LoadUnityScene (name, screen, args));
		}

		/// <summary>
		/// Loads the unity scene.
		/// </summary>
		/// <returns>The unity scene.</returns>
		/// <param name="name">Name.</param>
		/// <param name="screen">Screen.</param>
		/// <param name="args">Arguments.</param>
		IEnumerator LoadUnityScene (string name, UIContentType screen, params object[] args)
		{
			yield return SceneManager.LoadSceneAsync (name);
			OnLoadUnitySceneComplete (screen, args);
		}

		/// <summary>
		/// Raises the load unity scene complete event.
		/// </summary>
		/// <param name="args">Arguments.</param>
		void OnLoadUnitySceneComplete (params object[] args)
		{
			StartCoroutine (WaitForTransitionComplete (args));
		}

		IEnumerator WaitForTransitionComplete (params object[] args)
		{
			while (CurrentScreen != null && CurrentScreen.State != UIState.HIDE) {
				yield return null;
			}
			UIContentType screen = (UIContentType)args [0];
			object[] screenArgs = null;
			if (args.Length > 1)
				screenArgs = (object[])args [1];
			Instance.ShowScreen (screen, screenArgs);
		}

		/// <summary>
		/// Sets the native loading.
		/// </summary>
		/// <param name="isLoading">If set to <c>true</c> is loading.</param>
		static public void SetNativeLoading (bool isLoading)
		{
#if UNITY_IOS || UNITY_ANDROID
		if(isLoading)
		Handheld.StartActivityIndicator();
		else
		Handheld.StopActivityIndicator();
#endif
		}

		/// <summary>
		/// Registers the on back.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void RegisterOnBack (System.Action<UIContentType, UIContentType, object[]> callback)
		{
			UIEventDispatcher.AddEventListener<UIContentType, UIContentType, object[]> (UIManEvents.UIMan.OnBack, callback);
		}

		/// <summary>
		/// Registers the on show U.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void RegisterOnShowUI (System.Action<UIContentType, object[]> callback)
		{
			UIEventDispatcher.AddEventListener<UIContentType, object[]> (UIManEvents.UIMan.OnShowUI, callback);
		}

		/// <summary>
		/// Registers the on show user interface complete.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void RegisterOnShowUIComplete (System.Action<UIContentType, object[]> callback)
		{
			UIEventDispatcher.AddEventListener<UIContentType, object[]> (UIManEvents.UIMan.OnShowUIComplete, callback);
		}

		/// <summary>
		/// Registers the on hide U.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void RegisterOnHideUI (System.Action<UIContentType> callback)
		{
			UIEventDispatcher.AddEventListener<UIContentType> (UIManEvents.UIMan.OnHideUI, callback);
		}

		/// <summary>
		/// Registers the on hide user interface complete.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void RegisterOnHideUIComplete (System.Action<UIContentType> callback)
		{
			UIEventDispatcher.AddEventListener<UIContentType> (UIManEvents.UIMan.OnHideUIComplete, callback);
		}

	#endregion

	#region Events
	
		/// <summary>
		/// Raises the back event.
		/// </summary>
		/// <param name="before">Before.</param>
		/// <param name="after">After.</param>
		/// <param name="args">Arguments.</param>
		void OnBack (UIContentType before, UIContentType after, params object[] args)
		{
			UIEventDispatcher.TriggerEvent<UIContentType, UIContentType, object[]> (UIManEvents.UIMan.OnBack, before, after, args);
		}

		/// <summary>
		/// Raises the show UI event.
		/// </summary>
		/// <param name="dialog">Dialog.</param>
		/// <param name="args">Arguments.</param>
		void OnShowUI (UIContentType ui, params object[] args)
		{
			UIEventDispatcher.TriggerEvent<UIContentType, object[]> (UIManEvents.UIMan.OnShowUI, ui, args);
		}
	
		/// <summary>
		/// Raises the show user interface complete event.
		/// </summary>
		/// <param name="ui">User interface.</param>
		/// <param name="args">Arguments.</param>
		void OnShowUIComplete (UIContentType ui, params object[] args)
		{
			UIEventDispatcher.TriggerEvent<UIContentType, object[]> (UIManEvents.UIMan.OnShowUIComplete, ui, args);
		}

		/// <summary>
		/// Raises the hide U event.
		/// </summary>
		/// <param name="ui">User interface.</param>
		void OnHideUI (UIContentType ui)
		{
			UIEventDispatcher.TriggerEvent<UIContentType> (UIManEvents.UIMan.OnHideUI, ui);
		}

		/// <summary>
		/// Raises the hide user interface complete event.
		/// </summary>
		/// <param name="ui">User interface.</param>
		void OnHideUIComplete (UIContentType ui)
		{
			UIEventDispatcher.TriggerEvent<UIContentType> (UIManEvents.UIMan.OnHideUIComplete, ui);
		}

	#endregion

	#region Utils

		/// <summary>
		/// Preprocesses the U.
		/// </summary>
		/// <param name="prefab">Prefab.</param>
		/// <param name="args">Arguments.</param>
		void PreprocessUI (GameObject prefab, object[] args)
		{
			UIContentType content = (UIContentType)args [0];
			if (prefab == null) {
				Logger.LogFormatWarning ("UI Error: cannot find {0}, make sure you have put UI prefab in Resources folder!", content.ToString ());
				return;
			}
			GameObject uiObj = Instantiate (prefab) as GameObject;
			uiObj.name = content.ToString ();
			UIManBase uiBase = uiObj.GetComponent<UIManBase> ();
			if (uiBase is UIManScreen) {
				uiBase.Trans.SetParent (screenRoot, false);
				uiBase.RectTrans.localScale = Vector3.one;
				screenDict.Add (content, uiBase as UIManScreen);
				bool seal = (bool)args [1];
				object[] param = (object[])args [2];
				ShowScreen (content, seal, param);
			} else if (uiBase is UIManDialog) {
				uiBase.Trans.SetParent (dialogRoot, false);
				uiBase.RectTrans.localScale = Vector3.one;
				dialogDict.Add (content, uiBase as UIManDialog);
				UICallback callbacks = (UICallback)args [1];
				object[] param = (object[])args [2];
				IsLoadingDialog = false;
				ShowDialog (content, callbacks, param);
			}
		}

		/// <summary>
		/// Sets the screen background.
		/// </summary>
		/// <param name="texture">Texture.</param>
		void SetScreenBackground (Texture2D texture, object[] args)
		{
			background.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);
			LeanTween.alpha (bgRectTrans, 1, 0.25f).setEase (LeanTweenType.linear);
		}

		/// <summary>
		/// Dos the animation show.
		/// </summary>
		/// <param name="ui">User interface.</param>
		void DoAnimShow (UIManBase ui)
		{
			ui.LockInput ();
			if (ui.motionShow == UIMotion.CUSTOM_MECANIM_ANIMATION) { //Custom animation use animator
				ui.animRoot.EnableAndPlay (UIManDefine.ANIM_SHOW);
			} else if (ui.motionShow == UIMotion.CUSTOM_SCRIPT_ANIMATION) { //Custom animation use overrided function
				StartCoroutine (DelayDequeueDialog (ui.AnimationShow (), ui, true));
			} else { // Simple tween
				Vector3 initPos = GetTargetPosition (ui.motionShow, UIManDefine.ARR_SHOW_TARGET_POS);
			
				ui.RectTrans.localPosition = initPos;
				ui.GroupCanvas.alpha = 0;
			
				// Tween position
				if (ui.motionShow != UIMotion.NONE) {
					LeanTween.move (ui.RectTrans, ui.showPosition, ui.animTime)
					.setEase (LeanTweenType.linear);
				}
			
				// Tween alpha
				LeanTween.value (ui.gameObject, ui.UpdateAlpha, 0.0f, 1.0f, ui.animTime)
				.setEase (LeanTweenType.linear)
					.setOnComplete (show => {
					ui.OnShowComplete ();
					OnShowUIComplete (ui.ContentType);
					if (ui.GetUIType () == UIBaseType.DIALOG) {
						IsInDialogTransition = false;
					}
					ui.UnlockInput ();
					DoAnimIdle (ui);
				});
			}
		}

		/// <summary>
		/// Dos the animation hide.
		/// </summary>
		/// <param name="ui">User interface.</param>
		void DoAnimHide (UIManBase ui)
		{
			ui.LockInput ();
			if (ui.motionHide == UIMotion.CUSTOM_MECANIM_ANIMATION) { //Custom animation use animator
				ui.animRoot.EnableAndPlay (UIManDefine.ANIM_HIDE);
			} else if (ui.motionHide == UIMotion.CUSTOM_SCRIPT_ANIMATION) { //Custom animation use overrided function
				StartCoroutine (DelayDequeueDialog (ui.AnimationHide (), ui, false));
			} else { // Simple tween

				// Stop current tween if exist
				LeanTween.cancel (ui.gameObject);
				LeanTween.cancel (bgRectTrans.gameObject);

				Vector3 hidePos = GetTargetPosition (ui.motionHide, UIManDefine.ARR_HIDE_TARGET_POS);
			
				// Tween position
				if (ui.motionHide != UIMotion.NONE) {
					LeanTween.move (ui.RectTrans, hidePos, ui.animTime).setEase (LeanTweenType.linear);
				}
			
				// Tween alpha
				LeanTween.value (ui.gameObject, ui.UpdateAlpha, 1.0f, 0.0f, ui.animTime)
				.setEase (LeanTweenType.linear)
					.setOnComplete (hide => {
					ui.RectTrans.anchoredPosition3D = hidePos;
					ui.OnHideComplete ();
					OnHideUIComplete (ui.ContentType);
					if (ui.GetUIType () == UIBaseType.DIALOG) {
						IsInDialogTransition = false;
						DequeueDialog ();
					}
				});
			
				LeanTween.alpha (bgRectTrans, 0, 0.25f).setEase (LeanTweenType.linear);
			}
		}

		/// <summary>
		/// Dos the animation idle.
		/// </summary>
		/// <param name="ui">User interface.</param>
		public void DoAnimIdle (UIManBase ui)
		{
			if (ui.motionIdle == UIMotion.CUSTOM_MECANIM_ANIMATION) { //Custom animation use animator
				ui.animRoot.EnableAndPlay (UIManDefine.ANIM_IDLE);
			} else if (ui.motionHide == UIMotion.CUSTOM_SCRIPT_ANIMATION) { //Custom animation use overrided function
				StartCoroutine (DelayDequeueDialog (ui.AnimationIdle (), ui, false));
			} else { // Simple tween
				//UnuLogger.Log("UIMan does not support simple tween animation for idle yet!");
			}
		}

		/// <summary>
		/// Gets the target position.
		/// </summary>
		/// <returns>The target position.</returns>
		/// <param name="motion">Motion.</param>
		/// <param name="arrTargetPosition">Arr target position.</param>
		Vector3 GetTargetPosition (UIMotion motion, Vector3[] arrTargetPosition)
		{
			return arrTargetPosition [(int)motion];
		}

		/// <summary>
		/// Enqueues the dialog.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="transition">Transition.</param>
		/// <param name="args">Arguments.</param>
		/// <param name="callback">Callback.</param>
		public void EnqueueDialog (UIContentType content, UITransitionType transition, object[] args, UICallback callback)
		{
			UIDialogQueueData data = new UIDialogQueueData (content, transition, args, callback);
			dialogQueue.Enqueue (data);
		}

		/// <summary>
		/// Delaies the dequeue dialog.
		/// </summary>
		/// <returns>The dequeue dialog.</returns>
		/// <param name="coroutine">Coroutine.</param>
		/// <param name="ui">User interface.</param>
		/// <param name="resetDialogTransitionStatus">If set to <c>true</c> reset dialog transition status.</param>
		public IEnumerator DelayDequeueDialog (IEnumerator coroutine, UIManBase ui, bool resetDialogTransitionStatus)
		{
			yield return StartCoroutine (coroutine);
			IsInDialogTransition = false;
			ui.UnlockInput ();
			ui.OnHideComplete ();
			if (ui.GetUIType () == UIBaseType.DIALOG && !resetDialogTransitionStatus)
				DequeueDialog ();
		}

		/// <summary>
		/// Dequeues the dialog.
		/// </summary>
		public void DequeueDialog ()
		{
			if (dialogQueue.Count > 0) {
				UIDialogQueueData transition = dialogQueue.Dequeue ();
				if (transition.TransitionType == UITransitionType.SHOW) {
					ShowDialog (transition.Content, transition.Callbacks, transition.Args);
				} else if (transition.TransitionType == UITransitionType.HIDE) {
					HideDialog (transition.Content);					
				}
			}
		}

		public bool IsShowingDialog (UIContentType type)
		{
			if (dialogQueue.Count > 0)
				return (dialogQueue.Peek ().Content == type);
			return false;
		}

		public void DestroyUI (UIContentType type, bool dialog)
		{
			UIManBase ui = null;
			if (dialog) {
				if (dialogDict.ContainsKey (type)) {
					ui = dialogDict [type];
					dialogDict.Remove (type);
				}
			} else {
				if (screenDict.ContainsKey (type)) {
					ui = screenDict [type];
					screenDict.Remove (type);
				}
			}

			if (ui != null) {
				Destroy (ui.gameObject);
			}
		}

	#endregion
	}
}
