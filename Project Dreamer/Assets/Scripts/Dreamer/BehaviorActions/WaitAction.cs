using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait", story: "Wait between [min] and [max] seconds", category: "Action", id: "2eec65b4bc079279033641911bce3763")]
public partial class WaitAction : Action
{
    [SerializeReference] public BlackboardVariable<float> min;
    [SerializeReference] public BlackboardVariable<float> max;

    private float timerMax;
    private float currentTimer;

    protected override Status OnStart()
    {
        
        //If there isn't a currently active timer, pick a new timer value
        if (currentTimer == 0)
        {
            //Determine how long the timer will run for (inclusive)
            timerMax = UnityEngine.Random.Range(min.Value, max.Value);
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //Timer is maxed out, end wait
        if (currentTimer >= timerMax)
        {
            //Reset timer for next time
            currentTimer = 0;

            return Status.Success;
        }

        currentTimer += Time.deltaTime;
        return Status.Running;
    }

    protected override void OnEnd()
    {

    }
}

