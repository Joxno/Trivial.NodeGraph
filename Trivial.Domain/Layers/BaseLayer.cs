using Trivial.Domain.Models.Base;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Trivial.Domain;

public abstract class BaseLayer<T> : IReadOnlyList<T> where T : Model
{
    private readonly List<T> m_Items = new List<T>();

    public event Action<T>? Added;
    public event Action<T>? Removed;

    public BaseLayer(Diagram Diagram)
    {
        this.Diagram = Diagram;
    }

    public virtual TSpecific Add<TSpecific>(TSpecific Item) where TSpecific : T
    {
        if (Item is null)
            throw new ArgumentNullException(nameof(Item));

        Diagram.Batch(() =>
        {
            m_Items.Add(Item);
            OnItemAdded(Item);
            Added?.Invoke(Item);
        });
        return Item;
    }

    public virtual void Add(IEnumerable<T> Items)
    {
        if (Items is null)
            throw new ArgumentNullException(nameof(Items));

        Diagram.Batch(() =>
        {
            foreach (var t_Item in Items)
            {
                m_Items.Add(t_Item);
                OnItemAdded(t_Item);
                Added?.Invoke(t_Item);
            }
        });
    }

    public virtual void Remove(T Item)
    {
        if (Item is null)
            throw new ArgumentNullException(nameof(Item));

        if (m_Items.Remove(Item))
        {
            Diagram.Batch(() =>
            {
                OnItemRemoved(Item);
                Removed?.Invoke(Item);
            });
        }
    }

    public virtual void Remove(IEnumerable<T> Items)
    {
        if (Items is null)
            throw new ArgumentNullException(nameof(Items));

        Diagram.Batch(() =>
        {
            foreach (var t_Item in Items)
            {
                if (m_Items.Remove(t_Item))
                {
                    OnItemRemoved(t_Item);
                    Removed?.Invoke(t_Item);
                }
            }
        });
    }

    public bool Contains(T Item) => m_Items.Contains(Item);

    public void Clear()
    {
        if (Count == 0)
            return;

        Diagram.Batch(() =>
        {
            for (var t_I = m_Items.Count - 1; t_I >= 0; t_I--)
            {
                var t_Item = m_Items[t_I];
                m_Items.RemoveAt(t_I);
                OnItemRemoved(t_Item);
                Removed?.Invoke(t_Item);
            }
        });
    }

    protected virtual void OnItemAdded(T Item) { }

    protected virtual void OnItemRemoved(T Item) { }

    public Diagram Diagram { get; }

    public int Count => m_Items.Count;
    public T this[int Index] => m_Items[Index];
    public IEnumerator<T> GetEnumerator() => m_Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => m_Items.GetEnumerator();
}
