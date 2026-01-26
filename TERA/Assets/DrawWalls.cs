using UnityEngine;

public class DrawWalls : MonoBehaviour
{
    public Color gizmoColor = Color.green;

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // Si tu as un BoxCollider
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix; // utilise la position/rotation/scale locale
            Gizmos.DrawWireCube(box.center, box.size);
            return;
        }

        // Si tu as un MeshCollider
        MeshCollider meshCol = GetComponent<MeshCollider>();
        if (meshCol != null && meshCol.sharedMesh != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireMesh(meshCol.sharedMesh);
        }
    }
}
