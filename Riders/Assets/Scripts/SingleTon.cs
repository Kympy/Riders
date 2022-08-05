using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTon<T> : MonoBehaviour where T : class, new()
{
    private static volatile T instance;
    private static object lockObj = new object();
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(T)) as T;
                if(instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).ToString(), typeof(T));
                    lock (lockObj)
                    {
                        instance = obj.GetComponent<T>();
                    }
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
