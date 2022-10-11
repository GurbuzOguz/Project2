using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class Slice : MonoBehaviour
{
    PlatformManager platformManager;

    const string strPlatform = "Platform";

    public Transform cutPlane;

    public Material crossMaterial;

    public LayerMask layerMask;
    LayerMask platformLayer;

    WaitForSeconds waitForSeconds;

    public enum SliceDirection
    {
        empty,
        right,
        left
    }

    public SliceDirection sliceDirection;

    private void Start() 
    {
        platformManager = PlatformManager.Instance;
        platformLayer = LayerMask.NameToLayer(strPlatform);    

        waitForSeconds = new WaitForSeconds(3);
    }

    private void Update() 
    {
        RaycastHit hit;

        if (sliceDirection == SliceDirection.right)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity, layerMask))
            {
                gameObject.transform.position = hit.point;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Slicer();
        }
    }

    public void Slicer()
    {
        Collider[] hits = Physics.OverlapBox(cutPlane.position, new Vector3(5, 0.1f, 5), cutPlane.rotation, layerMask);

        if (hits.Length <= 0)
            return;

        for (int i = 0; i < hits.Length; i++)
        {
            SlicedHull hull = SliceObject(hits[i].gameObject, crossMaterial);
            if (hull != null)
            {
                GameObject bottom = hull.CreateLowerHull(hits[i].gameObject, crossMaterial);
                GameObject top = hull.CreateUpperHull(hits[i].gameObject, crossMaterial);

                if (sliceDirection == SliceDirection.right)
                {
                    top.layer = platformLayer;
                    platformManager.lastPlatformGo.layer = 0;
                    platformManager.lastPlatformGo = top;
                    AddHullComponents(bottom);
                    AddHullWithOutRigidboyd(top);
                    StartCoroutine(FallObjectsClose(bottom));
                    platformManager.PlatformSpawn();
                }

                if (sliceDirection == SliceDirection.left)
                {
                    bottom.layer = platformLayer;
                    platformManager.lastPlatformGo.layer = 0;
                    platformManager.lastPlatformGo = bottom;
                    AddHullComponents(top);
                    AddHullWithOutRigidboyd(bottom);
                    StartCoroutine(FallObjectsClose(top));
                    platformManager.PlatformSpawn();
                }

                hits[i].gameObject.SetActive(false);
                //Destroy(hits[i].gameObject);
            }
            else
            {
                //TODO: Doğru yerleştirildi. Ses ve kombo eklenecek
                Debug.Log("Kombo");
            }
        }
    }

    public void AddHullComponents(GameObject go)
    {
        Rigidbody rb = go.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        MeshCollider collider = go.AddComponent<MeshCollider>();
        collider.convex = true;

        rb.AddExplosionForce(100, go.transform.position, 20);
    }

    public void AddHullWithOutRigidboyd(GameObject go)
    {
        MeshCollider collider = go.AddComponent<MeshCollider>();
        collider.convex = true;
    }

    public SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        if (obj.GetComponent<MeshFilter>() == null)
            return null;

        return obj.Slice(cutPlane.position, cutPlane.right, crossSectionMaterial);
    }

    IEnumerator FallObjectsClose(GameObject go)
    {
        yield return waitForSeconds;
        go.SetActive(false);
    }
}
