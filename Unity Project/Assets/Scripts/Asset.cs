using UnityEngine;
using System.Collections;
using Veis.Unity.Bots;

public class Asset : MonoBehaviour
{
    public string AssetKey { get; protected set; }
    public string AssetName { get; protected set; }
    public Vector3 StartPosition { get; protected set; }

    protected void Start()
    {
        this.AssetKey = UUID.Random().ToString();
        this.AssetName = this.gameObject.name;
        this.StartPosition = this.transform.position;
    }
}
