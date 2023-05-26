using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VirginParachute : MonoBehaviour
{
    [SerializeField] private GameObject parachute;
    //[SerializeField] private float gravity = 9.8f;

    private Virgin virgin;
    private const float OPEN_TIME_DURATION = 1.0f;
    private float timer = 0.0f;
    public bool Open
    {
        get; private set;
    }

    public void Init (Virgin virgin)
    {
        Open = false;
        this.virgin = virgin;

        ParachuteInit();

    //    Debug.Log("Parachute Init success");
    }

    private void ParachuteInit ()
    {
        if (parachute == null)
            parachute = this.gameObject.FindComponentInChildWithName<Transform>("Parachute").gameObject;
        if (parachute == null)
            Debug.LogError("NIE MA SPADOCHRONU U DZIEWICY !!!");

        parachute.transform.localScale = Vector3.zero;
        parachute.SetActive(false);
    }

    public void OpenParachute ()
    {
        parachute.SetActive(true);
        parachute.transform.localScale = Vector3.zero;
        Open = true;
        timer = 0.0f;
    }

    public void CutParachute ()
    {
        parachute.transform.localScale = Vector3.zero;
        parachute.SetActive(false);
        Open = false;
    }

    private void Update ()
    {
        if (!Open)
            return;

        if (!virgin.isAlive && !virgin.Act.Captured)
            return;

        timer += Time.deltaTime;
        if (timer <= OPEN_TIME_DURATION)
        {
            float factor = Globals.Easing(timer / OPEN_TIME_DURATION);
            parachute.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, factor);
        }
        else
        {
            parachute.transform.localScale = Vector3.one;
            Open = false;
        }
    }
}