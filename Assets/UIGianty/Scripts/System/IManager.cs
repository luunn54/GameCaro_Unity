using UnityEngine;
using System.Collections;

public interface IManager<T> {

	T Init();
	bool IsReady {get; set;}
}
