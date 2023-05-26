using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public static class Extensions
{
    public static T FindComponentInChild<T> (this GameObject parent) where T : Component
    {
        List<Transform> children = new List<Transform>();
        children.Add(parent.transform);

        for (int i = 0; i < children.Count; i++)
        {
            for (int j = 0; j < children[i].childCount; j++)
            {
                Transform child = children[i].GetChild(j);
                T result = child.GetComponent<T>();
                if (result != null)
                {
                    return result;
                }
                else
                    children.Add(child);
            }
        }
        return null;
    }

    public static List<T> FindComponentsInChild<T> (this GameObject parent) where T : Component
    {
        List<T> foundObjects = new List<T>();
        List<Transform> children = new List<Transform>();
        children.Add(parent.transform);

        for (int i = 0; i < children.Count; i++)
        {
            for (int j = 0; j < children[i].childCount; j++)
            {
                Transform child = children[i].GetChild(j);
                T result = child.GetComponent<T>();
                if (result != null)
                {
                    foundObjects.Add(result);
                    children.Add(child);
                }
                else
                    children.Add(child);
            }
        }
        return foundObjects;
    }

    public static T FindComponentInChildWithTag<T> (this GameObject parent, string tag) where T : Component
    {
        List<Transform> children = new List<Transform>();
        children.Add(parent.transform);

        for (int i = 0; i < children.Count; i++)
        {
            for (int j = 0; j < children[i].childCount; j++)
            {
                Transform child = children[i].GetChild(j);
                if (child.gameObject.tag == tag)
                {
                    T result = child.GetComponent<T>();
                    if (result != null)
                        return result;
                    else
                        children.Add(child);
                }
                else
                    children.Add(child);
            }
        }
        return null;
    }

    public static T FindComponentInChildWithName<T> (this GameObject parent, string name) where T : Component
    {
        List<Transform> children = new List<Transform>();
        children.Add(parent.transform);

        for (int i = 0; i < children.Count; i++)
        {
            for (int j = 0; j < children[i].childCount; j++)
            {
                Transform child = children[i].GetChild(j);
                if (child.gameObject.name == name)
                {
                    T result = child.GetComponent<T>();
                    if (result != null)
                        return result;
                    else
                        children.Add(child);
                }
                else
                    children.Add(child);
            }
        }
        return null;
    }

    public static T FindComponentInChildStartsWithName<T> (this GameObject parent, string name) where T : Component
    {
        List<Transform> children = new List<Transform>();
        children.Add(parent.transform);

        for (int i = 0; i < children.Count; i++)
        {
            for (int j = 0; j < children[i].childCount; j++)
            {
                Transform child = children[i].GetChild(j);
                if (child.gameObject.name.StartsWith(name))
                {
                    T result = child.GetComponent<T>();
                    if (result != null)
                        return result;
                    else
                        children.Add(child);
                }
                else
                    children.Add(child);
            }
        }
        return null;
    }

    public static List<T> FindComponentsInChildWithName<T> (this GameObject parent, string name) where T : Component
    {
        List<T> foundObjects = new List<T>();
        List<Transform> children = new List<Transform>();
        children.Add(parent.transform);

        for (int i = 0; i < children.Count; i++)
        {
            for (int j = 0; j < children[i].childCount; j++)
            {
                Transform child = children[i].GetChild(j);
                if (child.gameObject.name.StartsWith(name))
                {
                    T result = child.GetComponent<T>();
                    if (result != null)
                    {
                        foundObjects.Add(result);
                        children.Add(child);
                    }
                    else
                        children.Add(child);
                }
                else
                    children.Add(child);
            }
        }
        return foundObjects;
    }

    public static List<T> FindComponentsInChildStarsWithName<T> (this GameObject parent, string name) where T : Component
    {
        List<T> foundObjects = new List<T>();
        List<Transform> children = new List<Transform>();
        children.Add(parent.transform);

        for (int i = 0; i < children.Count; i++)
        {
            for (int j = 0; j < children[i].childCount; j++)
            {
                Transform child = children[i].GetChild(j);
                if (child.gameObject.name == name)
                {
                    T result = child.GetComponent<T>();
                    if (result != null)
                    {
                        foundObjects.Add(result);
                        children.Add(child);
                    }
                    else
                        children.Add(child);
                }
                else
                    children.Add(child);
            }
        }
        return foundObjects;
    }

    public static List<T> FindInactiveChildren<T> (this Transform parent) where T : Component
    {
        List<T> newList = new List<T>();

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child != null)
            {
                T result = child.GetComponent<T>();
                if (result != null && !child.gameObject.activeSelf)
                {
                    newList.Add(result);
                }
            }
        }

        return newList;
    }

    public static void RemoveAt<T> (this T[,] array, T toRemove) where T : Component
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                if (array[i, j] == toRemove)
                {
                    array[i, j] = null;
                    return;
                }
            }
        }
    }

    public static void RemoveAt<T> (this T[] array, T toRemove) where T : Component
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            if (array[i] == toRemove)
            {
                array[i] = null;
                return;
            }
        }
    }

    public static List<T> DeleteDuplicates<T> (this List<T> list) where T : Component
    {
        List<T> newList = new List<T>();

        foreach (T item in list)
        {
            if (!newList.Contains(item))
                newList.Add(item);
        }

        return newList;
    }


    public static IEnumerator MoveLocalTo(this Transform parent, Vector3 target, float duration)
    {
        float timer = 0.0f;
        Vector3 parentPosition = parent.localPosition;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float factor = Globals.Easing(timer / duration);

            parent.localPosition = Vector3.Lerp(parentPosition, target, factor);

            yield return null;
        }
        Debug.Log("MoveLocalTo END");
        parent.localPosition = target;
    }

    public static IEnumerator MoveGlobalTo (this Transform parent, Vector3 target, float duration)
    {
        float timer = 0.0f;
        Vector3 parentPosition = parent.position;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float factor = Globals.Easing(timer / duration);

            parent.position = Vector3.Lerp(parentPosition, target, factor);

            yield return null;
        }
        parent.position = target;
    }

    public static IEnumerator Scale(this Transform parent, Vector3 target, float duration)
    {
        float timer = 0.0f;
        Vector3 parentScale = parent.localScale;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float factor = Globals.Easing(timer / duration);

            parent.localScale = Vector3.Lerp(parentScale, target, factor);

            yield return null;
        }
        parent.localScale = target;
    }


    #region Lerps
    public static IEnumerator Lerp(this Image parent, Color target, float duration)
    {
        float timer = 0.0f;
        Color parentColour = parent.color;

        while(timer < duration)
        {
            timer += Time.deltaTime;
            float factor = Globals.Easing(timer / duration);

            parent.color = Color.Lerp(parentColour, target, factor);

            yield return null;
        }
        parent.color = target;
    }

    public static IEnumerator Lerp (this Material parent, Color target, float duration)
    {
        float timer = 0.0f;
        Color parentColour = parent.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float factor = Globals.Easing(timer / duration);

            parent.color = Color.Lerp(parentColour, target, factor);

            yield return null;
        }
        parent.color = target;
    }
    #endregion Lerps
}