using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollow : MonoBehaviour
{
    RectTransform rectTransform;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Screen.width / 3, Screen.height / 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isActiveAndEnabled)
            rectTransform.position =
                new Vector3(Mathf.Clamp(Input.mousePosition.x + 30f, Screen.width / 16, Screen.width - Screen.width / 16 - rectTransform.sizeDelta.x),
                            Mathf.Clamp(Input.mousePosition.y, Screen.height / 9, Screen.height - Screen.height / 9 - rectTransform.sizeDelta.y));
    }
}
