using Trivial.Graph.Components.Controls;
using Trivial.Graph.Domain;
using Trivial.Graph.Domain.Controls.Default;
using Trivial.Graph.Domain.Models.Base;
using Trivial.Graph.Options;

namespace Trivial.Graph;

public class BlazorDiagram : Diagram
{
    private readonly Dictionary<Type, Type> m_ComponentsMapping;

    public BlazorDiagram(BlazorDiagramOptions? Options = null, bool RegisterDefaultBehaviors = true) : base(RegisterDefaultBehaviors)
    {
        m_ComponentsMapping = new Dictionary<Type, Type>
        {
            [typeof(RemoveControl)] = typeof(RemoveControlWidget),
            [typeof(BoundaryControl)] = typeof(BoundaryControlWidget),
            [typeof(DragNewLinkControl)] = typeof(DragNewLinkControlWidget),
            [typeof(ArrowHeadControl)] = typeof(ArrowHeadControlWidget)
        };

        this.Options = Options ?? new BlazorDiagramOptions();
    }

    public override BlazorDiagramOptions Options { get; }

    public void RegisterComponent<TModel, TComponent>(bool Replace = false)
    {
        RegisterComponent(typeof(TModel), typeof(TComponent), Replace);
    }

    public void RegisterComponent(Type ModelType, Type ComponentType, bool Replace = false)
    {
        if (!Replace && m_ComponentsMapping.ContainsKey(ModelType))
            throw new Exception($"Component already registered for model '{ModelType.Name}'.");

        m_ComponentsMapping[ModelType] = ComponentType;
    }

    public Type? GetComponent(Type ModelType, bool CheckSubclasses = true)
    {
        if (m_ComponentsMapping.ContainsKey(ModelType))
            return m_ComponentsMapping[ModelType];

        if (!CheckSubclasses)
            return null;
        
        foreach (var t_Rmt in m_ComponentsMapping.Keys)
        {
            if (ModelType.IsSubclassOf(t_Rmt))
                return m_ComponentsMapping[t_Rmt];
        }

        return null;
    }

    public Type? GetComponent<TModel>(bool CheckSubclasses = true)
    {
        return GetComponent(typeof(TModel), CheckSubclasses);
    }

    public Type? GetComponent(Model Model, bool CheckSubclasses = true)
    {
        return GetComponent(Model.GetType(), CheckSubclasses);
    }
}