using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkinnedMeshRepair : MonoBehaviour
{
    //public GameObject target;
    public GameObject original;

    //internal Mesh mesh;

    internal SkinnedMeshRenderer skin;
    void Awake()
    {

        //skin = GetComponent<SkinnedMeshRenderer>();



        //gameObject.GetComponent<SkinnedMeshRenderer>().rootBone = GameObject.FindGameObjectWithTag("pelvisbebe").transform;
        //tela = GetComponent<SkinnedMeshRenderer>().sharedMesh;

        //skin.sharedMesh = tela;



    }

    void Start()
    {
        //mesh.boneWeights = tela.boneWeights;

        SkinnedMeshRenderer targetRenderer = GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer originalRenderer = original.GetComponent<SkinnedMeshRenderer>();
        //Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();


        SkinnedMeshRenderer myRenderer = GetComponent<SkinnedMeshRenderer>();
        Transform[] newBones = new Transform[myRenderer.bones.Length];

        int a = 0;
        foreach (Transform boneoriginal in originalRenderer.bones)
        {
            int b = 0;
            foreach (Transform bonenuevo in targetRenderer.bones)
            {
                if (bonenuevo.name == boneoriginal.name)
                {
                    newBones[a] = targetRenderer.bones[b];
                    continue;
                }
                b++;
            }
            a++;
        }

        targetRenderer.sharedMesh = originalRenderer.sharedMesh;

        targetRenderer.bones = newBones;

    }





}

