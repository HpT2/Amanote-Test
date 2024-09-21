using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLine : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Tile")
        {
            GameManager.Instance.tilesManager.DestroyTile(collider.gameObject.GetInstanceID());
        }
    }
}
