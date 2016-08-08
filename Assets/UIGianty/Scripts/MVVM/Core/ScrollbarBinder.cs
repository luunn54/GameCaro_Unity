using UnityEngine;
using UnityEngine.UI;

namespace UIGianty.MVVM
{
	[RequireComponent(typeof(Scrollbar))]
	[DisallowMultipleComponent]
	public class ScrollbarBinder : BinderBase
	{

		protected Scrollbar scrollbar;
		[HideInInspector]
		public BindingField
		offsetValue = new BindingField ("Offset");

		public override void Init ()
		{
			if (!Application.isPlaying)
				return;
			if (isInit)
				return;
			isInit = true;

			scrollbar = GetComponent<Scrollbar> ();

			SubscribeOnChangedEvent (offsetValue, OnUpdateText);
		}

		public void OnUpdateText (object newText)
		{
			if (newText == null)
				return;

			scrollbar.value = (float)newText;
		}

		public override void OnDisable ()
		{
			//UnSubscribeOnChangedEvent(textValue, OnUpdateText);
		}
	}
}