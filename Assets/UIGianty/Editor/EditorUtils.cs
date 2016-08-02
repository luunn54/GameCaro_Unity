﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

namespace UIGianty
{
	public class EditorUtils : Editor
	{
		static public void CreatePath (string path)
		{
			string root = Application.dataPath;
			string[] folders = Regex.Split (path, "/");
			string curFolder = root + "/";
			for (int i=0; i<folders.Length; i++) {
				if (i == 0 && folders [i] == "Assets")
					continue;
				curFolder += "/" + folders [i];
				if (!Directory.Exists (curFolder)) {
					Directory.CreateDirectory (curFolder);
				}
			}
		}
	}
}