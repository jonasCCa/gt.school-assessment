using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int mastery;
    public string grade;
    public string domain;
    public string cluster;
    public string standardID;
    public string standardDescription;

    bool isUIActive;

    private void OnMouseDown()
    {
        if (!GameObject.Find("StacksManager").GetComponent<StacksManager>().IsBlockInCurrentStack(grade))
            return;

        UIHandler h = GameObject.FindGameObjectWithTag("UI Handler").GetComponent<UIHandler>();
        
        h.infoDisplay.SetActive(true);
        h.infoText.text = "<b><size=125%>" + grade + ": " + domain + "</size></b>\n\n" +
                          cluster + "\n\n" +
                          "<b>" + standardID + ":</b>\n" + standardDescription;

        isUIActive = true;
    }

    private void OnMouseExit()
    {
        if(isUIActive)
            GameObject.FindGameObjectWithTag("UI Handler").GetComponent<UIHandler>().infoDisplay.SetActive(false);
    }
}
