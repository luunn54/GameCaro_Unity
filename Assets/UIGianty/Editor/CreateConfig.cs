using UnityEngine;
using UnityEditor;

public class CreateConfigFile
{
	[MenuItem("UIGianty/Configuration")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<UIManConfig> ();
	}
}