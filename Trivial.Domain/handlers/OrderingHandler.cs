using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.handlers;

public class OrderingHandler(Diagram Graph)
{
    public List<SelectableModel> OrderedSelectables { get; init; } = new();

    public void SendToBack(SelectableModel Model)
    {
        var t_MinOrder = GetMinOrder();
        if (Model.Order == t_MinOrder)
            return;

        if (!OrderedSelectables.Remove(Model))
            return;

        OrderedSelectables.Insert(0, Model);

        // Todo: can optimize this by only updating the order of items before model
        Graph.Batch(() =>
        {
            Graph.SuspendSorting = true;
            for (var t_I = 0; t_I < OrderedSelectables.Count; t_I++)
            {
                OrderedSelectables[t_I].Order = t_I + 1;
            }
            Graph.SuspendSorting = false;
        });
    }

    public void SendToFront(SelectableModel Model)
    {
        var t_MaxOrder = GetMaxOrder();
        if (Model.Order == t_MaxOrder)
            return;

        if (!OrderedSelectables.Remove(Model))
            return;

        OrderedSelectables.Add(Model);

        Graph.SuspendSorting = true;
        Model.Order = t_MaxOrder + 1;
        Graph.SuspendSorting = false;
        Graph.Refresh();
    }

    public int GetMinOrder()
    {
        return OrderedSelectables.Count > 0 ? OrderedSelectables[0].Order : 0;
    }

    public int GetMaxOrder()
    {
        return OrderedSelectables.Count > 0 ? OrderedSelectables[^1].Order : 0;
    }

    public void RefreshOrders(bool Refresh = true)
    {
        OrderedSelectables.Sort((A, B) => A.Order.CompareTo(B.Order));
        
        if (Refresh)
        {
            Graph.Refresh();
        }
    }

    public void OnSelectableAdded(SelectableModel Model)
    {
        var t_MaxOrder = GetMaxOrder();
        OrderedSelectables.Add(Model);

        if (Model.Order == 0)
        {
            Model.Order = t_MaxOrder + 1;
        }

        Model.OrderChanged += OnModelOrderChanged;
    }

    public void OnSelectableRemoved(SelectableModel Model)
    {
        Model.OrderChanged -= OnModelOrderChanged;
        OrderedSelectables.Remove(Model);
    }

    public void OnModelOrderChanged(Model Model)
    {
        if (Graph.SuspendSorting)
            return;

        RefreshOrders();
    }
}