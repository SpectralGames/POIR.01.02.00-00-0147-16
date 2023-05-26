using UnityEngine;
using System.Collections;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Members
    protected bool isInit = false;
    public bool IsInit
    {
        get { return this.isInit; }
    }
    #endregion

    #region Instance
    private static T instance = null;
    public static T Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    string goName = typeof(T).ToString();
                    GameObject go = GameObject.Find(goName);
                    if (go == null)
                    {
                        go = new GameObject();
                        go.name = goName;
                    }
                    instance = go.AddComponent<T>();
                }
            }
            return instance;
        }
    }
    #endregion
}