
using UIGianty;
using UIGianty.MVVM;

// This code is generated automatically by UIMan ViewModelGerenrator, please do not modify!

public partial class UIHelpViewModel : UIManScreen {


	float _scrollOffset = 0;
	[UIManProperty]
	public float ScrollOffset {
		get { return _scrollOffset; }
		set { _scrollOffset = value; OnPropertyChanged(); }
	}

}
