﻿using UnityEngine;
using System;

namespace UIGianty
{
	public class UIManDefine
	{
		static public Vector3[] ARR_HIDE_TARGET_POS = new Vector3[6]{
		Vector3.zero, //Center
		new Vector3 (2000, 2000, 0), //Hidden pos
		new Vector3 (-2000, 0, 0), //Left
		new Vector3 (2000, 0, 0), //Right
		new Vector3 (0, 1000, 0), //Top
		new Vector3 (0, -1000, 0) //Bottom
	};
		static public Vector3[] ARR_SHOW_TARGET_POS = new Vector3[6]{
		Vector3.zero, //Center
		new Vector3 (2000, 2000, 0), //Hidden pos
		new Vector3 (2000, 0, 0), //Right
		new Vector3 (-2000, 0, 0), //Left
		new Vector3 (0, -1000, 0), //Bottom
		new Vector3 (0, 1000, 0) //Top
	};
		public const string ANIM_SHOW = "Show";
		public const string ANIM_HIDE = "Hide";
		public const string ANIM_IDLE = "Idle";
	}

	public enum UITransitionType
	{
		SHOW,
		HIDE
	}

	public struct UIDialogQueueData
	{
		public UIContentType Content { get; set; }

		public object[] Args { get; set; }

		public UICallback Callbacks { get; set; }

		public UITransitionType TransitionType { get; set; }

		public UIDialogQueueData (UIContentType content, UITransitionType transition, object[] args, UICallback callbacks = null)
		{
			Content = content;
			TransitionType = transition;
			Args = args;
			Callbacks = callbacks;
		}
	}

	public class UIManProperty : Attribute
	{
		public UIManProperty ()
		{

		}

	}
}