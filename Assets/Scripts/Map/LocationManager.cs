﻿using UnityEngine;

public class LocationManager : MonoBehaviour {

    private static LocationManager _instance;
    //Latitude = 52.50451
    //Longitude = 13.39699
    [SerializeField]
    private float _latitude = 52.50648f;
    [SerializeField]
    private float _longitude = 13.39403f;
    [SerializeField]
    private float _direction = 0f;

    public const float MoveSpeed = 3f;

    public static LocationManager Singleton
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("LocationManager");
                _instance = obj.AddComponent<LocationManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
        {
            Debug.LogError("Second instance of LocationManager.");
            Destroy(this);
            return;
        }
        Init();
    }

    private void Init()
    {
        Input.location.Start();
        Input.compass.enabled = true;

        
    }

    private void Update()
    {
#if !UNITY_EDITOR
        if (Input.location.status != LocationServiceStatus.Running)
        {
            //Debug.Log("Wrong Locations Status: "+ Input.location.status);
            return;
        }   
            SetLocation();
        
        _direction = Mathf.LerpAngle(_direction, Input.compass.trueHeading, Time.deltaTime*MoveSpeed);
#else
        _longitude += Input.GetAxis("Horizontal")*Time.deltaTime*0.001f;
        _latitude += Input.GetAxis("Vertical") * Time.deltaTime *0.001f;
#endif
        
    }


    private void SetLocation()
    {
        _longitude = Input.location.lastData.longitude;
        _latitude = Input.location.lastData.latitude;
    }

    public static Vector2 GetCurrentPosition()
    {
        return new Vector2(Singleton._longitude, Singleton._latitude);
    }

    public static MapUtils.ProjectedPos GetCurrentProjectedPos(int zoomLevel)
    {
        return MapUtils.GeographicToProjection(GetCurrentPosition(), zoomLevel);
    }

    public static float GetDirection()
    {
        return Singleton._direction;
    }
}