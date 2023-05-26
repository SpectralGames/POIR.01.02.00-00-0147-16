using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VirginTower : MonoBehaviour
{
    /// <summary>
    /// punkt srodka wiezy, miejsce do ktorego bedzie uciekala dziewica
    /// </summary>
    public Transform centralPoint;

    /// <summary>
    /// bedzie mial odpowednia sciezke do wiezy na pierto i na balkon
    /// </summary>
    public Pathpoint towerPathPoint;

    /// <summary>
    /// miejsce w pokoju do ktorego sie teleportuje dziewica z central Pointa
    /// </summary>
    public Transform roomPoint;

    /// <summary>
    /// miejsce do ktorego przechodzi dziewica, z rooma, np otwierajac drzwi
    /// </summary>
    public Transform standPoint;

    /// <summary>
    /// czy dziewica zostala porwana
    /// </summary>
    //public bool isVirginInTower = true; // juz nie potrzebne

    /// <summary>
    /// dziewica do ktorej nalezy ten tower
    /// </summary>
    //public Virgin virgin; // lista

    public List<Virgin> virgins = new List<Virgin>(); // pobieran z object pool lista powinna byc

    public List<GameObject> virginLights = new List<GameObject>();

    private void Start ()
    {
        InitVirgins();
        //virgins = GameObject.FindObjectsOfType<Virgin>().ToList();
        //virgin.Init(this);
        //virgin.OnCapture += OnCaptureVirgin;
    }

    private void InitVirgins()
    {
        foreach (Virgin v in virgins)
        {
            v.Init(this);
            v.OnArrivedToTowerCallback.AddListener(OnVirginStateChange);
            v.OnCaptureCallback.AddListener(OnVirginStateChange);
        }
    }

    public Virgin TakeAvailableVirgin()
    {
        List<Virgin> tmp = virgins.FindAll(x => x.IsAvailableToCapture());

        int random = UnityEngine.Random.Range(0, tmp.Count);
        Virgin v = tmp[random];

        return v;
    }

    // sprawdz czy jest jakas dziewica w wiezy
    public bool IsTowerEmpty()
    {
        List<Virgin> tmp = virgins.FindAll(x => x.IsAvailableToCapture());
        return tmp.Count <= 0;
    }

    private void OnVirginStateChange()
    {
        for(int i = 0; i <  virgins.Count;i++)
        {
            if(virginLights[i] != null)
                 virginLights[i].SetActive(virgins[i].IsAvailableToCapture());
        }
    }
}