using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ParticleLayering : MonoBehaviour
{
    public string sortLayerString = "";
    void Start()
    {
        GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = sortLayerString;
    }
}