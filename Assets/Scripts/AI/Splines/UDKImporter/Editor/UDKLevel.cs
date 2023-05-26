using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class UDKLevel
{
    public List<UDKActor> actors = new List<UDKActor>();
    
    public UDKLevel(string s)
    {
        
    }
    
    public void Parse(StreamReader sr)
    {
        string s = "";
        while((s=sr.ReadLine())!=null)
        {
            if(s.Contains("End Level"))
            {
                break;
            }
            else if(s.Contains("Begin Actor"))
            {
                UDKActor actor = new UDKActor(s);
                actor.Parse(sr);
                actors.Add(actor);
            }
        }
    }

    public UDKActor GetActorByName(string name, out int actorNumber)
    {
        int num = 0;
        foreach(UDKActor a in actors)
        {
            if(a.name==name)
            {
                actorNumber = num;
                return a;
            }
            num+=1;
        }
        actorNumber = -1;
        return null;
    }
}
