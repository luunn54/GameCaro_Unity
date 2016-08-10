using UnityEngine;
using UIGianty;
using UIGianty.MVVM;
using UnityEngine.UI;

public partial class UIInputNameViewModel : UIManDialog {

#region Fields
	public InputField _textName;
	public Dropdown _dropdownPlayer;
#endregion

#region Built-in Events
	public override void OnShow (params object[] args)
	{
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
	public void OnClickOk(){
		string name = _textName.text;
		if (string.IsNullOrEmpty (name)) {
			UIMan.Instance.ShowMessageDialog (string.Empty, "Input Name", delegate {
			});
			return;
		}

		int player = _dropdownPlayer.value;
		this.Callback (0, player, name);
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
