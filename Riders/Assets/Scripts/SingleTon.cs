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
                instance = FindObjectOfType(typeof(T)) as T;
                if(instance == null)
                {
                    lock (lockObj)
                    {
                        GameObject obj = new GameObject(typeof(T).ToString(), typeof(T));
                        instance = obj.GetComponent<T>();
                        DontDestroyOnLoad(obj);
                    }
                }
                return instance;
            }
            return instance;
        }
    }
}
