using UnityEngine;

public class Option1Handler : MonoBehaviour
{
	public GameObject GObjOfTheManipulatedLabel;
    private int Status = 0;
    private int saveStatus = 0;
    private dfLabel _label;

    void Awake()
    {
		_label = GObjOfTheManipulatedLabel.GetComponent<dfLabel>();
    }

    public void incStatus()
    {
        Status++;
        if (Status > 2) Status = 0;
    }

    void Update()
    {
        if (Status == saveStatus) return;
        switch (Status)
        {
            case 0:
                _label.Text = "One";
                break;

            case 1:
                _label.Text = "Two";
                break;

            case 2:
                _label.Text = "Three";
                break;
        }
        saveStatus = Status;
    }
}
