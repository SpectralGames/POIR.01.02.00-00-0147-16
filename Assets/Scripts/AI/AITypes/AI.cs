using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;


public abstract class AI : MonoBehaviour
{
    #region Actions
	public event Action<string> OnAttack;
	public void OnAttack_Invoke (string parameterName = "Attack")
    {
		OnAttack?.Invoke(parameterName);
    }

    public event Action OnDie;
    public void OnDie_Invoke ()
    {
		if(dieAudioClips.Length > 0)
		{
			int audioClipIndex = UnityEngine.Random.Range(0, dieAudioClips.Length);
			SoundManager.instance.PlaySceneEffect(dieAudioClips[audioClipIndex], gameObject.transform.position, -1f, 0f, 0.5f);
		}

        OnDie?.Invoke();
    }

    public event Action OnDamageRecorded = null;
    public void OnDamageRecorded_Invoke()
    {
        OnDamageRecorded?.Invoke();
    }

    public event Action OnTakeDamage;
    public void OnTakeDamage_Invoke ()
    {
        if (isAlive)
        {
			if(hitAudioClips.Length > 0)
			{
				int audioClipIndex = UnityEngine.Random.Range(0, hitAudioClips.Length);
				SoundManager.instance.PlaySceneEffect(hitAudioClips[audioClipIndex], gameObject.transform.position, -1f, 0f, 0.5f);
			}

            OnTakeDamage?.Invoke();
            //Debug.Log("invoke take dmg");
        }
    }

    public event Action OnPathEnd; // nawet chyba nie potrzebne
    public void OnPathEnd_Invoke ()
    {
        OnPathEnd?.Invoke();
    }

	public event Action OnTeleport;
	public void OnTeleport_Invoke()
	{
		OnTeleport?.Invoke();
	}

    #endregion Actions

    protected bool isInit = false;
    [HideInInspector]
    public bool isAlive = true;
    [HideInInspector]
    public bool isMoving = true;


    public int Level = 0;// { get; set; }

    protected NavigationType navigationType;
    public INavigation NavigationController
    {
        get; set;
    }

    public virtual void SetSpeedFactor (float newSpeedFactor)
    {
        this.speedFactor = newSpeedFactor;
    }

	public virtual void SetSpeedFactorFromAnimation (float newSpeedFactor)
	{
		this.speedFactorFromAnimation = newSpeedFactor;
	}

    public float walkSpeed = 1f;
    public float additionalSpeedToRun = 0.5f;
    [HideInInspector]
    public float speedFactor = 1f;
	[HideInInspector]
	public float speedFactorFromAnimation = 1f;

    public float navMeshAgentRadius = 0.25f; // przeniesc do innej klasy

    public float timeForAttackEvent; //kiedy ma odpalic attack
    [SerializeField]
    protected AIAnimator animator;// = new EnemyAnimator();
    public AIAnimator Animator { get { return animator; } }

    protected GameObject baseMesh;
    protected Collider baseCollider;
    public float GetAIHeight ()
    {
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        float enemyHeight = 0.0f;

		if (skinnedMeshRenderer != null && baseMesh == null)
            baseMesh = skinnedMeshRenderer.gameObject;

        if (baseMesh != null)
            enemyHeight = baseMesh.GetComponent<SkinnedMeshRenderer>().bounds.extents.y * 1.5f;

        baseCollider = this.GetComponent<Collider>();
        return enemyHeight;
    }


    protected virtual IEnumerator OnStartTrashing ()
    {
        yield return new WaitForSeconds(4.5f);
        yield return Trashing();
    }

    protected virtual IEnumerator OnStartTrashing (float dieTime)
    {
        yield return new WaitForSeconds(dieTime);
        yield return Trashing();
    }

    protected virtual IEnumerator Trashing()
    {
        yield return new WaitForSeconds(0.4f);

        float initScaleFactor = transform.localScale.x;
        float trashingTime = 0.25f;
        while (trashingTime > 0f)
        {
            trashingTime -= Time.deltaTime;
			transform.localScale = new Vector3(initScaleFactor, initScaleFactor * trashingTime * 4f, initScaleFactor);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }

	public AudioClip[] attackAudioClips;
	public AudioClip[] hitAudioClips;
	public AudioClip[] dieAudioClips;
	public AudioClip[] moveAudioClips;

    protected virtual void OnDrawGizmos()
    {
    }
}