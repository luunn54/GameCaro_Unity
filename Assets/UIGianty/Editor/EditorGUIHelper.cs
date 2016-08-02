﻿using System;
using UnityEditor;
using UnityEngine;

namespace UIGianty
{
	public class UISearchField
	{
	
		string keyWord = "";

		public string KeyWord {
			get { return keyWord; }
			set { keyWord = value; }
		}
	
		string oldKeyWord = "";
		GUIStyle searchFieldStyle;
		GUIStyle searchCancelButtonStyle;
		GUIStyle toolbarStyle;
		GUIStyle toolbarButtonStyle;
		GUIStyle toolbarDropDownStyle;
		Action<string> _onKeyWordChange;
		Action<object> _onLeftButtonClick;
		string _leftButtonText = "Create";

		public UISearchField (Action<string> onKeyWordChange, Action<object> onLeftButtonClick, string leftButtonText = "Create")
		{
			_onKeyWordChange = onKeyWordChange;
			_onLeftButtonClick = onLeftButtonClick;

			searchFieldStyle = GUI.skin.FindStyle ("ToolbarSeachTextField");
			searchCancelButtonStyle = GUI.skin.FindStyle ("ToolbarSeachCancelButton");
			toolbarStyle = GUI.skin.FindStyle ("Toolbar");
			toolbarButtonStyle = GUI.skin.FindStyle ("toolbarbutton");
			toolbarDropDownStyle = GUI.skin.FindStyle ("ToolbarDropDown");
			_leftButtonText = leftButtonText;
		}
	
		public void Draw ()
		{
			GUILayout.BeginHorizontal (toolbarStyle);

			if (_onLeftButtonClick != null && !string.IsNullOrEmpty (_leftButtonText)) {
				if (GUILayout.Button (_leftButtonText, toolbarButtonStyle, GUILayout.Width (50))) {
					if (_onLeftButtonClick != null)
						_onLeftButtonClick (null);
				}
				GUILayout.Label ("", toolbarDropDownStyle, GUILayout.Width (6));
				GUILayout.Space (5);
			}

			oldKeyWord = KeyWord;
		
			KeyWord = GUILayout.TextField (KeyWord, searchFieldStyle);
		
			if (GUILayout.Button ("", searchCancelButtonStyle)) {
				KeyWord = "";
				GUI.FocusControl (null);
			}
		
			if (!oldKeyWord.Equals (KeyWord)) {
				if (_onKeyWordChange != null) {
					_onKeyWordChange (KeyWord);
				}
			}
		
			GUILayout.EndHorizontal ();
		}
	}

	public class ListView
	{

		GUIStyle menuItemStyle;
		string[] _items;
		bool _selectOnMouseHover;
		string _filter;
		Vector2 _scrollPosition;
		Rect _position;
		EditorWindow _window;

		public EditorWindow Window {
			get { return _window; }
		}

		Action<string> _onSelected;

		public string SelectedItem { get; set; }

		public ListView ()
		{
			menuItemStyle = GUI.skin.FindStyle ("MenuItem");
		}

		public void SetData (string[] items, bool selectOnMouseHover, Action<string> onSelected, string filterString, EditorWindow window, Rect position)
		{
			_selectOnMouseHover = selectOnMouseHover;
			_filter = filterString;
			_items = items;
			_window = window;
			_onSelected = onSelected;
			_position = position;
		}

		public void Draw ()
		{

			//List of items
			_scrollPosition = GUILayout.BeginScrollView (_scrollPosition, GUILayout.Width (_position.width), GUILayout.Height (_position.height));

			for (int i=0; i<_items.Length; i++) {
			
				// Filter item by keyword
				if (!string.IsNullOrEmpty (_filter)) {
					if (_items [i].ToLower ().IndexOf (_filter.ToLower (), StringComparison.Ordinal) < 0)
						continue;
				}
			
				// Draw suitable items
				if (GUILayout.Button (_items [i], menuItemStyle)) {
					if (_onSelected != null) {
						SelectedItem = _items [i];
						_onSelected (_items [i]);
					}
				}
			
				// Update button's status (for hover event)
				if (_selectOnMouseHover)
					_window.Repaint ();
			}
		
			GUILayout.EndScrollView ();
		}
	}

	public class EditablePropertyDrawer
	{

