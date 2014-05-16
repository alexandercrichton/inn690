using UnityEngine;
using System.Collections;

public class PlaceMarker : MonoBehaviour
{
    public string PlaceMarkerName { get; protected set; }

    protected void Start()
    {
        this.PlaceMarkerName = this.gameObject.name;
    }
}
