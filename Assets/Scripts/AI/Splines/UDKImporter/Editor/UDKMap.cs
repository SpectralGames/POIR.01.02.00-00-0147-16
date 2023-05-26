using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class UDKMap
{
    public List<UDKLevel> levels=new List<UDKLevel>();
    
    public void Parse(StreamReader sr)
    {
        string s = "";
        while((s=sr.ReadLine())!=null)
        {
            if(s.Contains("End Level"))
            {
                break;
            }
            else if(s.Contains("Begin Level"))
            {
                UDKLevel level = new UDKLevel(s);
                level.Parse(sr);
                levels.Add(level);
            }
        }
    }
    
    public UDKMap(string s)
    {
        
        
    }
}