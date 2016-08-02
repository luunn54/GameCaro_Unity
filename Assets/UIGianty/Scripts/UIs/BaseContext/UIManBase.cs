/// <summary>
/// UnuGames - UI System
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
/// @This plugin is completely free for all purpose, please do not remove the author's information
/// </summary>
using System;
using UnityEngine;
using System.Collections;
using UIGianty.MVVM;
using UIGianty;

namespace UIGianty
{
	[RequireComponent(typeof(CanvasGroup))]
	[Serializable]
	public class UIManBase : ViewModelBehaviour
	{
		[HideInInspector]
		public UIMotion
			motionShow;
		[HideInInspector]
		public UIMotion
			motionHide;
		[HideInInspector]
		public UIMotion
			motionIdle;
		[HideInInspector]
		public Animator
			animRoot;
		[HideInInspector]
		public Vector3
			showPosition = Vector3.zero;
		[HideInInspector]
		public float
			animTime = 0.25f;

		public UIContentType ContentType {
			get {
				return Utils.ParseEnum<UIContentType> (gameObject.name);
			}
		}

		public UIState State { get; private set; }

		private CanvasGroup mCanvasGroup;

		public CanvasGroup GroupCanvas {
			get {
				if (mCanvasGroup == null)
					mCanvasGroup = GetComponent<CanvasGroup> ();
				if (mCanvasGroup == null)
					mCanvasGroup = gameObject.AddComponent<CanvasGroup> ();
				return mCanvasGroup;
			}
		}

		private RectTransform mRectTrans;

		public RectTransform RectTrans {
			get {
				if (mRectTrans == null)
					mRectTrans = GetComponent<RectTransform> ();
				return mRectTrans;
			}
		}

		private Transform mTrans;

		public Transform Trans {
			get {
				if (mTrans == null)
					mTrans = GetComponent<Transform> ();
				return mTrans;
			}
		}

		private bool _isReady = false;

		public bool IsActive {
			get { return _isReady; }
			set { _isReady = value; }
		}

		/// <summary>
		/// Gets the type of the user interface.
		/// </summary>
		/// <returns>The user interface type.</returns>
		public virtual UIBaseType GetUIType ()
		{
			return default(UIBaseType);
		}
	

		/// <summary>
		/// Raises the show event.
		/// </summary>
		/// <param name="args">Arguments.</param>
		public virtual void OnShow (params object[] args)
		{
			if (GroupCanvas.alpha != 0 && (motionShow != UIMotion.CUSTOM_MECANIM_ANIMATION && motionShow != UIMotion.CUSTOM_SCRIPT_ANIMATION))
				GroupCanvas.alpha = 0;
			State = UIState.BUSY;
			IsActive = false;
		}

		/// <summary>
		/// Raises the hide event.
		/// </summary>
		public virtual void OnHide ()
		{
			if (GroupCanvas.alpha != 1 && motionHide != UIMotion.CUSTOM_MECANIM_ANIMATION && motionHide != UIMotion.CUSTOM_SCRIPT_ANIMATION)
				GroupCanvas.alpha = 1;
			State = UIState.BUSY;
			IsActive = false;
		}

		/// <summary>
		/// Raises the show complete event.
		/// </summary>
		public virtual void OnShowComplete ()
		{
			State = UIState.SHOW;
			IsActive = true;
		}

		/// <summary>
		/// Raises the hide complete event.
		/// </summary>
		public virtual void OnHideComplete ()
		{
			State = UIState.HIDE;
		}

		/// <summary>
		/// Updates the alpha.
		/// </summary>
		/// <param name="alpha">Alpha.</param>
		public void UpdateAlpha (float alpha)
		{
			GroupCanvas.alpha = alpha;
		}

		/// <summary>
		/// Internal function for hide current ui
		/// </summary>
		public void HideMe ()
		{
			UIContentType content = ContentType;
			if (GetUIType () == UIBaseType.SCREEN) {
				UIMan.Instance.HideScreen (content);
			} else {
				UIMan.Instance.HideDialog (content);
			}
		}

		/// <summary>
		/// Animations the show.
		/// </summary>
		public virtual IEnumerator AnimationShow ()
		{
			yield return null;
		}

		/// <summary>
		/// Animations the hide.
		/// </summary>
		public virtual IEnumerator AnimationHide ()
		{
			yield return null;
		}

		/// <summary>
		/// Animations the idle.
		/// </summary>
		public virtual IEnumerator AnimationIdle ()
		{
			yield return null;
		}
		
		/// <summary>
		/// Locks the input.
		/// </summary>
		public void LockInput ()
		{
			GroupCanvas.interactable = false;
			GroupCanvas.blocksRaycasts = false;
		}

		/// <summary>
		/// Unlocks the input.
		/// </summary>
		public void UnlockInput ()
		{
			GroupCanvas.interactable = true;
			GroupCanvas.blocksRaycasts = true;
		}
	}
}
