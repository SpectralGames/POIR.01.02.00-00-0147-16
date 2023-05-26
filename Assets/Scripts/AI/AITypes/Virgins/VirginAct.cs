using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class VirginAct
{
    private Virgin virgin;
    private VirginParachute parachute;

    private float timer = 0.0f; // aktualny czas spadania
    private Vector3 placeToFall; // wyliczane w virgin, reszta w tej kupce do parachute?
    private float currentGravity;
    [SerializeField] private float gravity = 9.8f;
    private const float PARACHUTE_OPEN_HEIGHT = 2.0f;

    public bool Captured { get; set; }
    public bool InTower { get; set; }
    public bool IsFalling { get; set; }
    public bool IsRunning { get; set; }

    //private VirginFall fall;

    public VirginAct(Virgin virgin)
    {
        this.virgin = virgin;

        parachute = virgin.transform.GetComponent<VirginParachute>();
        if (parachute != null)
            parachute.Init(virgin);

        IsFalling = false;
        IsRunning = false;
        Captured = false;
        InTower = true;

        //fall = new VirginFall(virgin);

        virgin.OnCapture += Capture;
        virgin.OnDrop += Drop;
        virgin.OnDie += Die;
    }

    public void Tick()
    {
        Fall();
		if (InTower == false) {
			virgin.catchMarker.transform.rotation = Quaternion.identity;
			if (IsRunning) {
				virgin.catchMarker.transform.position = virgin.transform.position + Vector3.up * 3;
			} else {
				virgin.catchMarker.transform.position = virgin.transform.position + Vector3.up * 2;
			}
		}

    }

    private void Capture()
    {
        Captured = true;
        InTower = false;
        IsFalling = false;
        IsRunning = false;
		virgin.catchMarker.SetActive (true);
        parachute.CutParachute();
    }

    public void Fall ()
    {
        if (!IsFalling)
            return;

        Debug.DrawLine(virgin.transform.position, placeToFall, Color.cyan);

        float distance = Vector3.Distance(virgin.transform.position, placeToFall);

        if (distance > 0.01f &&
            virgin.transform.position.y > placeToFall.y)
        {
            InTheAir();
        }
        else
        {
            OnTheGround();
        }
    }

    private void InTheAir ()
    {
        timer += Time.deltaTime;
        float velocity = currentGravity * timer;

        virgin.transform.position = Vector3.MoveTowards(virgin.transform.position, placeToFall, velocity * Time.deltaTime);
    }

    private void OnTheGround ()
    {
        virgin.transform.position = placeToFall;
        parachute.CutParachute();
        IsRunning = true;

        StartRunningVirgin();

        IsFalling = false;
        timer = 0.0f;
    }

    private void StartRunningVirgin ()
    {
        virgin.transform.position = placeToFall;
        parachute.CutParachute();

        IsRunning = true;
        IsFalling = false;

		Debug.Log ("dziewica wyladowała, zacznij biegać");
        virgin.StartRunning();
    }

    private void Drop()
    {
        Captured = false;
        IsFalling = true;
        InTower = false;
        IsRunning = false;

        FindPlaceToDrop();
        //CreateDropSign();
    }

    private void FindPlaceToDrop()
    {
        Vector3 result;
        if (FindNearestPlaceOnNavMesh(virgin.transform.position, 100.0f, out result))
        {
            // znaleziono w danej odleglosci punkt na navmeshu
            placeToFall = result;
            Debug.Log("virgin fall hight: " + Vector3.Distance(virgin.transform.position, placeToFall));
            float height = Vector3.Distance(virgin.transform.position, placeToFall);
            if (height >= PARACHUTE_OPEN_HEIGHT)
            {
                OpenParachute(height);
            }
            else
            {
                currentGravity = gravity;
            }
        }
        else
            Debug.Log("vrgiin nie ma miejsca gdzie upasc: " + virgin.transform.position);
    }

    private void OpenParachute (float height)
    {
        // otworz spadochron, zmniejsz grawitacje
        currentGravity = gravity / 4;
        virgin.PlayFlyAnimation();
        parachute.OpenParachute();
        Debug.Log("VRIGIN SPADOCHRON !!!!: height: " + height);
    }

    private bool FindNearestPlaceOnNavMesh (Vector3 center, float range, out Vector3 result)
    {
        int mask = -1 << NavMesh.GetAreaFromName("Walkable");
        NavMeshHit hit;

        if (NavMesh.SamplePosition(center, out hit, range, mask))
        {
            result = hit.position;
            Debug.Log(virgin.name + ", virgin hit mask: " + hit.mask);
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    private void CreateDropSign ()
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.transform.position = placeToFall;
        g.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

	public void ArrivedToTower(){
		InTower = true;
		virgin.catchMarker.SetActive (false);
	}

    private void Die()
    {
        Captured = true;
        virgin.isAlive = false;
    }
}