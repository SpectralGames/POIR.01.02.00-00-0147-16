using UnityEngine;
using System.Collections;
using System;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;


public static class Globals
{

    public static T Load<T> (bool isEditor = true, string path = "") where T : new()
    {
        XmlReader reader = null;
        System.IO.StreamWriter file = null;

        T tmp = default(T);

        try
        {
            if (String.IsNullOrEmpty(path))
            {
                path = System.IO.Directory.GetParent(Application.dataPath) + "/" + typeof(T).ToString() + ".xml";
            }

            if (isEditor)
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            if (System.IO.File.Exists(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                FileStream fs = new FileStream(path, FileMode.Open);
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.CloseInput = true;
                reader = XmlReader.Create(fs, settings);

                tmp = (T)serializer.Deserialize(reader);
                //Logger.Log("Load " + typeof(T).ToString());
                return tmp;
            }
            else
            {
                tmp = new T();
                tmp.ToString();
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                file = new System.IO.StreamWriter(path);
                writer.Serialize(file, tmp);
                //Logger.Log("Save " + typeof(T).ToString());
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.Message + "  " + e.ToString());
        }
        finally
        {
            if (reader != null)
                reader.Close();
            if (file != null)
                file.Close();
        }

        return tmp;
    }

    public static void Save<T> (T _input, string addfolder = "", string addname = "") where T : new()
    {
        string path = string.Empty;
        if (string.IsNullOrEmpty(addfolder))
        {
            path = System.IO.Directory.GetParent(Application.dataPath) + "/" + typeof(T).ToString() + addname + ".xml";
        }
        else
        {
            path = System.IO.Directory.GetParent(Application.dataPath) + "/" + addfolder + "/" + typeof(T).ToString() + addname + ".xml";
        }

        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        System.IO.StreamWriter file = new System.IO.StreamWriter(path);
        writer.Serialize(file, _input);
        file.Close();
    }

    /// <summary>
    /// HOW TO USE?
    /// timer += Time.deltaTime;
    /// float factor = Globals.Easing(timer / duration);
    /// col = Color.Lerp(offColor, onColor, factor);
    /// duration - czas w jakim ma sie zrobic animacja
    /// </summary>
    /// <param name="normalizedTime"></param>
    /// <returns></returns>
    public static float Easing (float normalizedTime)
    {
        normalizedTime = Mathf.Clamp(normalizedTime, 0.0f, 1.0f);
        if (normalizedTime <= 0.5f)
        {
            return (2.0f * normalizedTime * normalizedTime);
        }
        else
        {
            return ((4.0f * normalizedTime) - (2.0f * normalizedTime * normalizedTime) - 1.0f);
        }
    }
    
}
