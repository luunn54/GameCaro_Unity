using UnityEngine;
using UnityEngine.UI;

namespace UIGianty.MVVM
{
	[RequireComponent(typeof(Image))]
	[DisallowMultipleComponent]
	public class InternalImageBinder : BinderBase
	{

		protected Image image;
		[HideInInspector]
		public BindingField
			imageValue = new BindingField ("Image");
		[HideInInspector]
		public BindingField
			imageColor = new BindingField ("Color");
		private string resourcePath = "Images/";

		public override void Init ()
		{
			CheckInit ();

			image = GetComponent<Image> ();

			SubscribeOnChangedEvent (imageValue, OnUpdateImage);
			SubscribeOnChangedEvent (imageColor, OnUpdateColor);
		}

		public void OnUpdateImage (object newImage)
		{
			if (newImage == null)
				return;
			image.sprite = ResourceFactory.Load<Sprite> (resourcePath + newImage.ToString ());
		}

		public void OnUpdateColor (object newColor)
		{
			if (newColor == null)
				return;
			try {
				image.color = (Color)newColor;
			} catch {
				Logger.LogWarning ("Binding field is not a color!");
			}
		}

		public override void OnDisable ()
		{
			UnSubscribeOnChangedEvent (imageValue, OnUpdateImage);
		}
	}
}