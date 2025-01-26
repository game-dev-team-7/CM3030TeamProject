using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseState
{
    protected GameFSM fsm;

    public BaseState(GameFSM fsm)
    {
        this.fsm = fsm;
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void Update()
    {
    }
}