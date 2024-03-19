using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cat))]
public class CatEditor : Editor
{
    private GUIStyle style;

    void OnSceneGUI()
    {
        Cat cat = (Cat)target;
        if (cat == null)
        {
            return;
        }
        style = new GUIStyle();
        style.normal.textColor = Color.black;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 18;
        style.normal.background = Texture2D.whiteTexture;

        if (Application.isPlaying)
        {

            if (cat.currentBehaviorState != null)
            {
                Handles.color = Color.blue;
                Handles.Label(cat.transform.position + Vector3.up * 2, cat.currentBehaviorState.name, style);
            }

        }
    }
}
