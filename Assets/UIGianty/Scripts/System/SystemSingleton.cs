using UnityEngine;
using System.Collections;
using System;

[DisallowMultipleComponent]
public class SystemSingleton<T> : MonoBehaviour where T : MonoBehaviour {

	static T _instance;
    public static bool IsInstance()
    {
        return _instance != null;
    }
	public static T Instance
	{
		get
		{
			if (_instance == null) {
				_instance = FindObjectOfType<T>();
				
				if (_instance == null) {

					object[] attributes = typeof(T).GetCustomAttributes(typeof(StartupAttribute), true);
					StartupType type = StartupType.NORMAL;
					Type parent = typeof(GM);
					if(attributes != null && attributes.Length > 0) {
						StartupAttribute attribute = (StartupAttribute)attributes[0];
						type = attribute.Type;
						parent = attribute.ParentType;
					}

					if(type == StartupType.NORMAL) {
						_instance = new GameObject(typeof(T).Name).AddComponent<T>();
					}
					else {
						UnityEngine.Object obj = Resources.Load("Managers/" + typeof(T).Name);
						if(obj != null) {
							_instance = (Instantiate(obj) as GameObject).GetComponent<T>();
							_instance.name = typeof(T).Name;
						}
						else {
							Debug.LogError("Manager could not be found, make sure you have put prefab into resources with the right name!");
						}
					}

					if(parent != null) {
						MonoBehaviour parentBehavior = (MonoBehaviour)GameObject.FindObjectOfType(parent);
						if(parentBehavior == null) {
							Debug.LogError("Parent object could not be found, make sure you have initial parent object before!");
						}
						else {
							_instance.transform.SetParent(parentBehavior.transform);
						}
					}
				}
			}

			return _instance;
		}
	}

	protected virtual void Awake ()
	{
		if (this != Instance) {
			GameObject go = gameObject;
			Destroy(this);
			Destroy(go);
			
		} else {
			DontDestroyOnLoad(gameObject);
		}

		Init ();
	}

	public virtual void Init () {
		
	}
}
