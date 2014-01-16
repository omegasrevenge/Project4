using UnityEngine;
using System.Collections;

public class GUIObjectVisualizer : MonoBehaviour
{
    public const string SpriteBarStr = "sprite_bar";
    public const float Speed = 10f;
    public const float Multiplier = 20f;

    public AudioSource Audio;

    private GameObject[] _bars;
    private dfControl _control;
    private float[] _spectrumData;

    void Awake()
    {
        GameObject bar = transform.FindChild(SpriteBarStr).gameObject;
        _control = GetComponent<dfControl>();

        _spectrumData = new float[64];

        _bars = new GameObject[32];
        Vector2 size = new Vector2(_control.Size.x/65f,_control.Size.y);
        Vector2 position = new Vector2(0f, 0f);
        Vector2 scale = new Vector3(1f,0.01f,1f);
        for (int i = 0; i< _bars.Length;i++)
        {
            dfControl b = _control.AddPrefab(bar);
            b.Size = size;
            position.x = (i*2 + 1)*size.x;
            b.RelativePosition = position;
            b.transform.localScale = scale;
            _bars[i] = b.gameObject;
        }
        Destroy(bar);
    }

    void Update()
    {
        if (Audio == null)
            return;
        Audio.GetOutputData(_spectrumData,0);

        Vector3 scale = new Vector3(1f,0.01f,1f);
        for (int i = 0; i < _bars.Length; i++)
        {
            scale.y = Mathf.Clamp(_spectrumData[i*2]*Multiplier,0f,1f);
             _bars[i].transform.localScale = Vector3.Lerp(_bars[i].transform.localScale, scale, Time.deltaTime * Speed);
        }

    }
}
