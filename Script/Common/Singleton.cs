using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : class, new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            instance??= new T();
            return instance;
        }
    }
}

public class MonoSingleton<T> :MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            
            return instance;
        }
    }


    private void Awake()
    {
        instance = this as T;
    }
}
