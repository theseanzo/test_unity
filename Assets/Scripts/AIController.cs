using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AIController : Unit
{
    public float lookDistance = 10;
    public Vector3 aimOffset = new Vector3(0, 1.5f, 0);
    public float shootInterval = 0.5f;
    // Start is called before the first frame update
    private enum State
    {
        Idle,
        MovingToOutpost,
        Chasing
    }
    private State currentState;
    private NavMeshAgent agent;
    private Outpost currentOutpost;
    private Unit currentEnemy;
    protected override void Start()
    {
        agent = GetComponent<NavMeshAgent>(); //recall that we are a getting a component on an object, and the component we are getting is the navmeshagent
        base.Start();//we want to also call the base unit class's Start method so that we can utilize its functionality
        SetState(State.Idle);
    }
    private void SetState(State newState)
    {
        //what we want to do is look at the newState, compare it to the enum values, and figure out what to do based on that
        currentState = newState;
        //to make sure that we don't have multiple coroutines going on, we need to do one of the following: stop a particular coroutine, or stop all coroutines
        StopAllCoroutines();//this will stop coroutines from executing
        switch (currentState)
        {
            case State.Idle:
                StartCoroutine(OnIdle()); //this starts the OnIdle, which will continuously call the OnIdle function
                break;
            case State.MovingToOutpost:
                StartCoroutine(OnMovingToOutpost());
                //once we are to move to an outpost, we will need to start a coroutine for that
                break;
            case State.Chasing:
                StartCoroutine(OnChasing());
                //in order to chase something we have to see an enemy. i.e. if you see an enemy, chase them
                break;
        }
    }
    private IEnumerator OnIdle()
    {
        while(currentOutpost == null)
        {
            //we need to find an outpost to leave here
            LookForOutposts(); //if this ever finds an outpost and changes currentOutpost to be not null, then we will leave the loop
            yield return null; //we are going to, on the next frame, continue from this line
        }
        SetState(State.MovingToOutpost); //we can only get to this point once we have exited the while loop; we can only exit the while loop when our currentOutpost does not equal null
    }
    private IEnumerator OnMovingToOutpost()
    {
        agent.SetDestination(currentOutpost.transform.position);//we are telling the agent now to move towards the outpost. Once we have done this, and got to the outpost, we are going to make that outpost ours
        while(!(currentOutpost.team == team && currentOutpost.currentValue == 1))
        {
            LookForEnemies();
            yield return null; //so keep waiting until the outpost is fully captured
        }
        currentOutpost = null;
        SetState(State.Idle); //once capturing a flag, just relax. hang out.
    }
    private IEnumerator OnChasing()
    {
        agent.ResetPath(); //stop on the path you are going
        float shootTimer = 0;
        while (currentEnemy.isAlive)
        {
            shootTimer += Time.deltaTime;
            
            //we need to see if we still have vision of our enemy, and, if so, we will move to where they are 
            float distanceToEnemy = Vector3.Distance(currentEnemy.transform.position, this.transform.position);
            if(distanceToEnemy > lookDistance || !CanSee(currentEnemy.transform, currentEnemy.transform.position + aimOffset))
            {
                agent.SetDestination(currentEnemy.transform.position);
            }
            else if (shootTimer >= shootInterval)
            {
                Debug.Log("Ready to shoot");
                agent.ResetPath();
                shootTimer = 0;
                ShootAt(currentEnemy.transform);
                ShowLasers(currentEnemy.transform.position + aimOffset);
            }
            yield return null;
        }
        currentEnemy = null;
        SetState(State.Idle);
    }
    private void LookForOutposts()
    {
        int r = Random.Range(0, GameManager.Instance.outposts.Length);//find a random outpost 
        currentOutpost = GameManager.Instance.outposts[r];
    }
    private void LookForEnemies()
    {
        Collider[] surroundingColliders = Physics.OverlapSphere(this.transform.position, lookDistance);
        foreach(Collider coll in surroundingColliders)
        {
            Unit unit = coll.GetComponent<Unit>();//this is game units whether they are an AI or player 
            //now we have to check if another unit is 1) our self, 2) on our team (don't chase your own team), and whether thay visible
            if(unit != null && unit != this && unit.isAlive && unit.team != team && CanSee(unit.transform, unit.transform.position + aimOffset))
            {
                currentEnemy = unit; //follow the enemy
                SetState(State.Chasing);
                return;
            }
        }
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (currentEnemy != null)
        {
            animator.SetLookAtPosition(currentEnemy.transform.position + aimOffset);
            animator.SetLookAtWeight(1);
        }
    }
    public override void OnHit(Unit attacker)
    {
        if(currentEnemy != null)
        {
            currentEnemy = attacker;
            SetState(State.Chasing);
        }
        base.OnHit(attacker);
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        animator.SetFloat("VerticalSpeed", agent.velocity.magnitude); //we need to update the speed we are moving at to the animator or else we won't shuffle through animation frames
    }

    protected override void Respawn()
    {
        base.Respawn();
        SetState(State.Idle);
    }
    protected override void Die()
    {
        base.Die();
        currentEnemy = null;
        StopAllCoroutines();
        agent.ResetPath();
    }

}
