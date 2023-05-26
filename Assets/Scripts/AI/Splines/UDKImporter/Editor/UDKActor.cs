using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class UDKActor
{
    public Vector3 udkPosition;
    public Vector3 udkRotation;
    public float udkDrawScale = 1f;
    public Vector3 udkSplineActorTangent;

    public class AttachTo
    {
        public string baseClassName;
        public string baseActorName;
        public string boneName;
        public Vector3 udkOffset;
        public Vector3 udkRotation;
    }

    public AttachTo attach;

    public Vector3 UnityPosition
    {
        get
        {
            return UDKImporter.UDKPositionToUnityPosition(udkPosition);
        }
    }

    public Quaternion UnityRotation
    {
        get
        {
            return UDKImporter.UDKRotationToUnityRotation(udkRotation);
        }
    }

    public Vector3 UnityScale
    {
        get
        {
            return UDKImporter.UDKScaleToUnityScale(Vector3.one*udkDrawScale);
        }
    }

    public List<LinksFrom> listOfLinksFrom=new List<LinksFrom>();
    public List<Connection> listOfConnections = new List<Connection>();
    public List<UDKObject> listOfObjects = new List<UDKObject>();
    
    public string className ="";
    public string name = "";
    
    public class LinksFrom
    {
        public string linkClassName;
        public string linkActorName;
        
        public override string ToString()
        {
            return string.Format("[LinksFrom] {0}.'{1}'",linkClassName,linkActorName);
        }
    }

    public class Connection
    {
        public string name;
        public string fromObjectClassName;
        public string fromObjectName;

        public string toActorClassName;
        public string toActorName;

        public override string ToString()
        {
            return string.Format("[Connection] {0}={1}'{2}',ConnectTo={3}'{4}'",name,fromObjectClassName,fromObjectName,toActorClassName,toActorName);
        }
    }
    
    public UDKActor(string s)
    {
        Regex r1 = new Regex("Class=(\\w+) Name=(\\w+) Archetype=(\\w+)'(\\w+).(\\w+)'");
        
        Match match = r1.Match(s);
        if(match.Success)
        {
            className = match.Groups[1].Value;
            name = match.Groups[2].Value;
            
            /*
                    foreach(Group g in match.Groups)
                    {
                        Debug.Log(g);
                    }
                    */
        }
    }

    public override string ToString()
    {
        return string.Format("[UDKActor] {0}'{1}', Location: {2}, Rotation: {3}, DrawScale: {4}",className,name, udkPosition,udkRotation,udkDrawScale);
    }

    public void Parse(StreamReader sr)
    {
        string s = "";
        while((s=sr.ReadLine())!=null)
        {
            if(TryParseLocation(s)==false && TryParseRotation(s)==false && TryParseDrawScale(s)==false)
            {
                if(s.Contains("End Actor"))
                {
                    break;
                }
                else if(UDKObject.IsBeginObject(s))
                {
                    UDKObject obj = UDKObject.CreateObject(s);
                    if(obj!=null)
                    {
                        obj.Parse(sr);
                        listOfObjects.Add(obj);
                    }
                }
                else if(s.Contains("LinksFrom")==true)
                {
                    Regex r1 = new Regex("LinksFrom\\(\\d+\\)=(\\w+)'(\\w+)'");
                    
                    Match match = r1.Match(s);
                    if(match.Success)
                    {
                        LinksFrom link = new LinksFrom();
                        link.linkClassName = match.Groups[1].Value;
                        link.linkActorName = match.Groups[2].Value;
                        
                        listOfLinksFrom.Add(link);
                    }
                    
                }
                else if(Regex.IsMatch(s,"Base=(\\w+'\\w+')"))
                {
                    Regex regex = new Regex("Base=(\\w+)'(\\w+)'");
                    Match match = regex.Match(s);
                    if(match.Success)
                    {
                        if(attach==null)
                            attach = new AttachTo();
                        attach.baseClassName = match.Groups[1].Value;
                        attach.baseActorName = match.Groups[2].Value;
                    }
                }
                else if(Regex.IsMatch(s,"BaseBoneName=\"(\\w+)\""))
                {
                    Regex regex = new Regex("BaseBoneName=\"(\\w+)\"");
                    Match match = regex.Match(s);
                    if(match.Success)
                    {
                        if(attach==null)
                            attach = new AttachTo();
                        attach.boneName = match.Groups[1].Value;
                        //Debug.Log(attach.boneName);
                    }
                }
                else if(Regex.IsMatch(s,string.Format("RelativeLocation={0}",UDKImporter.regexPatternCoords)))
                {
                    if(attach==null)
                        attach = new AttachTo();

                    UDKImporter.UDKPosition(Regex.Match(s,string.Format("RelativeLocation=({0})",UDKImporter.regexPatternCoords)).Groups[1].Value, ref attach.udkOffset);
                    //Debug.Log(attach.udkOffset);
                }
                else if(Regex.IsMatch(s,string.Format("RelativeRotation={0}",UDKImporter.regexPatternPitchYawRoll)))
                {
                    if(attach==null)
                        attach = new AttachTo();
                    
                    Regex regex = new Regex(string.Format("RelativeRotation=({0})",UDKImporter.regexPatternPitchYawRoll));
                    Match match = regex.Match(s);
                    if(match.Success)
                    {
                        if(attach==null)
                            attach = new AttachTo();
                        UDKImporter.UDKRotation( match.Groups[1].Value, ref attach.udkRotation);
                        //Debug.Log(attach.udkRotation);
                    }

                }
                else if(s.Contains("Connections"))
                {
                    Regex regex = new Regex("Connections\\((\\w+)\\)=\\((\\w+)=(\\w+)\\'(\\w+)\\',ConnectTo=(\\w+)\\'(\\w+)\\'\\)");

                    Match match = regex.Match(s);
                    if(match.Success)
                    {
                        Connection connection = new Connection();
                        connection.name = match.Groups[2].Value;
                        connection.fromObjectClassName = match.Groups[3].Value;
                        connection.fromObjectName = match.Groups[4].Value;
                        connection.toActorClassName = match.Groups[5].Value;
                        connection.toActorName = match.Groups[6].Value;
                        listOfConnections.Add(connection);
                    }
                    /*
                    string[] result = regex.Split(s);

                    int componentIndex = int.Parse(result[1]);
                    string componentClass = result[4];
                    string componentName = result[5];
                    string destComponentClass = result[9];
                    string destComponentName = result[10];

                    Debug.Log("Connection "+componentIndex+" "+componentClass+"'"+componentName+"' "+destComponentClass+"'"+destComponentName+"'");
                    */
                }
                else if(Regex.Match(s,string.Format("SplineActorTangent=({0})",UDKImporter.regexPatternCoords)).Success)
                {
                    UDKImporter.UDKPosition(Regex.Match(s,string.Format("SplineActorTangent=({0})",UDKImporter.regexPatternCoords)).Groups[1].Value, ref udkSplineActorTangent);
                    //Debug.Log(udkSplineActorTangent);
                }
            }
        }
    }

    private bool TryParseLocation(string s)
    {
        Regex regex = new Regex(UDKImporter.regexPatternLocation);
       
        Match match = regex.Match(s);
        if(match.Success)
        { 
            udkPosition.x = float.Parse(match.Groups[1].Value);
            udkPosition.y = float.Parse(match.Groups[2].Value);
            udkPosition.z = float.Parse(match.Groups[3].Value);
        }
        return match.Success;
    }

    private bool TryParseRotation(string s)
    {
        Regex regex = new Regex(UDKImporter.regexPatternRotation);
        Match match = regex.Match(s);
        if(match.Success)
        {
            udkRotation.x = float.Parse(match.Groups[1].Value);
            udkRotation.y = float.Parse(match.Groups[2].Value);
            udkRotation.z = float.Parse(match.Groups[3].Value);
        }
        return match.Success;
    }

    private bool TryParseDrawScale(string s)
    {
        Regex regex = new Regex(UDKImporter.regexPatternDrawScale);
        Match match = regex.Match(s);
        if(match.Success)
        {
            udkDrawScale = float.Parse(match.Groups[1].Value);
        }
        return match.Success;
    }
}