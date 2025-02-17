using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.handlers;

public class SelectionHandler(Diagram Graph)
{
    public event Action<SelectableModel>? SelectionChanged;
    
    public IEnumerable<SelectableModel> GetSelectedModels()
    {
        foreach (var t_Node in Graph.Nodes)
        {
            if (t_Node.Selected)
                yield return t_Node;
        }

        foreach (var t_Link in Graph.Links)
        {
            if (t_Link.Selected)
                yield return t_Link;

            foreach (var t_Vertex in t_Link.Vertices)
            {
                if (t_Vertex.Selected)
                    yield return t_Vertex;
            }
        }

        foreach (var t_Group in Graph.Groups)
        {
            if (t_Group.Selected)
                yield return t_Group;
        }
    }

    public void SelectModel(SelectableModel Model, bool UnselectOthers)
    {
        if (Model.Selected)
            return;

        if (UnselectOthers)
            UnselectAll();

        Model.Selected = true;
        Model.Refresh();
        SelectionChanged?.Invoke(Model);
    }

    public void UnselectModel(SelectableModel Model)
    {
        if (!Model.Selected)
            return;

        Model.Selected = false;
        Model.Refresh();
        SelectionChanged?.Invoke(Model);
    }

    public void UnselectAll()
    {
        foreach (var t_Model in GetSelectedModels())
        {
            t_Model.Selected = false;
            t_Model.Refresh();
            // Todo: will result in many events, maybe one event for all of them?
            SelectionChanged?.Invoke(t_Model);
        }
    }
}