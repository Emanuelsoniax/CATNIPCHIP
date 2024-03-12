using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DefaultExecutionOrder(-5)]
public class SingletonTemplateMono<T> : MonoBehaviour where T : Component
{

	public static T instance;

	public bool dontDestroyOnLoad;

	protected virtual void Awake()
	{
		if (instance == null)
		{
			instance = this as T;

			if (dontDestroyOnLoad)
				DontDestroyOnLoad(gameObject);
		}
		else
			Destroy(gameObject);
	}

}