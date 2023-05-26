using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance { get; private set; }
    public AudioClip ambientClip;
    public AudioClip[] inGameAudioClips;
    public AudioClip[] victoryLvlAudioClips;
    public AudioClip[] failLvlAudioClips;
    public AudioClip[] newHideoutAudioClips;
    public AudioClip[] starsLvlFinishAudioClips;
    public AudioClip[] unlockAudioClips; 
    public AudioClip[] effectClips;

	private AudioSource[] sceneEffectsSources;
	private AudioSource[] hudEffectsSources;
	private AudioSource[] ambientSource;
	private AudioSource queueSource;

	private float ambientVolume, effectsVolume, hudVolume;

	private int queueIndex = 0;
	public AudioClip[] queueAudioClips;
	void Awake ()
	{
		instance = this;
		ambientVolume = PlayerPrefs.GetFloat ("Settings.AmbientVolume", 1f) * 0.1f;
		effectsVolume = PlayerPrefs.GetFloat ("Settings.EffectsVolume", 1f) * 0.8f;
		hudVolume = PlayerPrefs.GetFloat ("Settings.HudVolume", 1f) * 0.35f;
		
		SetAmbientAudioSource ();
		SetSceneEffectsAudioSources ();
		SetHudEffectsAudioSources ();
		SetQueueAudioSource ();


        PlayGameMusic();
	}
    void Start()
    {
        // DontDestroyOnLoad(transform.gameObject);
    }

	void SetAmbientAudioSource ()
	{
        ambientSource = new AudioSource[2];
        for(int i = 0; i < 2; i++)
        {
    		GameObject ambientGameObject = new GameObject ("ambientGameObject");
    		ambientGameObject.AddComponent<AudioSource> ();
    		ambientGameObject.transform.parent = this.transform;
    		ambientSource[i] = ambientGameObject.GetComponent<AudioSource> ();
		
            ambientSource[i].playOnAwake = false;
            ambientSource[i].spatialBlend = 0f;
            ambientSource[i].bypassReverbZones = true;
            ambientSource[i].volume = ambientVolume;
            ambientSource[i].loop = true;
        }
	}

    public void ChangeAmbientVolume(float newValue)
    {
		PlayerPrefs.SetFloat("Settings.AmbientVolume", newValue);
		ambientVolume = newValue * 0.5f;

        foreach(AudioSource source in ambientSource)
        {
			source.volume = ambientVolume;
        }
    }

    public float getAmbientVolume()
    {
		return PlayerPrefs.GetFloat ("Settings.AmbientVolume", 1f);
    }
    public float getEffectsVolume()
    {
		return PlayerPrefs.GetFloat ("Settings.EffectsVolume", 1f);
    }

	
	void SetSceneEffectsAudioSources ()
	{
		sceneEffectsSources = new AudioSource[10];
		for (int i=0; i<sceneEffectsSources.Length; i++) {
			GameObject effectGameObject = new GameObject ("sceneEffect" + i + "GameObject");
			effectGameObject.AddComponent<AudioSource> ();
			effectGameObject.transform.parent = this.transform;
			sceneEffectsSources [i] = effectGameObject.GetComponent<AudioSource> ();
			
			sceneEffectsSources [i].playOnAwake = false;
			sceneEffectsSources [i].spatialBlend = 0.95f;
			sceneEffectsSources [i].dopplerLevel = 0.0f;
			sceneEffectsSources [i].rolloffMode = AudioRolloffMode.Linear;
			sceneEffectsSources [i].panStereo = 0.0f;
			sceneEffectsSources [i].spread = 80f;
			sceneEffectsSources [i].volume = effectsVolume;
			sceneEffectsSources [i].reverbZoneMix = 0f;
			sceneEffectsSources [i].minDistance = 1f;
			sceneEffectsSources [i].maxDistance = 10f;
		}
	}
	
	void SetHudEffectsAudioSources ()
	{
		hudEffectsSources = new AudioSource[6];
		for (int i=0; i<hudEffectsSources.Length; i++) {
			GameObject effectGameObject = new GameObject ("hudEffect" + i + "GameObject");
			effectGameObject.AddComponent<AudioSource> ();
			effectGameObject.transform.parent = this.transform;
			hudEffectsSources [i] = effectGameObject.GetComponent<AudioSource> ();
			
			hudEffectsSources [i].playOnAwake = false;
			hudEffectsSources [i].spatialBlend = 0f;
			hudEffectsSources [i].volume = hudVolume;
			hudEffectsSources[i].bypassReverbZones = true;
		}
	}

	void SetQueueAudioSource ()
	{
		GameObject effectGameObject = new GameObject ("queueEffectGameObject");
		effectGameObject.AddComponent<AudioSource> ();
		effectGameObject.transform.parent = this.transform;
		queueSource = effectGameObject.GetComponent<AudioSource> ();
		queueSource.playOnAwake = false;
		queueSource.spatialBlend = 0f;
		queueSource.volume = hudVolume;
		queueSource.bypassReverbZones = true;
	}


    public void ChangeSfxVolume(float newValue)
    {
        effectsVolume = newValue * 0.8f;
        PlayerPrefs.SetFloat ("Settings.EffectsVolume", newValue);

		hudVolume = newValue * 0.35f;
        PlayerPrefs.SetFloat ("Settings.HudVolume", newValue);

        for (int i=0; i<sceneEffectsSources.Length; i++) {
			sceneEffectsSources[i].volume = effectsVolume;
        }
        for (int i=0; i<hudEffectsSources.Length; i++) {
			hudEffectsSources[i].volume = hudVolume;
        }
    }
	
	// efekty dzwiekowe // // // // // // // // // // // // // // // // // // // wyszukaj pierwszy wolny source
	private void PlayOnFirstSpareSceneEffectSource (AudioClip sound, Vector3 position, float fadeTime = -1f, float fadeDelay = 0, float volumeFactor = 1f, float pitch = 1f)
	{
		for (int i=0; i<sceneEffectsSources.Length; i++) {
			if (sceneEffectsSources [i].isPlaying == false) { //jest wolny, odpal dzwiek
				sceneEffectsSources [i].clip = sound;
                sceneEffectsSources [i].gameObject.transform.position = position;

                /*float percent = 1;
				float dist = Vector3.Distance(mainCamera.transform.position, position);

              	if(dist > 150f)
					percent = Mathf.Clamp((2f - dist/150.0f), 0f, 1f);*/

				sceneEffectsSources [i].volume = effectsVolume * volumeFactor;
				sceneEffectsSources[i].pitch = pitch;
				sceneEffectsSources [i].Play ();
				if (fadeTime > 0) {
					IEnumerator fadeSound = FadeOut (sceneEffectsSources [i], fadeTime, fadeDelay);
					StartCoroutine (fadeSound);
				}

				return;
			}
		}
	}

    public void PlaySoundOnScene(string name, Vector3 position, float fadeTime = -1f, float fadeDelay = 0, float volumeFactor = 1f)
    {
        for (int i = 0; i < sceneEffectsSources.Length; i++)
        {
            if (sceneEffectsSources[i].isPlaying == false)
            { //jest wolny, odpal dzwiek
                sceneEffectsSources[i].clip = GetAudioClipUIByName(name);
                sceneEffectsSources[i].gameObject.transform.position = position;

                /*float percent = 1;
				float dist = Vector3.Distance(mainCamera.transform.position, position);

              	if(dist > 150f)
					percent = Mathf.Clamp((2f - dist/150.0f), 0f, 1f);*/

                sceneEffectsSources[i].volume = effectsVolume * volumeFactor;
                sceneEffectsSources[i].Play();
                if (fadeTime > 0)
                {
                    IEnumerator fadeSound = FadeOut(sceneEffectsSources[i], fadeTime, fadeDelay);
                    StartCoroutine(fadeSound);
                }

                return;
            }
        }
    }
	
	private void PlayOnFirstSpareHudEffectSource (AudioClip sound)
	{
		for (int i=0; i<hudEffectsSources.Length; i++) {
			if (hudEffectsSources [i].isPlaying == false) { //jest wolny, odpal dzwiek
				hudEffectsSources [i].clip = sound;
				hudEffectsSources [i].Play ();
				return;
			}
		}
	}



    public void PlayQueueSource (AudioClip[] sounds)
	{
		queueIndex = 0;
		queueAudioClips = sounds;
		PlayQueueClip ();
	}

    public void PlayUISoundEffect (string name)
    {
        for (int i = 0; i < hudEffectsSources.Length; i++)
        {
            if (hudEffectsSources[i].isPlaying == false)
            { //jest wolny, odpal dzwiek
                Debug.Log("play effect, " + name);
                hudEffectsSources[i].clip = GetAudioClipUIByName(name);
                hudEffectsSources[i].Play();
                return;
            }
        }
    }

    private AudioClip GetAudioClipUIByName(string name) // rak
    {
        switch (name)
        {
            case "button":
                return CheckAudioClipUIExists(0);
            case "button cancel":
                return CheckAudioClipUIExists(1);
            case "button confirm":
                return CheckAudioClipUIExists(2);
            case "button defense":
                return CheckAudioClipUIExists(3);
            case "no money":
                return CheckAudioClipUIExists(4);
            case "star1":
                return CheckAudioClipUIExists(5);
            case "star2":
                return CheckAudioClipUIExists(6);
            case "star3":
                return CheckAudioClipUIExists(7);
            case "window close":
                return CheckAudioClipUIExists(8);
            case "window open":
                return CheckAudioClipUIExists(9);
            case "create":
                return CheckAudioClipUIExists(10);
            case "remove":
                return CheckAudioClipUIExists(11);
            case "repair":
                return CheckAudioClipUIExists(12);
            case "upgrade":
                return CheckAudioClipUIExists(13);
            case "level fail":
                return CheckAudioClipUIExists(14);
            case "level win":
                return CheckAudioClipUIExists(15);
            case "dmg gate1":
                return CheckAudioClipUIExists(16);
            case "dmg gate2":
                return CheckAudioClipUIExists(17);
            case "destroy gate":
                return CheckAudioClipUIExists(18);
            default:
                return effectClips[0];
        }
    }

    private AudioClip CheckAudioClipUIExists(int index)
    {
        if (index < effectClips.Length && index >= 0)
        {
            return effectClips[index];
        }
        return null;
    }

	private void PlayQueueClip()
	{
		queueSource.clip = queueAudioClips[queueIndex];
		queueSource.Play ();
		StartCoroutine (OnPlayQueueSound ());
		ChangeVolumeEffects (.1f, .1f);
	}
	IEnumerator OnPlayQueueSound()
	{
		while(queueSource.isPlaying) {
			yield return null;
		}
		queueIndex++;
		if (queueIndex < queueAudioClips.Length) {
			PlayQueueClip ();
		} else {
			ChangeVolumeEffects (PlayerPrefs.GetFloat ("Settings.EffectsVolume", 1f) * 0.8f, PlayerPrefs.GetFloat ("Settings.HudVolume", 1f) * 0.5f);
		}
	}

	private void ChangeVolumeEffects(float effectsVolume, float hudVolume)
	{
		this.effectsVolume = effectsVolume;
		this.hudVolume = hudVolume;
		for (int i=0; i<sceneEffectsSources.Length; i++) {
			sceneEffectsSources[i].volume = this.effectsVolume;
		}
		for (int i=0; i<hudEffectsSources.Length; i++) {
			hudEffectsSources[i].volume = this.hudVolume;
		}
	}
	
	/////////////////////////////////////////////////////////// odpalanie dzwiekow
	
	public void PlayGameMusic ()
	{
		//ambient bg
        ambientSource[0].volume = ambientVolume;	
        ambientSource[0].clip = ambientClip;
		ambientSource[0].Play ();


        ambientSource[1].volume = ambientVolume;

        //furious sound
        if(inGameAudioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, inGameAudioClips.Length);
            playSecondAmbient(inGameAudioClips[randomIndex]);
        }
	}
	
    private void playSecondAmbient(AudioClip clip, bool looped = true)
    {
        ambientSource[1].clip = clip;
		ambientSource[1].loop = looped;
        ambientSource[1].Play();
    }

	public void StopGameMusic ()
	{
        foreach (AudioSource source in ambientSource)
        {
            if (source != null) 
    		{
    			source.Stop ();
    		}
        }
	}
	
	IEnumerator StartGameMusicWithDelay (AudioClip musicClip, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.PlayGameMusic();
	}
	
	//public void PlayButtonClick ()
	//{
	//	PlayOnFirstSpareHudEffectSource (effectClips [0]);
	//}

	//public void PlayBonusPowerClick ()
	//{
	//	PlayOnFirstSpareHudEffectSource (effectClips [1]);
	//}
	
	/*public void PlayTires (Transform parent)
	{
		loopSceneEffectSources [0].clip = loopEffectClips [0];
		loopSceneEffectSources [0].gameObject.transform.position = parent.position;
		loopSceneEffectSources [0].gameObject.transform.parent = parent;
		loopSceneEffectSources [0].Play ();
	}
	
	public void OnSetTiresPitch (float pitch)
	{
		loopSceneEffectSources [0].pitch = pitch;
	}
	
	public void StopTires ()
	{
		loopSceneEffectSources [0].Stop ();
	}*/

	public void PlayHudEffect(AudioClip clip)
	{
        if(hudVolume >= 0.07)
		    PlayOnFirstSpareHudEffectSource(clip);
	}
		
	public void PlaySceneEffect(AudioClip clip, Vector3 position, float fadeTime = -1f, float fadeDelay = 0f, float volumeFactor = 1f, float pitch = 1f)
    {
		if(effectsVolume >= 0.07)
			PlayOnFirstSpareSceneEffectSource(clip, position, fadeTime, fadeDelay, volumeFactor, pitch);
    }
	
	
	
    public void PlayLevelFinishedAudioClip(bool bSuccess)
    {
        int randomIndex = 0;
        if(bSuccess && victoryLvlAudioClips.Length > 0)
        {
            randomIndex = Random.Range(0, victoryLvlAudioClips.Length);
			playSecondAmbient(victoryLvlAudioClips[randomIndex], false);
        }
        else if(failLvlAudioClips.Length > 0)
        {
            randomIndex = Random.Range(0, failLvlAudioClips.Length);
			playSecondAmbient(failLvlAudioClips[randomIndex], false);
        }
    }

    public void OnPauseGame (bool isPaused)
	{
		if(PlayerPrefs.GetInt("Settings.SoundOn", 1) == 1)
		{
			for (int i=0; i<sceneEffectsSources.Length; i++) {
				sceneEffectsSources [i].mute = isPaused;
			}
		}
	}

	public void OnSoundOnOff (int bSound)
	{
		bool isMuted = false;
		if(bSound != 1) isMuted = true;

		for (int i=0; i<sceneEffectsSources.Length; i++) {
			sceneEffectsSources [i].mute = isMuted;
		}
		for (int i=0; i<hudEffectsSources.Length; i++) {
			hudEffectsSources [i].mute = isMuted;
		}
        foreach(AudioSource source in ambientSource) {
            source.mute = isMuted;
        }
	}

	void OnApplicationFocus(bool focusStatus)
	{
		int bSound = PlayerPrefs.GetInt("Settings.SoundOn", 1);
		if(bSound == 1)
		{
			if(focusStatus == false)
				this.OnSoundOnOff(0);
			else
				this.OnSoundOnOff(1);
		}
	}
		
    public IEnumerator PlayScoreStars(int starsCount)
    {
        yield return new WaitForSecondsRealtime(0.35f);
        if(starsCount > -1)
        {
            PlayUISoundEffect("star1");
           // PlayHudEffect(starsLvlFinishAudioClips[0]);
        }
		yield return new WaitForSecondsRealtime(0.65f);
        if(starsCount > 0)
        {
            PlayUISoundEffect("star2");
            //PlayHudEffect(starsLvlFinishAudioClips[1]);
        }
		yield return new WaitForSecondsRealtime(0.55f);
        if(starsCount > 1)
        {
            PlayUISoundEffect("star3");
           // PlayHudEffect(starsLvlFinishAudioClips[2]);
        }
		yield return new WaitForSecondsRealtime(2.45f);
		//PlayGoldGainAudioClip();
    }

	public void PlayEndGameAudioClip(){
		if (starsLvlFinishAudioClips.Length > 0) {
			PlayHudEffect(starsLvlFinishAudioClips[0]);
		}
	}

    //public void PlayUnlockedAudioClip()
    //{
    //    PlayHudEffect(unlockAudioClips[0]);
    //}

	//public void PlayUnlockStartAudioClip()
	//{
	//	PlayHudEffect(unlockAudioClips[1]);
	//}

	//public void PlayUnlockEndAudioClip()
	//{
	//	PlayHudEffect(unlockAudioClips[2]);
	//}

	//public void PlayGoldGainAudioClip()
	//{
	//	PlayHudEffect(effectClips[0]);
	//}

	//public void PlayWaveCountdownAudioClip()
	//{
	//	PlayHudEffect(effectClips[1]);
	//}

	//public void PlayNewWaveAudioClip()
	//{
	//	PlayHudEffect(effectClips[2]);
	//}

	//public void PlayWaveCompleteAudioClip()
	//{
	//	PlayHudEffect(effectClips[3]);
	//}

	//public void PlayWaveStartAudioClip()
	//{
	//	PlayHudEffect(effectClips[4]);
	//}

	//public void PlayWeaponPressAudioClip()
	//{
	//	PlayHudEffect(effectClips[5]);
	//}

	//public void PlayWeaponCancelAudioClip()
	//{
	//	PlayHudEffect(effectClips[6]);
	//}

	//public void PlayNewEnemyAudioClip()
	//{
	//	PlayHudEffect(effectClips[7]);
	//}

	private IEnumerator FadeOut(AudioSource audioSource, float fadeTime, float fadeDelay)
	{
		float startVolume = audioSource.volume;

		while (audioSource.volume > 0) {

			if (fadeDelay <= 0) {
				
				audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
			}
			else
				fadeDelay -= Time.deltaTime;
			yield return null;
		}

		audioSource.Stop ();
		audioSource.volume = startVolume;

	}


}
