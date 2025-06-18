using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach : MonoBehaviour
{
    public GameObject Attaching;
    public bool rotate = false;
    
    public bool move = true;
    public bool scale;
    public GameObject AttachingTo;
    void Update()
    {
        if (move)
        {
            Attaching.transform.position = AttachingTo.transform.position;
        }
        if(scale)
        {
            Attaching.transform.localScale = AttachingTo.transform.localScale;
        }
        if(rotate)
        {
            Attaching.transform.rotation = AttachingTo.transform.rotation; 
        }
    }
}
