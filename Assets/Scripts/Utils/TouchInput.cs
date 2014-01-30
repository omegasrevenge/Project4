using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
    public class Touch2D
    {
        public TouchObject Owner;
        public TouchObject Current;
        public Touch Touch;
        public object Data;
    }

    private const float MinRotationSpeed = 1f;
    private const float RotationDampTime = 4f;
    private const float RotationSpeedInterpolation = 8f;

    private static TouchInput _instance;
    private Vector2 _startInput;
    private float _startRotation;
    private float _lastRotation;
    [SerializeField]
    private float _rotationSpeed = 0;
    private float _startScale;
    [SerializeField]
    private int _touch1 = -1;
    [SerializeField]
    private int _touch2 = -1;
    [SerializeField]
    private int _singleTouch = -1;


    private List<TouchObject> _rigisteredObjects;
    private List<Touch2D> _touches;
    private List<Touch2D> _tempTouches;
    private TouchObject _solo;
    private bool _enabled = true;

    public float ZoomSpeed;

    public static TouchInput Singleton
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("TouchInput");
                _instance = obj.AddComponent<TouchInput>();
            }
            return _instance;
        }
    }

    public static bool SoloTouchInput
    {
        get { return Singleton._solo != null; }
        set
        {
            if (!value && Singleton._solo != null)
            {
                EnableOthers(Singleton._solo);
                Singleton._solo = null;
            }
        }
    }

    public static TouchObject SoloTouchObject
    {
        get { return Singleton._solo; }
        set
        {
            if (value == Singleton._solo)
                return;
            if (SoloTouchInput)
                EnableOthers(Singleton._solo);
            DisableOthers(value);
            Singleton._solo = value;
        }
    }

    public static bool Enabled
    {
        get { return Singleton._enabled; }
        set
        {
            if (value == Singleton._enabled) return;
            if (value) EnableOthers(null);
            else DisableOthers(null);
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Init();
        }
        else if (_instance != this)
            Debug.LogError("Second instance of TouchInput.");
    }

    private void Init()
    {
        Input.simulateMouseWithTouches = false;
        _rigisteredObjects = new List<TouchObject>();
        _touches = new List<Touch2D>();
        _tempTouches = new List<Touch2D>();
    }

    void Update()
    {
        if (!_enabled)
            return;
        ResetInput();
        if (!GetTouchInput())
            AddMouseInput();

    }

    public float GetRotation(float startAngle,Vector3 worldPos = default(Vector3), bool singleTouch = false, bool invertRotation = false)
    {
        if (Input.touchCount < 1 || (Input.touchCount == 1 && !singleTouch))
        {
            _touch1 = -1;
            _touch2 = -1;
            _singleTouch = -1;

            if (_rotationSpeed > MinRotationSpeed || _rotationSpeed < -MinRotationSpeed)
            {
                    startAngle += _rotationSpeed * Time.deltaTime;

                _lastRotation = startAngle;
                _rotationSpeed = Mathf.Lerp(_rotationSpeed, 0f, Time.deltaTime*RotationDampTime);
            }
            
            return startAngle;
        }

        Vector2 endInput = new Vector2();

        if (Input.touchCount == 1)
        {
            _touch1 = -1;
            _touch2 = -1;

            Touch t = Input.touches[0];
            
            if (t.fingerId != _singleTouch)
            {
                _startInput = (Vector2)Camera.main.WorldToScreenPoint(worldPos) - t.position;
                _startRotation = startAngle;
                _singleTouch = t.fingerId;
            }
            endInput = (Vector2)Camera.main.WorldToScreenPoint(worldPos) - t.position;
        }
        else
        {
            _singleTouch = -1;

            Touch t1 = new Touch();
            Touch t2 = new Touch();
            bool b1 = false;
            bool b2 = false;
            foreach (Touch touch in Input.touches)
            {
                if (touch.fingerId == _touch1)
                {
                    b1 = true;
                    t1 = touch;
                }
                if (touch.fingerId == _touch2)
                {
                    b2 = true;
                    t2 = touch;
                }
            }

            if (!b1 || !b2)
            {
                t1 = Input.touches[0];
                t2 = Input.touches[1];
                _startInput = t2.position - t1.position;
                _startRotation = startAngle;
                _touch1 = t1.fingerId;
                _touch2 = t2.fingerId;
            }
            endInput = t2.position - t1.position;
        }

        if (endInput != _startInput)
        {
            float a1 = Mathf.Atan2(_startInput.normalized.x, _startInput.normalized.y);
            float a2 = Mathf.Atan2(endInput.normalized.x, endInput.normalized.y);
            float a = a1 - a2;
            float newRotation;
            if(!invertRotation)
                newRotation = _startRotation + Mathf.Rad2Deg*a;
            else
                newRotation = _startRotation - Mathf.Rad2Deg * a;
            _rotationSpeed = Mathf.Lerp(_rotationSpeed,(newRotation - _lastRotation)/Time.deltaTime,Time.deltaTime*RotationSpeedInterpolation);
            _lastRotation = newRotation;

            return newRotation;
        }
        return startAngle;
    }

    public float GetZoom(float startScale)
    {
        if (Input.touchCount < 1)
            return startScale;
        return 0f;
    }

    private bool GetTouchInput()
    {
        if (Input.touchCount == 0)
            return false;

        foreach (Touch touch in Input.touches)
        {
            //Proof new touches:
            if (touch.phase == TouchPhase.Began)
            {
                TouchObject obj = FindObject(touch.position);
                if (obj)
                {
                    _touches.Add(new Touch2D() { Touch = touch, Owner = obj, Current = obj });
                    obj.OnTouchStart(_touches.Last());
                    Debug.Log("Started: " + obj.name);
                    _tempTouches.RemoveAll(t => t.Touch.fingerId == touch.fingerId);
                }
                continue;
            }

            //Shift old touches:
            //Look for the fingerId in _touches of the last frame
            Touch2D t2D = _tempTouches.FirstOrDefault(t => t.Touch.fingerId == touch.fingerId);
            if (!Equals(t2D, default(Touch2D)))
            {
                t2D.Touch = touch;
                t2D.Current = FindObject(touch.position);
                _touches.Add(t2D);
                _tempTouches.RemoveAll(t => t.Touch.fingerId == touch.fingerId);
                if (touch.phase == TouchPhase.Ended)
                    t2D.Owner.OnTouchEnd(t2D);
                else
                    t2D.Owner.OnTouchMove(t2D);
            }
        }
        return true;
    }

    private void ResetInput()
    {
        _tempTouches = _touches;
        _touches = new List<Touch2D>();
    }

    private void AddMouseInput()
    {

        if (Input.GetMouseButtonDown(0))
        {
            TouchObject obj = FindObject(Input.mousePosition);
            if (obj)
            {
                _touches.Add(new Touch2D() { Owner = obj, Current = obj });
                obj.OnTouchStart(_touches.Last());
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (_tempTouches.Count == 0)
                return;
            Touch2D t2D = _tempTouches[0];
            t2D.Current = FindObject(Input.mousePosition);
            _touches.Add(t2D);
            t2D.Owner.OnTouchEnd(t2D);

        }
        else if (Input.GetMouseButton(0))
        {
            if (_tempTouches.Count == 0)
                return;
            Touch2D t2D = _tempTouches[0];
            t2D.Current = FindObject(Input.mousePosition);
            _touches.Add(t2D);
            t2D.Owner.OnTouchMove(t2D);
        }
    }

    private TouchObject FindObject(Vector2 pos)
    {
        if (Camera.main == null) return null;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (hits.Length > 0)
        {         
            foreach (RaycastHit hit in hits)
            {
                Collider collider = hit.collider;
                TouchObject obj = collider.GetComponent<TouchObject>();
                if (obj != null && obj.Enabled)
                    return obj;
            }    
        }
        return null;
    }

    public static Touch[] GetInitTouches(TouchObject obj)
    {
        //Debug.Log("" + Singleton._touches.Count);
        //if (Singleton._touches.Count > 0) Debug.Log(Singleton._touches[0].Touch.position);
        return Singleton._touches.Where(t => t.Touch.phase == TouchPhase.Began && t.Owner == obj).Select(t => t.Touch).ToArray();
    }

    public static Touch[] GetEndTouches(TouchObject obj)
    {
        return Singleton._touches.Where(t => t.Touch.phase == TouchPhase.Ended && t.Owner == obj).Select(t => t.Touch).ToArray();
    }

    public static Touch[] GetTouches(TouchObject obj)
    {
        return Singleton._touches.Where(t => t.Owner == obj).Select(t => t.Touch).ToArray();
    }

    public static void Register(TouchObject obj)
    {
        if (Singleton._rigisteredObjects.Contains(obj))
            return;
        Singleton._rigisteredObjects.Add(obj);
    }

    public static void Unregister(TouchObject obj)
    {
        if (_instance)
            Singleton._rigisteredObjects.Remove(obj);
    }

    public static void DisableOthers(TouchObject obj)
    {
        foreach (TouchObject touchObject in Singleton._rigisteredObjects.Where(o => o != obj))
        {
            touchObject.DisableBy(obj);
        }
    }

    public static void EnableOthers(TouchObject obj)
    {
        foreach (TouchObject touchObject in Singleton._rigisteredObjects.Where(o => o != obj))
        {
            touchObject.EnableBy(obj);
        }
    }
}