		GUIStyle buttonSaveStyle;
		Type _viewModelType;
		CustomPropertyInfo _property;
		int selectedType = 0;
		string[] observableTypes;
		Action<CustomPropertyInfo> _onPropertyChanged;
		Action<CustomPropertyInfo> _onPropertyDelete;

		public EditablePropertyDrawer (Type viewModelType, CustomPropertyInfo property, Action<CustomPropertyInfo> onPropertyChanged, Action<CustomPropertyInfo> onPropertyDelete)
		{
	
			buttonSaveStyle = new GUIStyle (GUI.skin.FindStyle ("button"));
			buttonSaveStyle.normal.textColor = Color.gray;

			_viewModelType = viewModelType;
			_property = property;	
			_onPropertyChanged = onPropertyChanged;
			_onPropertyDelete = onPropertyDelete;

			observableTypes = ReflectUtils.GetAllObservableType (_viewModelType);
			for (int i = 0; i<observableTypes.Length; i++) {
				if (_property.LastPropertyType.GetAllias () == observableTypes [i]) {
					selectedType = i;
					break;
				}
			}
		}

		public void Draw (float totalWidth)
		{

			GUILayout.BeginHorizontal ();

			GUILayout.BeginVertical ();
			GUILayout.Space (3);
			_property.IsSelected = EditorGUILayout.Toggle (_property.IsSelected);
			GUILayout.EndVertical ();

			GUILayout.BeginVertical ();
			GUILayout.Space (4);

			GUILayoutOption nameWidth = GUILayout.Width (totalWidth * 1 / 3 - 10);
			GUILayoutOption typeWidth = GUILayout.Width (totalWidth / 6 - 10);
			GUILayoutOption defaultValWidth = GUILayout.Width (totalWidth * 1 / 3 - 10);
			GUILayoutOption buttonWidth = GUILayout.Width (totalWidth / 16 - 5);

			// Property name
			_property.LastName = EditorGUILayout.TextField (_property.LastName, nameWidth);
			GUILayout.EndVertical ();

			GUILayout.BeginVertical ();
			GUILayout.Space (4.5f);

			// Property type
			selectedType = EditorGUILayout.Popup (selectedType, observableTypes, typeWidth);
			_property.LastPropertyType = ReflectUtils.GetTypeByName (observableTypes [selectedType]);
			GUILayout.EndVertical ();

			GUILayout.BeginVertical ();
			GUILayout.Space (4);

			// Default value
			if (_property.LastPropertyType == typeof(int)) {
				_property.SetLastValueAs<int> (EditorGUILayout.IntField (_property.GetLastValueAs<int> (), defaultValWidth));
			} else if (_property.LastPropertyType == typeof(long)) {
				_property.SetLastValueAs<long> (EditorGUILayout.LongField (_property.GetLastValueAs<long> (), defaultValWidth));
			} else if (_property.LastPropertyType == typeof(float)) {
				_property.SetLastValueAs<float> (EditorGUILayout.FloatField (_property.GetLastValueAs<float> (), defaultValWidth));
			} else if (_property.LastPropertyType == typeof(double)) {
				_property.SetLastValueAs<double> (EditorGUILayout.DoubleField (_property.GetLastValueAs<double> (), defaultValWidth));
			} else if (_property.LastPropertyType == typeof(string)) {
				if (_property.DefaltValue == null) {
					_property.DefaltValue = string.Empty;
					_property.LastValue = string.Empty;
				}
				_property.SetLastValueAs<string> (EditorGUILayout.TextField (_property.GetLastValueAs<string> (), defaultValWidth));
			} else if (_property.LastPropertyType == typeof(bool)) {
				_property.SetLastValueAs<bool> (EditorGUILayout.Toggle (_property.GetLastValueAs<bool> (), defaultValWidth));
			} else {
				GUILayout.Label ("Undefined!", defaultValWidth);
			}
			GUILayout.EndVertical ();

			if (_property.HasChange)
				buttonSaveStyle.normal.textColor = Color.black;
			else
				buttonSaveStyle.normal.textColor = Color.gray;

			if (GUILayout.Button ("S", buttonSaveStyle, buttonWidth)) {
				if (_property.HasChange) {
					_property.CommitChange ();
					if (_onPropertyChanged != null)
						_onPropertyChanged (_property);
				}
			}

			if (GUILayout.Button ("X", buttonWidth)) {
				if (_onPropertyDelete != null)
					_onPropertyDelete (_property);
			}

			GUILayout.EndHorizontal ();
		}
	}

