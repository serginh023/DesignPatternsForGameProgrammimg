using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class State
{
    public enum STATE
    {
        IDLE, PATROL, PURSUE, ATTACK, SLEEP, RUNAWAY
    };

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public STATE name;
    protected EVENT stage;
    protected GameObject npc;
    protected Animator anim;
    protected Transform player;
    protected State nextState;
    protected NavMeshAgent agent;

    float visDist = 10f;
    float visAngle = 30f;
    float shootDist = 7f;

    public State(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    {
        npc = _npc;
        agent = _agent;
        anim = _anim;
        stage = EVENT.ENTER;
        player = _player;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }
    public State Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }

        return this;
    }


    public bool CanSeePlayer()
    {
        Vector3 direction = player.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);
        if (direction.magnitude < visDist && angle < visAngle)
            return true;

        return false;
    }

    public bool CanAttackPlayer()
    {
        Vector3 direction = player.position - npc.transform.position;
        if(direction.magnitude < shootDist)
            return true;

        return false;
    }

    public bool IsPlayerBehind()
    {
        Vector3 direction = npc.transform.position - player.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);
        if(direction.magnitude < 2 && angle < 45)
            return true;

        return false;
    }
}

public class Idle: State
{
    public Idle(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        :base(_npc, _agent, _anim, _player)
    {
        name = STATE.IDLE;
    }

    public override void Enter()
    {
        anim.SetTrigger("isIdle");
        base.Enter();
    }

    public override void Update()
    {
        if (CanSeePlayer())
        {
            nextState = new Pursue(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
        else if (Random.Range(0, 5000) < 10 )
        {
            nextState = new Patrol(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isIdle");
        base.Exit();
    }

}

public class Patrol: State
{
    int currentIndex = -1;

    public Patrol(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        name = STATE.PATROL;
        agent.speed = 2;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        currentIndex = 0;
        float lastDist = Mathf.Infinity;
        for(int i = 0; i < GameEnvironment.Singleton.CheckPoints.Count; i++)
        {
            GameObject thisWP = GameEnvironment.Singleton.CheckPoints[i];
            float distance = Vector3.Distance(npc.transform.position, thisWP.transform.position);
            if(distance < lastDist)
            {
                currentIndex = i - 1; //-1 == gambiarra por causa da linha 165
                lastDist = distance;
            }
        }
        anim.SetTrigger("isWalking");
        base.Enter();
    }

    public override void Update()
    {
        if(agent.remainingDistance < 1)
        {
            if(currentIndex > GameEnvironment.Singleton.CheckPoints.Count)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }
            agent.SetDestination(GameEnvironment.Singleton.CheckPoints.ElementAt(currentIndex).transform.position);
        }
        if (CanSeePlayer())
        {
            nextState = new Pursue(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
        else if (IsPlayerBehind())
        {
            nextState = new Runaway(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isWalking");
        base.Exit();
    }
}


public class Pursue : State
{
    public Pursue(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        name = STATE.PURSUE;
        agent.speed = 5;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        anim.SetTrigger("isRunning");
        base.Enter();
    }

    public override void Update()
    {
        agent.SetDestination(player.position);
        if (agent.hasPath)
            if (CanAttackPlayer())
            {
                nextState = new Attack(npc, agent, anim, player);
                stage = EVENT.EXIT;
            }
            else
            {
                nextState = new Patrol(npc, agent, anim, player);
                stage = EVENT.EXIT;
            }

    }

    public override void Exit()
    {
        base.Exit();
    }

}

public class Attack : State
{
    float rotationSpeed = 2f;
    AudioSource shootSound;
    public Attack(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    : base(_npc, _agent, _anim, _player)
    {
        name = STATE.ATTACK;
        shootSound = _npc.GetComponent<AudioSource>();
    }

    public override void Enter()
    {
        anim.SetTrigger("isShooting");
        agent.isStopped = true;
        shootSound.Play();
        base.Enter();
    }

    public override void Update()
    {
        Vector3 direction = player.position - npc.transform.position;
        //float angle = Vector3.Angle(direction, npc.transform.position);
        direction.y = 0;

        npc.transform.rotation = Quaternion.Slerp(
                                    npc.transform.rotation,
                                    Quaternion.LookRotation(direction), // o que isso faz?
                                    Time.deltaTime * rotationSpeed
                                    );

        if (!CanAttackPlayer())
        {
            nextState = new Idle(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isShooting");
        shootSound.Stop();
        base.Exit();
    }

}                                                                                                               

public class Runaway : State
{
    GameObject SafeLocation;
    public Runaway(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    : base(_npc, _agent, _anim, _player)
    {
        name = STATE.RUNAWAY;
        SafeLocation = GameEnvironment.Singleton.SafePlace;
    }

    public override void Enter()
    {
        anim.SetTrigger("isRunning");
        agent.isStopped = false;
        agent.speed = 7;
        agent.SetDestination(SafeLocation.transform.position);
        base.Enter();
    }

    public override void Update()
    {
        if(agent.remainingDistance < 1)
        {
            nextState = new Idle(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isRunning");
        base.Exit();
    }
}