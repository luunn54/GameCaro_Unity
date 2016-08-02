using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UIGianty.MVVM
{
	public class VisibleBinder : BinderBase
	{
		public List<GameObject> listTarget = new List<GameObject> ();
		public List<GameObject> listIntarget = new List<GameObject> ();
		[HideInInspector]
		public BindingField
			Value = new BindingField ("bool");

		public override void Init ()
		{
			CheckInit ();

			SubscribeOnChangedEvent (Value, OnUpdateValue);
		}

		public void OnUpdateValue (object val)
		{
			if (val == null)
				return;

			bool valChange = (bool)val;

			if (listTarget != null && listTarget.Count > 0) {
				for (int i = 0; i < listTarget.Count; i++) {
					listTarget [i].SetActive (valChange);
				}
			}

			if (listIntarget != null && listIntarget.Count > 0) {
				for (int i = 0; i < listIntarget.Count; i++) {
					listIntarget [i].SetActive (!valChange);
				}
			}
		}

		public override void OnDisable ()
		{
			UnSubscribeOnChangedEvent (Value, OnUpdateValue);
		}
	}
}