using UnityEngine;
using System.Collections;
using System;

[StartupAttribute(StartupType.NORMAL, null)]
public class GM : SystemSingleton<GM> {

	bool _isReady = false;
	public bool IsReady {
		get {
			return _isReady;
		}
	}

	public override void Init () {

		if (_isReady)
			return;

		// Init managers here

		_isReady = true;
	}

	public T CreateManager<T> () where T : MonoBehaviour {
		T _instance = default(T);
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
