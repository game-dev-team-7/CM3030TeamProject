using UnityEngine;  

public class CenterPivot : MonoBehaviour  
{  
    void Start()  
    {  
        MeshFilter meshFilter = GetComponent<MeshFilter>();  
        if (meshFilter != null)  
        {  
            Mesh mesh = meshFilter.mesh;  
            mesh.RecalculateBounds();  

            Vector3 center = mesh.bounds.center;  
            Vector3[] vertices = mesh.vertices;  

            // Adjust vertices to center the pivot  
            for (int i = 0; i < vertices.Length; i++)  
            {  
                vertices[i] -= center;  
            }  

            mesh.vertices = vertices;  
            mesh.RecalculateBounds();  
        }  
    }  
}