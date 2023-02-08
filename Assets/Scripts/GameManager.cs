using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;//underscore for private variables
    public Color[] teams;
    internal Outpost[] outposts;
    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
                //some setup code
                _instance.OnCreateInstance();
            }
            return _instance;
        }
    }
    private void OnCreateInstance()
    {
        outposts = GetComponentsInChildren<Outpost>();
        Debug.Log("The number of outposts is " + outposts.Length);
    }

}
