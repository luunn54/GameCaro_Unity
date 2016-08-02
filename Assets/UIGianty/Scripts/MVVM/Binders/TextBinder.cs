using UnityEngine;
using UnityEngine.UI;

namespace UIGianty.MVVM
{
	[RequireComponent(typeof(Text))]
	[DisallowMultipleComponent]
	public class TextBinder : BinderBase
	{

		protected Text text;
		[HideInInspector]
		public BindingField
			textValue = new BindingField ("Text");
		[HideInInspector]
		public BindingField
			textColor = new BindingField ("Color", true);
		public string Format;

		public override void Init ()
		{
			if (!Application.isPlaying)
				return;
			if (isInit)
				return;
			isInit = true;

			text = GetComponent<Text> ();

			SubscribeOnChangedEvent (textValue, OnUpdateText);
			SubscribeOnChangedEvent (textColor, OnUpdateColor);
		}

		public void OnUpdateText (object newText)
		{
			if (newText == null)
				return;

			if (string.IsNullOrEmpty (Format)) {
				text.text = newText.ToString ();
			} else {
				text.text = string.Format (Format, newText.ToString ());
			}
		}

		public void OnUpdateColor (object newColor)
		{
			if (newColor == null)
				return;
			try {
				text.color = (Color)newColor;
			} catch {
				Logger.LogWarning ("Binding field is not a color!");
			}
		}

		public override void OnDisable ()
		{
			//UnSubscribeOnChangedEvent(textValue, OnUpdateText);
		}
	}
}