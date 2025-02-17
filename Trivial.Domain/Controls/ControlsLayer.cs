using System;
using System.Collections.Generic;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Controls;

public class ControlsLayer
{
    private readonly Dictionary<Model, ControlsContainer> m_Containers;

    public event Action<Model>? ChangeCaused;

    public ControlsLayer()
    {
        m_Containers = new Dictionary<Model, ControlsContainer>();
    }

    public IReadOnlyCollection<Model> Models => m_Containers.Keys;

    public ControlsContainer AddFor(Model Model, ControlsType Type = ControlsType.OnSelection)
    {
        if (m_Containers.ContainsKey(Model))
            return m_Containers[Model];
        
        var t_Container = new ControlsContainer(Model, Type);
        t_Container.VisibilityChanged += OnVisibilityChanged;
        t_Container.ControlsChanged += RefreshIfVisible;
        Model.Changed += RefreshIfVisible;
        m_Containers.Add(Model, t_Container);
        return t_Container;
    }

    public ControlsContainer? GetFor(Model Model)
    {
        return m_Containers.TryGetValue(Model, out var t_Container) ? t_Container : null;
    }

    public bool RemoveFor(Model Model)
    {
        if (!m_Containers.TryGetValue(Model, out var t_Container))
            return false;
        
        t_Container.VisibilityChanged -= OnVisibilityChanged;
        t_Container.ControlsChanged -= RefreshIfVisible;
        Model.Changed -= RefreshIfVisible;
        m_Containers.Remove(Model);
        ChangeCaused?.Invoke(Model);
        return true;
    }

    public bool AreVisibleFor(Model Model) => GetFor(Model)?.Visible ?? false;

    private void RefreshIfVisible(Model Cause)
    {
        if (!AreVisibleFor(Cause))
            return;
        
        ChangeCaused?.Invoke(Cause);
    }

    private void OnVisibilityChanged(Model Cause) => ChangeCaused?.Invoke(Cause);
}