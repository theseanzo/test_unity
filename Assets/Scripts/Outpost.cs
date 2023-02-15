using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outpost : MonoBehaviour
{
    // Start is called before the first frame update
    public float flagTop = 7;
    public float flagBottom = 1;

    private SkinnedMeshRenderer flag;
    public float scoreInterval = 5;
    internal float currentValue = 0;
    public float valueIncrease = 0.005f;
    internal int team = -1; //by default no team has the flag. recall that "internal" means it's viewable as public by anything in this project/namespace
    private float timer;
    void Start()
    {
        flag = GetComponentInChildren<SkinnedMeshRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        //we will need to update the position of the flag based on teams
        //we need to get the team color of whomever has captured this flag
        //we will then need to change the color of the flag
        //we will need to then transform the position of the flag based on some sort of timer
        if (team != -1)
        {
            Color teamColor = GameManager.Instance.teams[team];
            flag.material.color = Color.Lerp(Color.white, teamColor, currentValue);
            flag.transform.parent.localPosition = new Vector3(0, Mathf.Lerp(flagBottom, flagTop, currentValue));
            if (currentValue == 1)
            {
                timer += Time.deltaTime;
                if (timer >= scoreInterval)
                {
                    timer = 0;
                    ScoreManager.Instance.scores[team]++;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other) //on trigger stay means that someone has triggered the collision and stayed there (i.e. breached the bounds and continued
    {
        Unit unit = other.GetComponent<Unit>();
        if(unit != null) //make sure we actually found a unit and not something else colliding
        {
            if (unit.team == team)
            {
                currentValue += valueIncrease;
                if(currentValue >= 1)
                {
                    currentValue = 1;
                }
            }
            else
            {
                currentValue -= valueIncrease;
                if(currentValue <= 0)
                {
                    currentValue = 0;
                    team = unit.team; //the new unit team
                }
            }
        }
    }
}
