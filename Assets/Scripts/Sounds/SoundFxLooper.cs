using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SoundFxPlayer))]
public class SoundFxLooper : MonoBehaviour
{
    [SerializeField] private SoundFxPlayer soundFxPlayer = null;
    [SerializeField] private bool useInterval = false;
    [SerializeField] private float minTime = 1f;
    [SerializeField] private float maxTime = 1f;

    private float counter = 0;

    private void OnDisable()
    {
        soundFxPlayer.StopFx();
    }

    private void Update()
    {
        if (!soundFxPlayer.AudioSource.isPlaying)
        {
            if ((counter -= Time.deltaTime) <= 0)
            {
                soundFxPlayer.PlayFx();
                counter = useInterval ? Random.Range(minTime, maxTime) : 0;
            }
        }
    }

    private void Reset()
    {
        soundFxPlayer = gameObject.GetComponent<SoundFxPlayer>();
    }
}