	public class EditablePopup
	{

		string[] _arrItems;
		string _currentItem;
		int selectedIndex = 0;
		Action<string> _onSave;

		public string SelectedItem {
			get {
				return _arrItems [selectedIndex];
			}
		}

		public EditablePopup (string[] items, string currentItem, Action<string> onSave)
		{
			_currentItem = currentItem;
			_arrItems = items;
			_onSave = onSave;
			for (int i = 0; i < _arrItems.Length; i++) {
				if (_arrItems [i] == currentItem) {
					selectedIndex = i;
					break;
				}
			}
		}

		public void Draw ()
		{
			selectedIndex = EditorGUILayout.Popup (selectedIndex, _arrItems);

			if (_arrItems [selectedIndex] != _currentItem && _onSave != null) {
				if (GUILayout.Button ("Save")) {
					if (_onSave != null)
						_onSave (_arrItems [selectedIndex]);
				}
			}
		}
	}

	public class LineHelper
	{

		static public void Draw (Color color, float width, float height = 1)
		{
			Color backupColor = GUI.color;
			GUI.color = color;
			GUILayout.Box (Texture2D.whiteTexture, GUILayout.Width (width - 20), GUILayout.Height (height));
			GUI.color = backupColor;
		}
	}

	public class LableHelper
	{

		static GUIStyle headerLabel;
		static GUIStyle titleLabel;

		static public void HeaderLabel (string text)
		{
			if (headerLabel == null) {
				headerLabel = new GUIStyle ();
				headerLabel.normal.textColor = Color.black;
				headerLabel.fontStyle = FontStyle.Bold;
				headerLabel.alignment = TextAnchor.MiddleLeft;
			}

			GUILayout.BeginHorizontal ();
			GUILayout.Space (5);
			GUILayout.Label (text, headerLabel);
			GUILayout.EndHorizontal ();
		}

		static public void TitleLabel (string text)
		{
			if (titleLabel == null) {
				titleLabel = new GUIStyle ();
				titleLabel.normal.textColor = Color.black;
				titleLabel.fontStyle = FontStyle.Bold;
				titleLabel.fontSize = titleLabel.fontSize + 15;
				titleLabel.alignment = TextAnchor.MiddleCenter;
			}
		
			GUILayout.BeginHorizontal ();
			GUILayout.Space (5);
			GUILayout.Label (text, titleLabel);
			GUILayout.EndHorizontal ();
		}
	}

	public class PathBrowser {
		public string SelectedPath { get; set; }
		public string StripPattern { get; set; }
		public string Draw (string label) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label (label, GUILayout.Width(80));
			SelectedPath = EditorGUILayout.TextField (SelectedPath);
			if (GUILayout.Button ("...", GUILayout.Height(15))) {
				string newPath = EditorUtility.OpenFolderPanel("Select folder", SelectedPath, "");
				if(!string.IsNullOrEmpty(newPath)) {
					if(!newPath.Contains(Application.dataPath)) {
						EditorUtility.DisplayDialog("Error", "Cannot save file outside of project's asset folder!", "OK");
					}
					else {
						newPath = NormalizePath(newPath);
						SelectedPath = newPath;
					}
				}
			}

			GUILayout.EndHorizontal ();

			return SelectedPath;
		}

		public PathBrowser (string defaultPath, string stripPattern) {
			SelectedPath = defaultPath;
			StripPattern = stripPattern;
		}

		public string NormalizePath (string path) {
			string normalized = path.Replace (StripPattern, "");

			if (normalized != null && normalized.Length >= 1 && normalized.Substring(0,1).Equals("/"))
				normalized = normalized.Substring (1, normalized.Length - 1);

			return normalized + "/";
		}
	}

	public class EditorGUIHelper
	{
		static GUISkin skin;

		static public GUIStyle RichText ()
		{
			GUIStyle style = new GUIStyle ();
			style.richText = true;
			return style;
		}

		static public bool QuickPickerButton ()
		{
			return GUILayout.Button ("Quick Picker");
		}
	}
}