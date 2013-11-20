using UnityEngine;

public class Option1Handler : MonoBehaviour
{

    public int status = 0;
    private dfRichTextLabel label;

    void Awake()
    {
        label = GetComponent<dfRichTextLabel>();
    }

    public void incStatus()
    {
        status++;
        if (status > 2) status = 0;
    }

    void Update()
    {
        switch (status)
        {
            case 0:
                label.Text = "One";
                break;

            case 1:
                label.Text = "Two";
                break;

            case 2:
                label.Text = "Three";
                break;
        }
    }
}
