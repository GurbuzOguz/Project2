using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Pixelplacement;

public class PlatformManager : Singleton<PlatformManager>
{
    public float platformMovementSpeed;

    [Header("GameObjects")]
    public GameObject lastPlatformGo;

    [Header("Prefabs")]
    public GameObject platformPrefab;

    [Header("Slicers")]
    public GameObject rightSlicer;
    public GameObject leftSlicer;

    [Header("Transforms")]
    public Transform platformSpawnTransform;

    [Header("Lists")]
    public List<Color32> platformColorList = new List<Color32>();

    Vector3 platformScale;

    Material platformMat;

    float rnd;
    int colorCounter;

    private void Start() 
    {
        
        platformScale = platformPrefab.transform.localScale;    

        PlatformSpawn();
    }

    private void Update() 
    {
        
    }    

    public void PlatformSpawn()
    {
        rnd = ChooseRandomSpawnPos();

        platformSpawnTransform.position = new Vector3(lastPlatformGo.transform.position.x + (platformScale.x * rnd), lastPlatformGo.transform.position.y , lastPlatformGo.transform.position.z + platformScale.z);

        GameObject platformGo = Instantiate(lastPlatformGo, platformSpawnTransform.position, Quaternion.identity, transform);

        MovePlatform(platformGo, rnd);
        ChangePlatformColor(platformGo);
        SlicerPositionUpdate(lastPlatformGo);
    }

    public float ChooseRandomSpawnPos()
    {
        int rnd = Random.Range(0,2);

        if (rnd == 0)
            rnd = 1;
        else
            rnd = -1;
            
        return rnd;
    }

    public void MovePlatform(GameObject platformGo, float rnd)
    {
        lastPlatformGo.transform.DOKill();

        platformGo.transform.DOMoveX(platformGo.transform.position.x - (platformScale.x * (2 * rnd)), platformMovementSpeed).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    public void ChangePlatformColor(GameObject platformGo)
    {
        colorCounter++;

        platformMat = platformGo.GetComponent<MeshRenderer>().material;
        platformMat.color = platformColorList[colorCounter];

        if (colorCounter == platformColorList.Count - 1)
            colorCounter = 0;
    }

    public void SlicerPositionUpdate(GameObject platformGo)
    {
        Vector3 scale = lastPlatformGo.transform.localScale;

        rightSlicer.transform.position = new Vector3((scale.x / 2) + .25f, platformGo.transform.position.y, platformGo.transform.position.z + 1);
        leftSlicer.transform.position = new Vector3((-scale.x / 2) - .25f, platformGo.transform.position.y, platformGo.transform.position.z + 1);
    }
}
