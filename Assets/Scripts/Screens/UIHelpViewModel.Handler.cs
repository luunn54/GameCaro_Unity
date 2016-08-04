using UnityEngine;
using UIGianty;
using UIGianty.MVVM;

public partial class UIHelpViewModel : UIManScreen {

#region Fields

	// Your fields here
#endregion

#region Built-in Events
	public override void OnShow (params object[] args)
	{
		base.OnShow (args);
		ScrollOffset = 1;
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
	public void OnBack(){
		UIMan.Instance.BackScreen ();
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
