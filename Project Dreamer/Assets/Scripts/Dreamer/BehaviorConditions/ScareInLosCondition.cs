using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ScareInLOS", story: "[Scare] in [DreamerSight] ? [SawScare] [Star]", category: "Conditions", id: "3a30d06eeb8cc633549fc83d5e2ebf53")]
public partial class ScareInLosCondition : Condition
{
    [SerializeReference] public BlackboardVariable<DreamerSight> DreamerSight;
    [SerializeReference] public BlackboardVariable<Boolean> SawScare;
    [SerializeReference] public BlackboardVariable<GameObject> Scare;
    [SerializeReference] public BlackboardVariable<GameObject> Star;

    public override bool IsTrue()
    {
        //Access scarelist from dreamer sight
        if (SawScare.Value || DreamerSight.Value.Scares.Count > 0)
        {
            //There's a scare spotted, determine what to pick
            GameObject current = null;
            float currentDistance = Mathf.Infinity;

            foreach (GameObject scare in DreamerSight.Value.Scares)
            {
                float distance = Vector3.Distance(Star.Value.transform.position, scare.transform.position); 
                if (distance < currentDistance)
                {
                    current = scare;
                    currentDistance = distance;
                }
            }

            //If current isn't null, update scare
            if (current)
            {
                Scare.Value = current;
                SawScare.Value = true;
                return true;
            }
            else if (Scare.Value && SawScare.Value)
            {
                return true;
            }      
        }

        //Both current and scare are null, there's nothing to be seen
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
