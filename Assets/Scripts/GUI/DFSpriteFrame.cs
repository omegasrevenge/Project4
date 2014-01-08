using UnityEngine;
using System.Collections;

public class DFSpriteFrame : MonoBehaviour
{
    private dfSprite[] _masks;
    private dfSprite _control; 
	public Vector2 Padding = new Vector2(2f,2f);

	void Awake ()
	{
        _control = GetComponent<dfSprite>();

        _masks = new dfSprite[4];
	    for (int i = 0; i < _masks.Length; i++)
	    {
            _masks[i] = _control.AddControl<dfSlicedSprite>();
            _masks[i].Atlas = _control.Atlas;
            _masks[i].SpriteName = "gui_blank";
            _masks[i].name = "sprite_mask_" + i;   
	    }

	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
        _masks[0].Size = new Vector2(_control.RelativePosition.x, _control.Size.y)+(2f*Padding);
        _masks[1].Size = new Vector2(_control.Parent.Size.x - _control.RelativePosition.x - _control.Size.x, _control.Size.y) + (2f * Padding);
        _masks[2].Size = new Vector2(_control.Parent.Size.x, _control.RelativePosition.y) + (2f * Padding);
        _masks[3].Size = new Vector2(_control.Parent.Size.x, _control.Parent.Size.y - _control.RelativePosition.y - _control.Size.y)+(2f*Padding);

	    _masks[0].RelativePosition = new Vector2(-_control.RelativePosition.x, 0) - Padding;
        _masks[1].RelativePosition = new Vector2(_control.Size.x, 0) - Padding;
        _masks[2].RelativePosition = -(Vector2)_control.RelativePosition - Padding;
        _masks[3].RelativePosition = new Vector2(-_control.RelativePosition.x, _control.Size.y) - Padding;
	}
}
