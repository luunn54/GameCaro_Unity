using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UIGianty
{
	public class CodeGenerator : EditorWindow
	{

		static string savedDirectory;
		static CodeGenerator container;
		UISearchField searchField;
		ListView listTypes;
		EditablePopup baseTypePopup;
		static string[] types;
		static Rect rctTypeWindow;
		static Rect rctLayoutWindow;
		static Rect rctPropertiesWindow;
		static Dictionary<Type, EditablePropertyDrawer[]> propertiesDrawerCache = new Dictionary<Type, EditablePropertyDrawer[]> ();
		static Type selectedType = null;
		static CustomPropertyInfo[] selectedProperties = null;
		static string currentScriptPath = null;
		static string handlerScriptPath = null;
		static string[] arrSupportType = new string[3] {
		"ObservableModel",
		"UIManScreen",
		"UIManDialog"
		};
		static UIManConfig config;
		static PathBrowser screenPrefabPath;
		static PathBrowser dialogPrefabPath;
		static PathBrowser screenScriptPath;
		static PathBrowser dialogScriptPath;

		static public string GetSupportTypeName (int index) {
			return arrSupportType [index];
		}

		[MenuItem("UIGianty/Script Generator")]
		static void Init ()
		{
			ReflectUtils.RefreshAssembly (false);
			types = ReflectUtils.GetAllUIManType ();
			container = EditorWindow.GetWindow<CodeGenerator> (true, "Script Generator");
			container.minSize = new Vector2 (800, 600);
			container.maxSize = container.minSize;
			rctTypeWindow = new Rect (0, 0, 250, 500);
			rctPropertiesWindow = new Rect (260, 0, 540, 600);
			config = Resources.Load<UIManConfig> ("UIManConfig");
			screenPrefabPath = new PathBrowser (config.screenPrefabFolder, Application.dataPath);
			dialogPrefabPath = new PathBrowser (config.dialogPrefabFolder, Application.dataPath);
			screenScriptPath = new PathBrowser (config.screenScriptFolder, Application.dataPath);
			dialogScriptPath = new PathBrowser (config.dialogScriptFolder, Application.dataPath);
		}

		void OnGUI ()
		{
			if (EditorApplication.isCompiling) {
				EditorUtility.DisplayProgressBar ("Compiling...", "Unity is compiling...", 1);
			} else {
				EditorUtility.ClearProgressBar();
			}

			if (container == null) {
				Init ();
			}

			BeginWindows ();
			GUILayout.Window (0, rctTypeWindow, TypeWindow, "UIMan Type Browser");
			GUILayout.Window (2, rctPropertiesWindow, PropertiesWindow, "View Model Property");
			EndWindows ();
		}

		void TypeWindow (int id = 0)
		{
			GUILayout.BeginVertical ();

			config.screenPrefabFolder = screenPrefabPath.Draw ("ScreenPrefab");
			config.dialogPrefabFolder = dialogPrefabPath.Draw ("DialogPrefab");
			config.screenScriptFolder = screenScriptPath.Draw ("ScreenScript");
			config.dialogScriptFolder = dialogScriptPath.Draw ("DialogScript");
			EditorUtility.SetDirty (config);

			GUILayout.Space (2);

			GUILayout.EndVertical ();

			if (searchField == null)
				searchField = new UISearchField (OnSearchType, OnClickCreateType);
			
			searchField.Draw ();

			if (listTypes == null)
				listTypes = new ListView ();
			listTypes.SetData (types, false, OnSelecType, searchField.KeyWord, this, rctTypeWindow);
			listTypes.Draw ();
		}
	
		void LayoutWindow (int id = 1)
		{
			GUILayout.Label ("NO LAYOUT HAS BEEN SELECTED!");
		}
	
		void PropertiesWindow (int id = 2)
		{
			GUILayout.BeginVertical ();

			if (listTypes != null && !string.IsNullOrEmpty (listTypes.SelectedItem)) {
				if (selectedType != null) {

					// Title
					GUILayout.Space (2);
					LableHelper.TitleLabel (selectedType.Name);
					LineHelper.Draw (Color.gray, rctPropertiesWindow.width);

					// Base type
					GUILayout.Space (10);
					LableHelper.HeaderLabel ("Type");
					LineHelper.Draw (Color.gray, rctPropertiesWindow.width);
					baseTypePopup.Draw ();

					if (baseTypePopup.SelectedItem != "ObservableModel") {
						if (!System.IO.File.Exists (handlerScriptPath)) {
							if (GUILayout.Button ("Generate Handler")) {
								string backupCode = CodeGenerationHelper.DeleteScript (handlerScriptPath);
								GenerateViewModelHandler (backupCode, selectedType.BaseType.Name);
							}
						}
					}

					// Properties
					GUILayout.Space (10);
					LableHelper.HeaderLabel ("Properties");
					LineHelper.Draw (Color.gray, rctPropertiesWindow.width);
					if (propertiesDrawerCache.ContainsKey (selectedType)) {
						EditablePropertyDrawer[] props = propertiesDrawerCache [selectedType];
						for (int i=0; i<props.Length; i++) {
							props [i].Draw (rctPropertiesWindow.width);
						}
					}
				}

				GUILayout.Space (10);
			
				// Add property
				if (GUILayout.Button ("Add New Property", GUILayout.Height (30))) {
					int newIndex = 0;
					string strNewIndex = "";
					for (int i=0; i<selectedProperties.Length; i++) {
						if (selectedProperties [i].LastName.Contains ("NewProperty"))
							newIndex++;
					}
					if (newIndex > 0)
						strNewIndex = newIndex.ToString ();
					CustomPropertyInfo newProperty = new CustomPropertyInfo ("", typeof(string));
					newProperty.LastName = "NewProperty" + strNewIndex;
					ArrayUtility.Add<CustomPropertyInfo> (ref selectedProperties, newProperty);
					CachePropertiesDrawer ();
				}

				//Save all change
				CustomPropertyInfo[] changeList = new CustomPropertyInfo[0];
				CustomPropertyInfo[] selectedList = new CustomPropertyInfo[0];
				for (int i=0; i<selectedProperties.Length; i++) {
					if (selectedProperties [i].HasChange)
						ArrayUtility.Add (ref changeList, selectedProperties [i]);
					if (selectedProperties [i].IsSelected)
						ArrayUtility.Add (ref selectedList, selectedProperties [i]);
				}

				GUILayout.Space (10);
				LineHelper.Draw (Color.gray, rctPropertiesWindow.width);
				GUILayout.Space (5);

				if (changeList.Length > 0) {
					if (GUILayout.Button ("Save All Changes", GUILayout.Height (30))) {
						for (int i=0; i<changeList.Length; i++) {
							changeList [i].CommitChange ();
						}
						SaveCurrentType (true, baseTypePopup.SelectedItem);
					}
				}

				if (selectedList.Length > 0) {
					if (GUILayout.Button ("Delete Selected Properties", GUILayout.Height (30))) {
						for (int i=0; i<selectedList.Length; i++) {
							ArrayUtility.Remove (ref selectedProperties, selectedList [i]);
						}
						SaveCurrentType (true, baseTypePopup.SelectedItem);
						CachePropertiesDrawer (true);
					}
				}

				if (selectedProperties.Length > 0) {
					if (GUILayout.Button ("Delete All Properties", GUILayout.Height (30))) {
						while (selectedProperties.Length > 0) {
							ArrayUtility.Clear (ref selectedProperties);
							SaveCurrentType ();
							CachePropertiesDrawer (true);
						}
					}
				}
			} else {
				GUILayout.Label ("NO DATA FOR PREVIEW!");
			}


			GUILayout.EndVertical ();
		}

		void CachePropertiesDrawer (bool clearCurrentCache = false)
		{
			if (selectedType == null)
				return;
			if (clearCurrentCache)
				propertiesDrawerCache.Clear ();
			if (!propertiesDrawerCache.ContainsKey (selectedType)) {
				propertiesDrawerCache.Add (selectedType, new EditablePropertyDrawer[0]);
			}
			EditablePropertyDrawer[] drawers = new EditablePropertyDrawer[0];
			for (int i=0; i<selectedProperties.Length; i++) {
				EditablePropertyDrawer drawer = new EditablePropertyDrawer (selectedType, selectedProperties [i], OnApplyPropertyChanged, OnPropertyDelete);
				ArrayUtility.Add<EditablePropertyDrawer> (ref drawers, drawer);						
			}
			propertiesDrawerCache [selectedType] = drawers;
		}

		public void OnSearchType (string keyword)
		{
		}

		public void OnClickCreateType (object obj)
		{
			GetWindow<TypeCreatorPopup> (true, "Create new type", true);
		}

		public void OnChangeBaseType (string newBaseType)
		{
			SaveCurrentType (true, newBaseType);
		}

		public void OnSelecType (string typeName)
		{
			selectedType = ReflectUtils.GetTypeByName (typeName);
			selectedProperties = selectedType.GetUIManProperties ();
			baseTypePopup = new EditablePopup (arrSupportType, selectedType.BaseType.Name, OnChangeBaseType);
			currentScriptPath = CodeGenerationHelper.GetScriptPathByType (selectedType);
			handlerScriptPath = CodeGenerationHelper.GeneratPathWithSubfix (currentScriptPath, ".Handler.cs");
			CachePropertiesDrawer ();
		}

		public void OnApplyPropertyChanged (CustomPropertyInfo newInfo)
		{
			SaveCurrentType (true);
		}

		public void OnPropertyDelete (CustomPropertyInfo property)
		{
			ArrayUtility.Remove<CustomPropertyInfo> (ref selectedProperties, property);
			SaveCurrentType ();
			CachePropertiesDrawer ();
		}

		public void SaveCurrentType (bool warning = false, string baseType = null)
		{

			// Verify properties list
			for (int i=0; i<selectedProperties.Length; i++) {
				CustomPropertyInfo property = selectedProperties [i];
				if (string.IsNullOrEmpty (property.Name) || Char.IsNumber (property.Name [0])) {
					property.Name = "";
					if (warning)
						EditorUtility.DisplayDialog ("Save script error", "Property name cannot be a digit, null or empty!", "OK");
					return;
				}

				for (int j=0; j<selectedProperties.Length; j++) {
					if (j != i && selectedProperties [i].Name.ToLower () == selectedProperties [j].Name.ToLower ()) {
						selectedProperties [j].Name = "";
						if (warning)
							EditorUtility.DisplayDialog ("Save script error", "There are one or more properties are have the same name!", "OK");
						return;
					}
				}
			}

			if (baseType == null)
				baseType = selectedType.BaseType.Name;

			if (!string.IsNullOrEmpty (currentScriptPath)) {
				string backupCode = CodeGenerationHelper.DeleteScript (handlerScriptPath);
				string code = CodeGenerationHelper.GenerateScript (selectedType.Name, baseType, selectedProperties);

				bool saved = CodeGenerationHelper.SaveScript (currentScriptPath, code, true);

				if (baseType != "ObservableModel") {
					GenerateViewModelHandler (backupCode, baseType);
					saved = false;
				}

				if (saved) {
					EditorUtility.DisplayProgressBar ("Saving inprogress", "Decompiling...", 0.1f);
					AssetDatabase.Refresh (ImportAssetOptions.Default);
				}
			}
		}

		public void GenerateViewModelHandler (string backupCode, string baseType = null)
		{
			if (string.IsNullOrEmpty (baseType))
				baseType = selectedType.BaseType.Name;

			string handlerCode = backupCode;
			if (string.IsNullOrEmpty (handlerCode))
				handlerCode = CodeGenerationHelper.GenerateViewModelHandler (selectedType.Name, baseType);
			else
				handlerCode = handlerCode.Replace (": " + selectedType.BaseType.Name, ": " + baseType);
			bool saved = CodeGenerationHelper.SaveScript (handlerScriptPath, handlerCode, false, selectedType.BaseType.Name, baseType);
			if (saved) {
				EditorUtility.DisplayProgressBar ("Saving inprogress", "Decompiling...", 0.1f);
				AssetDatabase.Refresh (ImportAssetOptions.Default);
			}
		}

		[UnityEditor.Callbacks.DidReloadScripts]
		static void OnScriptsReloaded ()
		{
			ReflectUtils.RefreshAssembly (true);
			EditorUtility.ClearProgressBar ();
		}
	}
}