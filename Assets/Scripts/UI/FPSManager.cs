using UnityEngine;

public class FPSDisplay : MonoBehaviour { private float deltaTime;

void Update()
{
    deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
}

void OnGUI()
{
    int w = Screen.width, h = Screen.height;

    GUIStyle style = new GUIStyle();
    style.fontSize = 20;
    style.normal.textColor = Color.white;

    float fps = 1.0f / deltaTime;
    string text = $"{fps:0.} FPS";

    GUI.Label(new Rect(10, 10, w, h * 0.1f), text, style);
}

}