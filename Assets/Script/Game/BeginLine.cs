using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Tile")
        {
            GameManager.Instance.tilesManager.EnableTile(collider.gameObject.GetInstanceID());
        }
    }
}
