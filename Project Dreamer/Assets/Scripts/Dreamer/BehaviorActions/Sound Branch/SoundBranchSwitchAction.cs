using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SoundBranchSwitch", story: "End branch, sound handled by other sound branch", category: "Action", id: "bef0e9d429b1c6da4c33b1d24c7aa7c2")]
public partial class SoundBranchSwitchAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> IsHarshSound;

    [SerializeReference] public BlackboardVariable<Boolean> IsInvestigatingLoud;

    [SerializeReference] public BlackboardVariable<Boolean> IsInvestigatingSoft;
    protected override Status OnStart()
    {
        /* Return status depends on sound type. Since this is a mismatch handler, a harsh sound on soft branch should succeed,
         * ending the current tree check and looping back to hit the harsh branch. Conversely, a soft sound on a harsh branch should fail,
         * so the check continues down to the soft sound branch.
         */
        
        //Harsh sound on soft branch
        if (IsHarshSound.Value)
        {
            //Turn off the current branch's bool
            IsInvestigatingSoft.Value = false;
            return Status.Success;
        }

        //Soft sound on harsh branch
        IsInvestigatingLoud.Value = false;
        return Status.Failure;
    }

}

