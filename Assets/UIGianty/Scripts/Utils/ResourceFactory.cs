using System.Collections.Generic;
using UnityEngine;
using UIGianty;

public class ResourceFactory {


	static private Dictionary<string, Object> cache = new Dictionary<string, Object>();

	static public void Cache (string path, Object value, bool forceOverwrite = false) {
		if (!cache.ContainsKey (path)) {
			cache.Add (path, value);
		} else {
			if(forceOverwrite)
				cache[path] = value;
		}
	}

	static public T Load<T> (string path) where T : Object {
		path = NormalizePath (path);
		Object res = null;
		if(!cache.TryGetValue(path, out res)) {
			res = Resources.Load<T>(path);
			if (res != null)
				Cache (path, res);
		}

		return (T)res;
	}

	static public void LoadAsync<T> (string path, System.Action<T, object[]> onLoaded, params object[] callbackArgs) where T : Object {
		path = NormalizePath (path);
		Object res = null;
		if (!cache.TryGetValue (path, out res)) {
			LoadResourceAsync<T>(path, value=>onLoaded(value, callbackArgs));
		} else {
			onLoaded((T)res, callbackArgs);
		}
	}

	static private void LoadResourceAsync<T> (string path, System.Action<T> onLoaded) where T : Object {
		GameObject requestObj = new GameObject ("AsyncResourceLoader");
		AsyncResourcesLoader loader = requestObj.AddComponent<AsyncResourcesLoader> ();
		loader.Load<T> (path, value=>onLoaded(value));
	}

	static public string NormalizePath (string path) {
		string normalized = path.Replace (Application.dataPath, "");
		if(normalized.Contains("/Resources")) {
			int stripIndex = normalized.LastIndexOf("/Resources") + 11;
			if(normalized.Length > 0)
				normalized = normalized.Substring(stripIndex, normalized.Length-stripIndex);
		}

		if (normalized != null && normalized.Length >= 1 && normalized.Substring(0,1).Equals("/"))
			normalized = normalized.Substring (1, normalized.Length - 1);

		return normalized;
	}
}
