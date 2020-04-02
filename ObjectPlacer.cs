using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPlacer : MonoBehaviour
{

    public List<GameObject> proxies;
    public List<GameObject> tiltableObjects;
    public List<GameObject> uprightObjects;
    private int tiltableObjNum = 0;
    private int uprightObjNum = 0;
    private float totalobjLength =0.0f;
    private List<List<GameObject>> objslist;
    private List<GameObject> InstancedObjs;
    private bool cloneIsMade = false;
    private List<GameObject> cloneslist;


    // Start is called before the first frame update
    void Start()
    {
        // objslist = new List<List<GameObject>>();
        // objslist.Add(tiltableObjects);
        // objslist.Add(uprightObjects);
        // tiltableObjNum = tiltableObjects.Count;
        // uprightObjNum = uprightObjects.Count;

        // for (int i = 0;  i < proxies.Count; i++)
        // {
        //     makeInstances(proxies[i], objslist, totalobjLength, tiltableObjNum, uprightObjNum);
        // }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if(cloneIsMade == true)
            {
                for (int i = 0; i < cloneslist.Count; i++)
                {
                    Destroy(GameObject.Find(cloneslist[i].transform.name + "_Clone"));
                    proxies[i].GetComponent<Renderer>().enabled = true;
                }
                cloneIsMade = false;
            }
            if(cloneIsMade != true)
            {
                objslist = new List<List<GameObject>>();
                cloneslist = new List<GameObject>();
                objslist.Add(tiltableObjects);
                objslist.Add(uprightObjects);
                tiltableObjNum = tiltableObjects.Count;
                uprightObjNum = uprightObjects.Count;

                for (int i = 0;  i < proxies.Count; i++)
                {
                    makeInstances(proxies[i], objslist, totalobjLength, tiltableObjNum, uprightObjNum);
                    cloneslist.Add(GameObject.Find(proxies[i].transform.name.ToString()));
                }
                cloneIsMade = true;
            }
        }
    }


    void makeInstances(GameObject proxy, List<List<GameObject>> objslist, float totalobjLength, int tiltObjs, int upObjs)
    {
        float prevAngle = 0.0f;
        Quaternion orient = Quaternion.Euler(0, 0, 0);
        GameObject parent = new GameObject();
        parent.name = proxy.transform.name.ToString() + "_Clone";
        GameObject lastClone;

        //---Calculate proxy boundry and get origin point
        Renderer proxyRenderer = proxy.GetComponent<Renderer>();
        Vector3 proxyboundMax = proxyRenderer.bounds.max;
        Vector3 proxyboundMin = proxyRenderer.bounds.min;
      

        Vector3 heightPoint = new Vector3(proxyboundMin.x, proxyboundMax.y, proxyboundMin.z);
        float height = heightPoint.y - proxyboundMin.y;

        Vector3 depthPoint = new Vector3(proxyboundMax.x, proxyboundMin.y, proxyboundMin.z);
        float centerPoint = Vector3.Distance(proxyboundMin, depthPoint)/ 2.0f;

        Vector3 WidthPoint = new Vector3(proxyboundMin.x, proxyboundMin.y, proxyboundMax.z);
        float boundlength = Vector3.Distance(proxyboundMin, WidthPoint);


        while (totalobjLength <= boundlength)
        {
            int subObjIndex = 0;
            int objIndex = UnityEngine.Random.Range(0, 2);

            if (objIndex == 0)
            {
                subObjIndex = UnityEngine.Random.Range(0, tiltObjs);
            }
            if (objIndex == 1)
            {
                subObjIndex = UnityEngine.Random.Range(0, upObjs);
            }
         
            GameObject clone = Instantiate(objslist[objIndex][subObjIndex], new Vector3(0, 0, 0), Quaternion.identity, parent.transform);
            Renderer objRenderer = clone.GetComponent<Renderer>();


            Vector3 objHeightMax = objRenderer.bounds.max;
            float scaleRandom = UnityEngine.Random.Range(0.8f, 1.0f);
            float scale = (height / objHeightMax.y) * scaleRandom;
            clone.transform.localScale = new Vector3(scale, scale, scale);

            if (prevAngle != 0.0f || objIndex == 1)
            {
                Vector3 rot = new Vector3(0, 0, 0);
                orient = Quaternion.Euler(rot.x, rot.y, rot.z);
                prevAngle = 0.0f;
            }
            else
            {
                Vector3 rot = new Vector3(UnityEngine.Random.Range(0.0f, 30.0f), 0, 0);
                orient = Quaternion.Euler(rot.x, rot.y, rot.z);
                prevAngle = rot.x;
            }
            clone.transform.rotation = orient;

            clone.transform.position = new Vector3(0, 0, totalobjLength + objRenderer.bounds.extents.z - objRenderer.bounds.center.z);

            totalobjLength += objRenderer.bounds.size.z;
            lastClone = clone;
            if (totalobjLength > objRenderer.bounds.size.z)
            {
                lastClone.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            
        }

        

        proxyRenderer.enabled = false;
        parent.transform.position = new Vector3(proxyboundMin.x + centerPoint, proxyboundMin.y, proxyboundMin.z);
    }

}
