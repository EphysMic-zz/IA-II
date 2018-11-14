using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queries : MonoBehaviour
{
    public SpatialGrid targetGrid;
    public float width = 15f;
    public float height = 30f;
    public IEnumerable<GridEntity> selected = new List<GridEntity>();

    public IEnumerable<GridEntity> Query()
    {

        var h = height * 0.5f;
        var w = width * 0.5f;

        return targetGrid.Query(transform.position + new Vector3(-w, 0, -h),transform.position + new Vector3(w, 0, h), x => true);

    }

    void OnDrawGizmos()
    {
        if (targetGrid == null)
            return;

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(transform.position, new Vector3(width, 0, height));

        if (Application.isPlaying)
        {
            selected = Query();
        }
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 20, 20), "HOLA");
    }
}
