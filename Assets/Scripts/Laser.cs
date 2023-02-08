using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Start is called before the first frame update
    public float lifeTime = 0.05f; //how long the laser exists
    private LineRenderer line;
    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }
    public void Init(Color c, Vector3 start, Vector3 end)
    {
        line.startColor = c;
        line.endColor = c;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        Invoke("DestroyMe", lifeTime);
    }
    private void DestroyMe()
    {
        Destroy(this.gameObject);
    }
    void Start()
    {
        
    }

}
