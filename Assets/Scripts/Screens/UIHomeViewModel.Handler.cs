﻿using UnityEngine;
using UIGianty;
using UIGianty.MVVM;

public partial class UIHomeViewModel : UIManScreen {

#region Fields

	// Your fields here
#endregion

#region Built-in Events
	public delegate void LogAA(string s);
	public LogAA logger;
	public override void OnShow (params object[] args)
	{
		logger ("SS");
		logger += sss;
		base.OnShow (args);
	}

	public void sss(string s){
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
	public void OnClickHelp(){
		UIMan.Instance.ShowScreen (UIContentType.SCREEN_HELP);
	}

	public void OnClickPlay(){
		UIMan.Instance.ShowScreen (UIContentType.SCREEN_PLAY_GAME);
	}

	public void OnClickSetting(){
		UIMan.Instance.ShowScreen (UIContentType.SCREEN_SETTING);
	}
	// Your custom code here
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
