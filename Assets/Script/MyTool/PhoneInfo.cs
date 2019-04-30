using UnityEngine;
using System.Collections;

public class PhoneInfo : MonoBehaviour
{

#if UNITY_EDITOR
    void OnGUI()
    {
        GUILayout.Space(50);

        GUILayout.Label("Total DrawCall: " + UnityEditor.UnityStats.drawCalls, GUILayout.Width(500));
        GUILayout.Label("Batch: " + UnityEditor.UnityStats.batches, GUILayout.Width(500));
        GUILayout.Label("Static Batch DC: " + UnityEditor.UnityStats.staticBatchedDrawCalls, GUILayout.Width(500));
        GUILayout.Label("Static Batch: " + UnityEditor.UnityStats.staticBatches, GUILayout.Width(500));
        GUILayout.Label("DynamicBatch DC: " + UnityEditor.UnityStats.dynamicBatchedDrawCalls, GUILayout.Width(500));
        GUILayout.Label("DynamicBatch: " + UnityEditor.UnityStats.dynamicBatches, GUILayout.Width(500));

        GUILayout.Label("Tri: " + UnityEditor.UnityStats.triangles, GUILayout.Width(500));
        GUILayout.Label("Ver: " + UnityEditor.UnityStats.vertices, GUILayout.Width(500));
    }
#endif 
}