using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;

public class UDKImporter
{
    public static string regexPatternFloat = "([-+]?[0-9]*\\.[0-9]+)";
    public static string regexPatternInt = "([-]?[0-9]+)";    //TODO avoid gathering flaot numbers
    public static string regexPatternCoords = String.Format("\\(X={0},Y={0},Z={0}\\)",regexPatternFloat);
    public static string regexPatternPitchYawRoll = String.Format("\\(Pitch={0},Yaw={0},Roll={0}\\)",regexPatternInt);
    public static string regexPatternLocation = String.Format("\\bLocation\\b={0}",regexPatternCoords);
    public static string regexPatternRotation = String.Format("\\bRotation\\b=\\(Pitch={0},Yaw={0},Roll={0}\\)",regexPatternInt);
    public static string regexPatternDrawScale = String.Format("\\bDrawScale\\b={0}",regexPatternFloat);

    const double RadToDeg = 57.295779513082321600; // 180 / Pi
    const double DegToRad = 0.017453292519943296;  // Pi / 180
    const double UnrRotToRad = 0.00009587379924285;  // Pi / 32768
    const double RadToUnrRot = 10430.3783504704527;  // 32768 / Pi
    const double DegToUnrRot = 182.0444;
    const double UnrRotToDeg = 0.00549316540360483;
 

    public List<UDKMap> listOfMaps = new List<UDKMap>();
        
    public UDKImporter(string path)
    {
        // Open the stream and read it back. 
        using(StreamReader sr = File.OpenText(path))
        {
            string s = "";
            while ((s=sr.ReadLine())!=null) 
            {
                if(s.Contains("Begin Map"))
                {
                    UDKMap map = new UDKMap(s);
                    map.Parse(sr);
                    listOfMaps.Add(map);
                }
            }
        }
    }
        
    public UDKActor GetActorByName(string name)
    {
        List<UDKActor> actors = listOfMaps[0].levels[0].actors;
        
        foreach(UDKActor actor in actors)
        {
            if(actor.name==name)
                return actor;
        }
        
        return null;
    }
    
    public UDKActor GetActorLinkedWith(string name)
    {
        List<UDKActor> actors = listOfMaps[0].levels[0].actors;
        foreach(UDKActor actor in actors)
        {
            foreach(UDKActor.LinksFrom link in actor.listOfLinksFrom)
            {
                if(link.linkActorName==name)
                {
                    return actor;
                }
            }
        }
        
        return null;
    }
    
    public UDKActor GetRootActor()
    {
        List<UDKActor> actors = listOfMaps[0].levels[0].actors;
        
        foreach(UDKActor actor in actors)
        {
            if(actor.listOfLinksFrom.Count==0)
                return actor;
        }
        
        return null;
    }

    public static Vector3 UDKPositionToUnityPosition(Vector3 udkPosition)
    {
        Vector3 destPos = new Vector3(-udkPosition.x, udkPosition.z, udkPosition.y );
        destPos*=0.01f;
        return destPos;
    }

    static int NormalizeAxis(int Angle)
    {
        Angle &= 0xFFFF;
        if( Angle > 32767 )
        {
            Angle -= 0x10000;
        }
        return Angle;
    }

    public static Quaternion UDKRotationToUnityRotation(Vector3 udkRotation)
    {
        udkRotation.x = (float)NormalizeAxis((int)udkRotation.x);
        udkRotation.y = (float)NormalizeAxis((int)udkRotation.y);
        udkRotation.z = (float)NormalizeAxis((int)udkRotation.z);

        udkRotation *= (float)UnrRotToDeg;
                
        //Debug.Log(udkRotation);

        Quaternion rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        Quaternion quaternion = Quaternion.Euler(udkRotation.z, udkRotation.y, -udkRotation.x);
        quaternion = rotation * Quaternion.Inverse(rotation) * quaternion * rotation;
        return quaternion;
    }

    public static Vector3 UDKScaleToUnityScale(Vector3 udkScale)
    {
        return udkScale;
    }

    public static bool UDKPosition(string strCoordinates, ref Vector3 position)
    {
        Regex r = new Regex(regexPatternCoords);
        Match match = r.Match(strCoordinates);
        if(match.Success)
        {
            position = new Vector3( float.Parse( match.Groups[1].Value ), float.Parse( match.Groups[2].Value), float.Parse( match.Groups[3].Value) );
            return true;
        }

        position = Vector3.zero;
        return false;
    }

    public static bool UDKRotation(string strRotation, ref Vector3 udkRotation)
    {
        Regex r = new Regex(regexPatternPitchYawRoll);
        Match match = r.Match(strRotation);
        if(match.Success)
        {
            udkRotation = new Vector3( int.Parse( match.Groups[1].Value), int.Parse( match.Groups[2].Value), int.Parse( match.Groups[3].Value ) );
            return true;
        }

        udkRotation = Vector3.zero;
        return false;
    }
}
