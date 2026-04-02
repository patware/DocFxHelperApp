using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Text;

namespace DocFxHelper.Infrastructure
{
  public class HierarchyDictionary<TKey, TValue> where TKey : notnull
  {
    public HierarchyDictionary()
    {
      Items = new Dictionary<TKey, HierarchyItem<TKey, TValue>>();
    }


    public HierarchyItem<TKey, TValue>? Root { get; set; }

    public IDictionary<TKey, HierarchyItem<TKey, TValue>> Items { get; init; } 

    public HierarchyItem<TKey, TValue> Add(TKey key, TValue item)
    {
      if (Root != null)
      {
        throw new InvalidOperationException("Already an item at root");
      }

      var newItem = new HierarchyItem<TKey, TValue>
      {
        Key = key,
        Item = item
      };

      Root = newItem;
      newItem.Root = Root;

      Items.Add(key, Root);

      return Root;
    }

    public HierarchyItem<TKey, TValue> Add(TKey parentKey, TKey itemKey, TValue item)
    {
      var parentItem = Items[parentKey];
      
      var childItem = new HierarchyItem<TKey,TValue>
      {
        Key = itemKey,
        Item = item,
        Parent = parentItem,
        Root = Root
      };

      parentItem.AddChild(childItem);

      Items.Add(itemKey, childItem);

      return childItem;

    }
  }

  public class HierarchyItem<Tkey, TValue> where Tkey: notnull
  {
    public required Tkey Key { get; init; }
    public required TValue Item { get; init; }

    public HierarchyItem<Tkey, TValue>? Root { get; set; }
    public HierarchyItem<Tkey, TValue>? Parent { get; set; }

    private List<HierarchyItem<Tkey, TValue>> _items = [];

    public IReadOnlyList<HierarchyItem<Tkey, TValue>> Children => _items;

    internal void AddChild(HierarchyItem<Tkey, TValue> child)
    {
      child.Parent = this;

      _items.Add(child);
    }

    public string Path(string divider)
    {
      if (Parent == null)
      {
        return string.Concat(divider, Key);
      }
      else
      {
        return string.Concat(Parent.Path(divider), divider, Key);
      }

    }
        
  }
}
