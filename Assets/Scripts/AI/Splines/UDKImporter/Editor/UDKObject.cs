using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System;

public class UDKObject
{
    public string name;

    private static string patternBeginActorObject = "Begin Object Class=(\\w+) Name=(\\w+) ObjName=(\\w+)";

    private static Dictionary<string,Type> dictionaryOfObjects = new Dictionary<string, Type>()
    {
        {"SplineComponent", typeof(UDKObjectSplineComponent)}
    };

    public UDKObject(string s)
    {

    }

    public virtual void Parse(StreamReader reader)
    {
        string s="";
        while((s=reader.ReadLine())!=null)
        {
            if(s.Contains("End Object"))
            {
                break;
            }
            else
                Parse(s);
        }
    }

    protected virtual bool Parse(string s)
    {
        return false;
    }

    public static UDKObject CreateObject(string s)
    {
        Regex regex = new Regex(patternBeginActorObject);

        Match match = regex.Match(s);
        if(match.Success)
        {
            string className = match.Groups[1].Value;
           
            if(dictionaryOfObjects.ContainsKey(className))
            {
                Type t = dictionaryOfObjects[className];

                UDKObject obj = (UDKObject)Activator.CreateInstance(t,new object[]{s});
                return obj;
            }
        }

        return null;
    }

    public static bool IsBeginObject(string s)
    {
        Regex regex = new Regex(patternBeginActorObject);
        if(regex.Match(s).Success)
            return true;
        return false;
    }
}

public class UDKObjectSplineComponent : UDKObject
{
    public static string splineComponentName = "SplineComponent";
    public class Point1
    {
        public Vector3 outVal;
        public Vector3 arriveTangent;
        public Vector3 leaveTangent;
        public int intepMode;
    }

    public class Point2 : Point1
    {
        public float inVal;
    }

    public class SplineInfo
    {
        public Point1 point1 = new Point1();
        public Point2 point2 = new Point2();
    }

    public SplineInfo splineInfo=new SplineInfo();
   
    public UDKObjectSplineComponent(string s):base(s)
    {

    }

    protected override bool Parse(string s)
    {
        if(base.Parse(s)==false)
        {
            string patternFloat = UDKImporter.regexPatternFloat;

            string patternCoords = UDKImporter.regexPatternCoords;
            string patternCustom = "(\\w+)";

            /*
            SplineInfo=(
                Points=(
                    (
                        OutVal=(X=374.224487,Y=-648.881958,Z=1273.719116),
                        ArriveTangent=(X=-87.965714,Y=-185.768951,Z=-290.820007),
                        LeaveTangent=(X=-87.965714,Y=-185.768951,Z=-290.820007),
                        InterpMode=CIM_CurveUser
                    )
                    ,
                    (
                        InVal=1.000000,
                        OutVal=(X=207.279953,Y=-812.387268,Z=853.159668),
                        ArriveTangent=(X=-229.380508,Y=-82.370979,Z=0.000000),
                        LeaveTangent=(X=-229.380508,Y=-82.370979,Z=0.000000),
                        InterpMode=CIM_CurveUser)
                    )
              )*/

            string pattern = String.Format("SplineInfo=\\(Points=\\(\\(OutVal=({0}),ArriveTangent=({0}),LeaveTangent=({0}),InterpMode={1}\\),\\(InVal={2},OutVal=({0}),ArriveTangent=({0}),LeaveTangent=({0}),InterpMode={1}\\)\\)\\)",patternCoords,patternCustom,patternFloat);

            Regex regex = new Regex(pattern);
            Match match = regex.Match(s);
            if(match.Success)
            {
                //int d = 0;

                if( !(
                    UDKImporter.UDKPosition( match.Groups[1].Value, ref splineInfo.point1.outVal ) && 
                    UDKImporter.UDKPosition(match.Groups[5].Value,ref splineInfo.point1.arriveTangent) && 
                    UDKImporter.UDKPosition(match.Groups[9].Value,ref splineInfo.point1.leaveTangent) && 
                    UDKImporter.UDKPosition(match.Groups[15].Value,ref splineInfo.point2.outVal) &&
                    UDKImporter.UDKPosition(match.Groups[19].Value,ref splineInfo.point2.arriveTangent) &&
                    UDKImporter.UDKPosition(match.Groups[23].Value, ref splineInfo.point2.leaveTangent) )
                   )
                   Debug.Log("Failed to import splineInfo");


            }
                /*
                SplineInfo=(Points=((OutVal=(X=1538.595947,Y=285.271576,Z=119.630089),ArriveTangent=(X=-260.113861,Y=-149.468277,Z=0.000000),LeaveTangent=(X=-260.113861,Y=-149.468277,Z=0.000000),InterpMode=CIM_CurveUser),(InVal=1.000000,OutVal=(X=586.477661,Y=-261.840576,Z=734.915649),ArriveTangent=(X=-1072.667236,Y=-149.468323,Z=18.787354),LeaveTangent=(X=-1072.667236,Y=-149.468323,Z=18.787354),InterpMode=CIM_CurveUser)))
            SplineCurviness=0.984250
            SplineColor=(B=255,G=0,R=255,A=250)
            SplineReparamTable=(Points=(,(InVal=57.117077,OutVal=0.111111),(InVal=161.493271,OutVal=0.222222),(InVal=302.524628,OutVal=0.333333),(InVal=468.311096,OutVal=0.444444),(InVal=646.926636,OutVal=0.555556),(InVal=826.800415,OutVal=0.666667),(InVal=997.195679,OutVal=0.777778),(InVal=1149.270264,OutVal=0.888889),(InVal=1278.886475,OutVal=1.000000)))
            ReplacementPrimitive=None
            LightingChannels=(bInitialized=True,Dynamic=True)
            Name="SplineComponent_1"
            ObjectArchetype=SplineComponent'Engine.Default__SplineComponent'
            */
        }
        return true;
    }
}