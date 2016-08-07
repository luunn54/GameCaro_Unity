
using UIGianty;
using UIGianty.MVVM;

// This code is generated automatically by UIMan ViewModelGerenrator, please do not modify!

public partial class UIPlayGameViewModel : UIManScreen {


	string _playerFirstName = "";
	[UIManProperty]
	public string PlayerFirstName {
		get { return _playerFirstName; }
		set { _playerFirstName = value; OnPropertyChanged(); }
	}

	string _playerSecondName = "";
	[UIManProperty]
	public string PlayerSecondName {
		get { return _playerSecondName; }
		set { _playerSecondName = value; OnPropertyChanged(); }
	}

}
