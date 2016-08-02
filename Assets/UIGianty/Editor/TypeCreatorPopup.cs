using UnityEngine;
using System.Collections;
using UnityEditor;
using UIGianty;
using System;
using System.Text.RegularExpressions;
using System.IO;

public class TypeCreatorPopup : EditorWindow {

	string typeName = "NewViewModel";
	string baseType = "UIManDialog";
	EditablePopup baseTypePopup;
	string[] arrSupportType = new string[3] {
		"ObservableModel",
		"UIManScreen",
		"UIManDialog"
	};
	bool inited = false;
	bool generating = false;
	string savingPath = "";

	void Init () {
		if (!inited) {
			if (baseTypePopup == null) {
				baseTypePopup = new EditablePopup (arrSupportType, "UIManDialog", null);
			}
			minSize = new Vector2(300, 100);
			maxSize = minSize;
			inited = true;
			generating = false;
		}
	}

	void OnGUI () {
		Init ();
		GUILayout.Space (10);
		GUILayout.BeginHorizontal ();
		GUI.SetNextControlName("NameTextField");
		EditorGUILayout.PrefixLabel ("Name");
		typeName = EditorGUILayout.TextField (typeName);
		GUILayout.EndHorizontal ();
		GUI.FocusControl("NameTextField");
		
		baseTypePopup.Draw ();
		LineHelper.Draw (Color.black, 310);
		GUILayout.Space (10);

		if (GUILayout.Button ("Create", GUILayout.Height (30))) {
			GenerateViewModel();
		}
	}

	public void GenerateViewModel () {
		bool warn = false;
		
		if(typeName.Length <= 1 || (!typeName.Substring(0, 2).Equals("UI") && !baseTypePopup.SelectedItem.Equals(CodeGenerator.GetSupportTypeName(0)))) {
			typeName = "UI" + typeName;
			warn = true;
		}

		baseType = baseTypePopup.SelectedItem;

		UIManConfig config = Resources.Load<UIManConfig>("UIManConfig");
		
		string savePath = "";
		if(baseType.Equals(CodeGenerator.GetSupportTypeName(1))) {
			savePath = config.screenScriptFolder;
		}
		else if(baseType.Equals(CodeGenerator.GetSupportTypeName(2))) {
			savePath = config.dialogScriptFolder;
		}
		
		savePath = Application.dataPath + "/" + savePath + typeName + ".cs";
		if (File.Exists (savePath)) {
			EditorUtility.DisplayDialog ("Error", "View model name is already exist, please input other name!", "OK");
			return;
		}

		string[] paths = Regex.Split (savePath, "/");
		string scriptName = paths [paths.Length - 1];
		scriptName = scriptName.Replace (".cs", "");
		
		if (scriptName.Contains (" ")) {
			EditorUtility.DisplayDialog ("Error", "View model name cannot constain special character", "OK");
		} else {
			savingPath = savePath;
			generating = true;
			string code = CodeGenerationHelper.GenerateScript (typeName, baseType);
			CodeGenerationHelper.SaveScript (savePath, code, true);
			GenerateViewModelHandler(savePath);
			EditorUtility.DisplayProgressBar ("Saving inprogress", "Decompiling...", 1f);
			AssetDatabase.Refresh (ImportAssetOptions.Default);

			if(warn) {
				Debug.LogWarning("Code generation warning: Invalid name detected, auto generate is activated!");
			}
		}
	}

	public void GenerateViewModelHandler (string scriptPath)
	{
		string handlerScriptPath = CodeGenerationHelper.GeneratPathWithSubfix (scriptPath, ".Handler.cs");
		string handlerCode = "";
		if (string.IsNullOrEmpty (handlerCode))
			handlerCode = CodeGenerationHelper.GenerateViewModelHandler (typeName, baseType);
		else
			handlerCode = handlerCode.Replace (": " + typeName, ": " + baseType);
		CodeGenerationHelper.SaveScript (handlerScriptPath, handlerCode, false, typeName, baseType);
	}

	void Update () {
		if (generating && !EditorApplication.isCompiling && !EditorApplication.isUpdating) {
			if (File.Exists (savingPath)) {
				generating = false;
				Close ();
			}
		}
	}
			
	/*void Update () {
		if (generating && !EditorApplication.isCompiling && !EditorApplication.isUpdating) {

			if (File.Exists (savingPath)) {
				generating = false;

				ReflectUtils.RefreshAssembly (true);
				EditorUtility.ClearProgressBar ();

				string[] template = AssetDatabase.FindAssets ("UIMAN_TEMPLATE_PREB");
				if (template != null && template.Length > 0) {
					
					UIManConfig config = Resources.Load<UIManConfig>("UIManConfig");

					string prefabPath = "";
					if(baseType.Equals(CodeGenerator.GetSupportTypeName(1))) {
						prefabPath = config.screenPrefabFolder;
					}
					else if(baseType.Equals(CodeGenerator.GetSupportTypeName(2))) {
						prefabPath = config.dialogPrefabFolder;
					}

					AssetDatabase.CopyAsset (AssetDatabase.GUIDToAssetPath(template [0]), prefabPath + typeName + ".prefab");
					template = AssetDatabase.FindAssets (typeName);
					if (template != null && template.Length > 0) {
						GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject> (AssetDatabase.GUIDToAssetPath (template [0]));
					}

					AssetDatabase.Refresh ();
				}

				Close ();
			}
		}
	}*/
}
