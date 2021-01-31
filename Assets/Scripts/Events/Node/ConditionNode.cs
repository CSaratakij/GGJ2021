using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNode;

public class ConditionNode : DialogNode
{
    [Input(backingValue = ShowBackingValue.Never)] public DialogNode input;
    [Output(backingValue = ShowBackingValue.Never)] public DialogNode resulttrue;
    [Output(backingValue = ShowBackingValue.Never)] public DialogNode resultfalse;
    public ConditionAction[] actions;

    protected override void Init()
    {
        base.Init();
        DialogType = Dialog.Conditional;
    }

    public override object GetValue(NodePort port)
    {
        return null;
    }
}
