using UnityEngine;
using System.Collections;
using UIGianty;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UIMan.Instance.ShowScreen (UIContentType.SCREEN_HOME);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
