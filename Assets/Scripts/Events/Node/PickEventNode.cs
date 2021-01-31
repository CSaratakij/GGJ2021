using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNode;

public class PickEventNode : DialogNode
{
    [Input(backingValue = ShowBackingValue.Never)] public DialogNode input;

    protected override void Init()
    {
        base.Init();
        DialogType = Dialog.PickEvent;
    }

    public override object GetValue(NodePort port)
    {
        return null;
    }
}
