/// <summary>
/// UnuGames - UI System
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
/// @This plugin is completely free for all purpose, please do not remove the author's information
/// </summary>
using UnityEngine;
using System.Collections;

namespace UIGianty
{
	[DisallowMultipleComponent]
	public class UIManScreen : UIManBase
	{
		public bool useBackground = false;
		public UIScreenBG backgroundType = UIScreenBG.COMMON;

		public override UIBaseType GetUIType ()
		{
			return UIBaseType.SCREEN;
		}
	}
}
