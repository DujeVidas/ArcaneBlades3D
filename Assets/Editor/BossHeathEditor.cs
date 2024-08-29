using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BossHealth))]
public class BossHealthEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the default inspector

        BossHealth bossHealth = (BossHealth)target;

        if (GUILayout.Button("Decrease Health"))
        {
            bossHealth.TakeDamage(20); // Decrease health by 10 each time the button is clicked
        }
    }
}
