#define USE_NEW_TIMERS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseMonoBehaviour : MonoBehaviour
{
    Transform mTrans;
    //public Transform cachedTransform { get { return mTrans; } }    
    public Transform cachedTransform { get { if( mTrans == null ) mTrans = transform; return mTrans; } }

    #region MonoBehaviour virtual methods
    protected virtual void Awake()
    {
        mTrans = transform;
        DefaultProperties();
    }

    protected virtual void Start()
    { }

    protected virtual void Update()
    {
#if !USE_NEW_TIMERS
        UpdateTimers();
#endif
    }

    protected virtual void OnEnable()
    { }

    protected virtual void OnDisable()
    { }

    protected virtual void OnDestroy()
    { }
    #endregion

    #region UnrealStyle
    protected virtual void DefaultProperties()
    { }
    #endregion


    #region Timers
    public class _Timer
    {
        public System.Action Function;
        public float fStartTime;
        public float fPauseTime;
        public float fRate;
        public bool bLoop;
    }

    private List<_Timer> PendingActivateTimers = new List<_Timer>();
    private List<_Timer> PendingDeactivateTimers = new List<_Timer>();
    private List<_Timer> PendingUnPauseTimers = new List<_Timer>();

    public List<_Timer> Timers = new List<_Timer>();
    private bool bTimerRunning = false;

    // Timer Evaluation Loop
    private IEnumerator TimerEvaluation()
    {
        //Debug.Log("TimerEvaluation() - Begin");
        bTimerRunning = true;
        while( bTimerRunning )
        {
            //yield return new WaitForEndOfFrame();
            // this line allocated 16Bytes every frame 
            // and I am not sure if we really need to wait for end of frame
            // with this line commented timer will trigger right after Update()
            
            //localy cache GameplayTime.TimeSinceLevelLoad
			float timeSinceLevelLoad = Time.timeSinceLevelLoad;


            //check pending deactivate timers first
            if( PendingDeactivateTimers.Count > 0 )
            {
                for( int Idx = 0; Idx < PendingDeactivateTimers.Count; Idx++ )
                {
                    //Debug.Log("Removing Timer: " + PendingDeactivateTimers[Idx].Function.Method.ToString());
                    Timers.Remove(PendingDeactivateTimers[Idx]);
                }
                PendingDeactivateTimers.Clear();
            }

            //check if we need to add new timers
            if( PendingActivateTimers.Count > 0 )
            {
                for( int Idx = 0; Idx < PendingActivateTimers.Count; Idx++ )
                {
                    //Debug.Log("Adding Timer: " + PendingActivateTimers[Idx].Function.Method.ToString());
                    Timers.Add(PendingActivateTimers[Idx]);
                }
                PendingActivateTimers.Clear();
            }

            //check if we want to unpause any timer
            if( PendingUnPauseTimers.Count > 0 )
            {
                //if we unpause timer compensate timers starttime with puase interval
                for( int Idx = 0; Idx < PendingUnPauseTimers.Count; Idx++ )
                {
                    _Timer Timer = PendingUnPauseTimers[Idx];
                    //Debug.Log("Unpausing Timer: " + Timer.Function.Method + " with oldstarttime: " + Timer.fStartTime + " and newstarttime: " + (Timer.fStartTime + (GameplayTime.TimeSinceLevelLoad - Timer.fPauseTime)));
                    Timer.fStartTime = Timer.fStartTime + ( timeSinceLevelLoad - Timer.fPauseTime );
                    Timer.fPauseTime = 0f;
                }
                PendingUnPauseTimers.Clear();
            }

            //iterate throught each timer and check if it should be called
            for( int Idx = 0; Idx < Timers.Count; Idx++ )
            {
                _Timer Timer = Timers[Idx];

                //if timer is not paused
                if( Timer.fPauseTime <= 0f )
                {
                    if( !Timer.bLoop )
                    {
                        //if not loop and time is right call function and add timer to pendingdeactivate list
                        if( Timer.fStartTime + Timer.fRate < timeSinceLevelLoad )
                        {
                            PendingDeactivateTimers.Add(Timer);
                            Timer.Function();
                        }
                    }
                    else
                    {
                        //if loop and time is right call function and reset timer
                        if( Timer.fStartTime + Timer.fRate < timeSinceLevelLoad)
                        {
                            //Debug.Log("Calling delegate to: " + Timer.Function.Method);
                            Timer.Function();
                            Timer.fStartTime = timeSinceLevelLoad;
                        }
                    }
                }
            }

            // if all timers finished mark exit coroutine
            if( Timers.Count == 0 )
            {
                bTimerRunning = false;
            }

            yield return null;
        }
        //Debug.Log("TimerEvaluation() - End");
    }

    // Start Timers
    public void SetTimer(System.Action Function, float Rate, bool bLoop = false)
    {
        // bail if function not specified or timer is currently running and not marked as deactivated
        // or if we already have this timer marked as pendingactivate
        if( Function == null
            || Timers.Find(s => s.Function == Function) != null
               && PendingDeactivateTimers.Find(s => s.Function == Function) == null
            || PendingActivateTimers.Find(s => s.Function == Function) != null )
        {
            return;
        }

        //Debug.Log("SetTimer - function: " + Function.Method.ToString());

        _Timer t = new _Timer();
        t.Function = Function;
        t.bLoop = bLoop;
		t.fStartTime = Time.timeSinceLevelLoad;
        t.fRate = Rate;
        t.fPauseTime = 0f;

        PendingActivateTimers.Add(t);

        //if coroutine is not already running start it
        if( !bTimerRunning )
        {
            StartCoroutine(TimerEvaluation());
        }
    }

    // Stops timer
    public void ClearTimer(System.Action Function)
    {
        //Debug.Log("ClearTimer - function: " + Function.Method.ToString());
        _Timer t = Timers.Find(s => s.Function == Function);
        if( t != null )
        {
            PendingDeactivateTimers.Add(t);
        }

        t = PendingActivateTimers.Find(s => s.Function == Function);
        if( t != null )
        {
            PendingActivateTimers.Remove(t);
        }
    }

    public void ClearAllTimers()
    {
        foreach( var Timer in Timers )
        {
            PendingDeactivateTimers.Add(Timer);
        }
        PendingActivateTimers.Clear();
    }

    public void PauseTimer(System.Action Function, bool bPause)
    {
        _Timer t = Timers.Find(s => s.Function == Function);
        if( t != null )
        {
            //if we pausing register pause timemarker
            if( bPause && t.fPauseTime <= 0 )
            {
				t.fPauseTime = Time.timeSinceLevelLoad;
                //Debug.Log("PauseTimer - function: " + Function.Method.ToString() + " with starttime: " + t.fStartTime + " pausetime: " + t.fPauseTime);
            }
            else if( !bPause && t.fPauseTime > 0 )
            {
                _Timer tp = PendingUnPauseTimers.Find(s => s.Function == Function);
                if( tp == null )
                {
                    PendingUnPauseTimers.Add(t);
                }
            }
        }
    }

    // Returs TRUE if timer is running or is paused
    public bool IsTimerActive(System.Action Function)
    {
        return ( Timers.Find(s => s.Function == Function) != null );
    }

    public bool IsTimerPaused(System.Action Function)
    {
        _Timer t = Timers.Find(s => s.Function == Function);
        return ( t != null && t.fPauseTime > 0f );
    }

    // Gets timer Rate
    public float GetTimerDuration(System.Action Function)
    {
        _Timer t = Timers.Find(s => s.Function == Function);
        if( t != null )
        {
            return t.fRate;
        }

        return -1f;
    }

    // Gets timer current running time
    public float GetTimerCount(System.Action Function)
    {
        _Timer t = Timers.Find(s => s.Function == Function);
        if( t != null )
        {
			return Time.timeSinceLevelLoad - t.fStartTime;
        }
        return -1f;
    }

    //Gets remaining time for timer
    public float GetTimerRemainingTime(System.Action Function)
    {
        _Timer t = Timers.Find(s => s.Function == Function);
        if( t != null )
        {
			return t.fRate - ( Time.timeSinceLevelLoad - t.fStartTime );
        }
        return -1f;
    }

    #endregion //Timers

    #region TimersKuba

    public class Timer
    {
        public bool isValid = true;
        public float weight=0.0f;
    }

    private class TimerHelper
    {
        public float currentTime=0;
        public float delay=0;
        public System.Action actionToCall;
        public int count;
        public Timer timer;
    }

    Dictionary<Timer,TimerHelper> dictionaryOfTimers = new Dictionary<Timer, TimerHelper>();
    List<Timer> timersToRemove = new List<Timer>();
    List<TimerHelper> timersToAdd = new List<TimerHelper>();

    /// <summary>
    /// Starts the timer.
    /// </summary>
    /// <returns>The timer.</returns>
    /// <param name="action">Action.</param>
    /// <param name="delay">Delay.</param>
    /// <param name="count">Count.</param>
    public Timer StartTimer(System.Action action, float delay, int count)
    {
        TimerHelper helper = null;
        foreach(KeyValuePair<Timer,TimerHelper> pair in dictionaryOfTimers)
        {
            if(pair.Value.actionToCall==action)
            {
                helper = pair.Value;
                break;
            }
        }

        if (helper != null)
        {
            #if !FINAL_RELEASE
            Debug.LogWarning("Timer exists, will be override " + helper.actionToCall.ToString());
            #endif

            dictionaryOfTimers.Remove(helper.timer);
            helper.timer.isValid = false;
        }

        helper = new TimerHelper();
        helper.delay = delay;
        helper.actionToCall = action;
        helper.currentTime = 0;
        helper.count = count;
        helper.timer = new Timer();

        timersToAdd.Add(helper);

        //dictionaryOfTimers[helper.timer] = helper;

        return helper.timer;
    }

    /// <summary>
    /// Stops the timer.
    /// </summary>
    /// <param name="timer">Timer to stop.</param>
    public void StopTimer(Timer timer)
    {
        if (timer != null)
        {
            dictionaryOfTimers.Remove(timer);
            timer.isValid = false;
        }
    }

    /// <summary>
    /// Updates the timers. You have to call it manually
    /// </summary>
    protected void UpdateTimers()
    {
        foreach (TimerHelper h in timersToAdd)
        {
            dictionaryOfTimers[h.timer]=h;
        }
        timersToAdd.Clear();

        foreach(KeyValuePair<Timer,TimerHelper> keyPair in dictionaryOfTimers)
        {
            TimerHelper timer = keyPair.Value;
			timer.currentTime += Time.timeSinceLevelLoad;
            if(timer.currentTime>=timer.delay && timer.count>0)
            {
                timer.currentTime-=timer.delay;
                timer.timer.weight = 1.0f;
                timer.actionToCall();

                timer.count-=1;
                if(timer.count==0)
                {
                    timer.timer.isValid = false;
                    timersToRemove.Add (keyPair.Key);
                }
            }
            else
            {
                timer.timer.weight = timer.currentTime/timer.delay;
            }
        }

        foreach(Timer a in timersToRemove)
        {
            dictionaryOfTimers.Remove(a);
        }
        timersToRemove.Clear();
    }
    #endregion
}
