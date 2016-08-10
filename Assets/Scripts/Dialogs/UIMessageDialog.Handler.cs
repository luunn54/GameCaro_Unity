using UnityEngine;
using UIGianty.MVVM;
using UIGianty;

public partial class UIMessageDialog : UIManDialog {

	object[] _args;
	public override void OnShow(params object[] args)
	{
		base.OnShow(args);
		
		if(args != null && args.Length >= 2)
		{
			Title = (string)args[0];
			Content = (string)args[1];
			if(args.Length > 2)
				_args = (object[])args;
		}
	}
	
	public void OK ()
	{
		this.Callback(0, _args);
	}
}
