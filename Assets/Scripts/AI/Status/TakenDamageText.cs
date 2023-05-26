using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

// efekt jak w wiedzminie, pojawia sie dmg ile dostal przeciwnik i ulatnia sie
// gravity wlaczone do tego obiektu
public class TakenDamageText : BarElement
{

    private Text text;

    public Color normal = Color.white;
    public Color nerf = Color.red;
    public Color buffed = Color.green;
    public Color crit = Color.yellow;
    
    [Tooltip("czas zanikania tekstu")]
    [SerializeField] private float duration = 2.0f;

    [Tooltip("predkosc unoszenia sie tekstu")]
    [SerializeField] private float floatSpeed = 100.0f;

    private float timer = 0.0f;
    private float factor;
    private Vector3 force;
	private Rigidbody myRigidbody;

    public void SetDamageType(int type)
    {
        switch(type)
        {
            case 0:
                text.color = normal;
                break;
            case 1:
                text.color = buffed;
                break;
            case -1:
                text.color = nerf;
                break;
            case 2:
                text.color = crit;
                break;
        }
    }

    public TakenDamageText(AI ai, GameObject go, float damageTaken, float spawnHeight)
    {
        Init(ai, go, damageTaken, spawnHeight);
    }

    public void Init (AI ai, GameObject go, float damageTaken, float spawnHeight)
    {
        this.aiObject = ai;

        Transform parent = ObjectPool.Instance.WorldCanvas.transform;// GameObject.Find("WorldCanvas").transform;
        this.gameObject.transform.SetParent(parent, true);
        this.transform.position = aiObject.transform.position + Vector3.up * spawnHeight;
        this.transform.LookAt(Camera.main.transform.position);

        InitText(damageTaken);

		myRigidbody = this.gameObject.GetComponent<Rigidbody>();
        force = new Vector3(0.0f, 300.0f, 0.0f);
		myRigidbody.drag = 5f;
		myRigidbody.AddForce(force);
        }

    private void InitText(float damageTaken)
    {
        text = gameObject.FindComponentInChildWithName<Text>("Damage");
        if (text != null)
            text.text = damageTaken.ToString(); // dmg jaki dostal gracz, pobierany w konstruktorze
        else
            Debug.LogError("Nie ma textu na ktorym mozna by bylo wyswietlic zadany damage");
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer <= duration)
        {
            factor = Globals.Easing(timer / duration);
            SetAlpha(Mathf.Lerp(1.0f, 0.0f, factor));
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public override float GetAlpha ()
    {
        return text.canvasRenderer.GetAlpha();
    }

    public override void HideElement ()
    {
        this.text.gameObject.SetActive(false);
    }

    public override void ShowElement()
    {
        this.text.gameObject.SetActive(true);
    }

    public override void SetAlpha (float alpha)
    {
        text.canvasRenderer.SetAlpha(alpha);
    }
    
}
