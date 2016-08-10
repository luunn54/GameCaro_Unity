using UIGianty;
using UIGianty.MVVM;

// This code is generated automatically by UIMan ViewModelGerenrator, please do not modify!


public partial class UIMessageDialog : UIManDialog {

	string _title = "";
	[UIManProperty]
	public string Title {
		get { return _title; }
		set { _title = value; OnPropertyChanged(); }
	}

	string _content = "";
	[UIManProperty]
	public string Content {
		get { return _content; }
		set { _content = value; OnPropertyChanged(); }
	}

}
