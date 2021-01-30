using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class ShopNode : DialogNode
{
    [Input(backingValue = ShowBackingValue.Never)] public DialogNode input;
    [Output(backingValue = ShowBackingValue.Never)] public DialogNode output;
    public ShopCart[] shopCarts;

    public enum ShopActionType
    {
        Buy,
        Sell,
        Give
    }

    [System.Serializable]
    public struct ShopCart
    {
        public ShopActionType actionType;
        public ItemScriptableObject item;
    }

	protected override void Init()
    {
		base.Init();
        DialogType = Dialog.Shop;
	}

	public override object GetValue(NodePort port)
    {
		return null;
	}
}

