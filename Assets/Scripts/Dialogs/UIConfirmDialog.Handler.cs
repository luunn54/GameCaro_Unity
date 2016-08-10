using UnityEngine;
using UIGianty.MVVM;
using UIGianty;

public partial class UIConfirmDialog : UIManDialog {

	object[] args1;
	object[] args2;
	public override void OnShow(params object[] args)
	{
		base.OnShow(args);
		if (args != null && args.Length >= 4)
		{
			Title = (string)args[0];
			Content = (string)args[1];
			LabelButtonYes = (string)args[2];
			LabelButtonNo = (string)args[3];
			if(args.Length > 4)
				args1 = (object[])args[4];
			if(args.Length > 5)
				args2 = (object[])args[5];
		}
	}
	
	public void Button1()
	{
		this.Callback(0, args1);
	}
	
	public void Button2()
	{
		this.Callback(1, args2);
	}
}
