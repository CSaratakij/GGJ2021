using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNode;

public class RelationshipNode : DialogNode
{
    [Input(backingValue = ShowBackingValue.Never)] public DialogNode input;
    [Output(backingValue = ShowBackingValue.Never)] public DialogNode output;
    public RelationshipAction[] actions;

    protected override void Init()
    {
        base.Init();
        DialogType = Dialog.Relationship;
    }

    public override object GetValue(NodePort port)
    {
        return null;
    }
}
