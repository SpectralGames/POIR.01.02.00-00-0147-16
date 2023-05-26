using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Virgin : AI
{
    public UnityEvent OnCaptureCallback = new UnityEvent();
    public UnityEvent OnDropCallback = new UnityEvent();
    public UnityEvent OnArrivedToTowerCallback = new UnityEvent();

    #region Members
    //[SerializeField] private VirginTower virginTower;
    //private Transform parent;

    public bool isFat = false;
		
    public event Action OnCapture;
    public void OnCapture_Invoke ()
    {
        OnCapture?.Invoke();
        OnCaptureCallback.Invoke();
        Debug.Log("OnCapture Invoke");
    }

    public event Action OnDrop;
    public void OnDrop_Invoke ()
    {
        OnDropCallback.Invoke();
        OnDrop?.Invoke();
    }

    public event Action OnRun;
    public void OnRun_Invoke()
    {
        OnRun?.Invoke();
    }

	public GameObject catchMarker;
    public VirginAct Act; // behaviour
    //private Transform b_root; // offset
    #endregion

    #region Constructors
    public Virgin ()
    {

    }
    public void Init (VirginTower vt)
    {
        Act = new VirginAct(this);
        

        //parent = GameObject.Find("Enemies").transform;
        //this.virginTower = vt;
        //aiController = new AIVirginNavMeshController();
        animator = new VirginAnimator(this);

        NavigationController = new AIVirginNavMesh(this, CreateStartPoint());
        OnCapture += CaptureAct;
        OnDrop += DropAct;
		OnTeleport += Teleport;
        //OnRun += StartRunningVirgin;

        //VirginOffset();
        //b_root = this.gameObject.FindComponentInChildWithName<Transform>("b_root");
        //Debug.Log("Virgin offset " + b_root.localPosition);
    }

    private Pathpoint CreateStartPoint ()
    {
        Pathpoint startPoint = new GameObject().AddComponent<Pathpoint>();
        startPoint.transform.position = this.transform.position;
        startPoint.name = this.gameObject.name + "_StartPathPoint";
        return startPoint;
    }
    #endregion Constructors

    #region Logic
    private void Update ()
    {
        Act?.Tick();
        NavigationController?.Tick();
    }


    public void ArrivedToTower ()
    {
        //isSitting = false;
        Act.IsFalling = false;
        Act.IsRunning = false;
        Act.Captured = false;
		Act.ArrivedToTower ();
        // runTimer = 0.0f;
        OnArrivedToTowerCallback.Invoke();
        NavigationController.Disable();
    }

    private void PlayCaptureAnimation () //
    {
        animator.SetAnimatorBool("bCaptured", true);
        Animator.PlayAnimation("catch");

        Debug.Log("Captured Virgin");
    }

    public void PlayBalconyAnimation () // private zeobic poprzez jakas akcje
    {
        animator.SetAnimatorBool("bCaptured", false);
        animator.SetAnimatorTrigger("Balcony");
    }

    public void PlayFlyAnimation()
    {
        animator.SetAnimatorBool("bCaptured", false);
        animator.SetAnimatorBool("Fly", true);
        animator.PlayAnimation("fly");
    }

    public void SetParent (Enemy enemy)
    {
        foreach(var pivot in enemy.carryPivotList)
        {
            if (pivot.childCount == 0)
            {
                this.gameObject.transform.SetParent(pivot);
                break;
            }
        }
      
    }


    private void CaptureAct ()
    {
        animator.SetAnimatorBool("Fly", false);
        //animator.PlayAnimation("fly");
        // i play animaton od razu
        NavigationController?.Disable();
        PlayCaptureAnimation();
        AttachToEnemy();
        //parachute.CutParachute();
    }

    public void StartRunning()
    {
		if (isFat) {
			Animator.PlayAnimation ("fatRun");
			Animator.SetAnimatorBool("isFatRun", true);
		} else {
			Animator.PlayAnimation ("run");
			Animator.SetAnimatorBool("isFatRun", false);
		}
		
        Animator.SetAnimatorBool("bCaptured", false);
        Animator.SetAnimatorBool("Fly", false);
        NavigationController.Enable();
		NavigationController.GetBackPosition();
    }

    private void AttachToEnemy ()
    {
        this.transform.localPosition = Vector3.zero;
        this.transform.rotation =  new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
    }

    private void DropAct ()
    {
        UnattachFromEnemy();
    }
	public void OnCatchEvent()
	{
		if(moveAudioClips.Length > 0)
		{			
			int audioClipIndex = UnityEngine.Random.Range(0, moveAudioClips.Length);
			SoundManager.instance.PlaySceneEffect(moveAudioClips[audioClipIndex], gameObject.transform.position, -1f, 0f, 0.5f);
		}
	}
		
    private void UnattachFromEnemy ()
    {
        //parent = GameObject.Find("Enemies").transform;
		this.transform.SetParent(null, true);
        this.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        animator.SetAnimatorBool("bCapture", false);
    }
		
	public void Teleport()
	{
		Act.Captured = true;
		isAlive = false;
		ObjectPool.Instance.RemoveVirgin(this);
		Destroy(this.gameObject);
	}

    public bool IsAvailableToCapture()
    {
        return Act.InTower && isAlive;
    }
    #endregion
}