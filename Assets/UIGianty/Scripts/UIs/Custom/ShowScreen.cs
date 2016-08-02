using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIGianty;

public class ShowScreen : MonoBehaviour {

	public UIContentType screen;
	public bool showLoading = false;

	public void Show () {
		if(showLoading)
			UIMan.Loading.Show ();
		UIMan.Instance.ShowScreen (screen);
	}
}
