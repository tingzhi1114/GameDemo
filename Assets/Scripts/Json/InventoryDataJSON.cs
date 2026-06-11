using System;
using System.Collections.Generic;

/// <summary>
/// JSON反序列化辅助类——JSON中一条背包条目
/// </summary>
[Serializable]
public class InventoryDataJSON
{
    public int character_id;
    public List<InventoryItemJSON> items;
}

/// <summary>
/// JSON反序列化辅助类——物品及数量条目
/// </summary>
[Serializable]
public class InventoryItemJSON
{
    public int item_id;
    public int count;
}

/// <summary>
/// JSON反序列化辅助类——JSON根容器
/// </summary>
[Serializable]
public class InventoryDataListJSON
{
    public List<InventoryDataJSON> inventories;
}
