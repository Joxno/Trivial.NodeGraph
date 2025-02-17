using System.Numerics;
using Trivial.Domain.Behaviors;
using Trivial.Domain.Extensions;
using Trivial.Domain.Geometry;
using Trivial.Domain.Layers;
using Trivial.Domain.Models.Base;
using Trivial.Domain.Events;
using System.Runtime.CompilerServices;
using Trivial.Domain.Options;
using Trivial.Domain.Controls;

[assembly: InternalsVisibleTo("Trivial.Graph")]
[assembly: InternalsVisibleTo("Trivial.Graph.Tests")]
[assembly: InternalsVisibleTo("Trivial.Domain.Tests")]

namespace Trivial.Domain;

public abstract class Diagram
{
    private readonly Dictionary<Type, Behavior> m_Behaviors;
    private readonly List<SelectableModel> m_OrderedSelectables;

    public event Action<Model?, PointerEventArgs>? PointerDown;
    public event Action<Model?, PointerEventArgs>? PointerMove;
    public event Action<Model?, PointerEventArgs>? PointerUp;
    public event Action<Model?, PointerEventArgs>? PointerEnter;
    public event Action<Model?, PointerEventArgs>? PointerLeave;
    public event Action<KeyboardEventArgs>? KeyDown;
    public event Action<WheelEventArgs>? Wheel;
    public event Action<Model?, PointerEventArgs>? PointerClick;
    public event Action<Model?, PointerEventArgs>? PointerfloatClick;

    public event Action<SelectableModel>? SelectionChanged;
    public event Action? PanChanged;
    public event Action? ZoomChanged;
    public event Action? ContainerChanged;
    public event Action? Changed;

    protected Diagram(bool RegisterDefaultBehaviors = true)
    {
        m_Behaviors = new Dictionary<Type, Behavior>();
        m_OrderedSelectables = new List<SelectableModel>();

        Nodes = new NodeLayer(this);
        Links = new LinkLayer(this);
        Groups = new GroupLayer(this);
        Controls = new ControlsLayer();

        Nodes.Added += OnSelectableAdded;
        Links.Added += OnSelectableAdded;
        Groups.Added += OnSelectableAdded;

        Nodes.Removed += OnSelectableRemoved;
        Links.Removed += OnSelectableRemoved;
        Groups.Removed += OnSelectableRemoved;

        if (!RegisterDefaultBehaviors)
            return;

        RegisterBehavior(new SelectionBehavior(this));
        RegisterBehavior(new DragMovablesBehavior(this));
        RegisterBehavior(new DragNewLinkBehavior(this));
        RegisterBehavior(new PanBehavior(this));
        RegisterBehavior(new ZoomBehavior(this));
        RegisterBehavior(new EventsBehavior(this));
        RegisterBehavior(new KeyboardShortcutsBehavior(this));
        RegisterBehavior(new ControlsBehavior(this));
        RegisterBehavior(new VirtualizationBehavior(this));
    }

    public abstract DiagramOptions Options { get; }
    public NodeLayer Nodes { get; }
    public LinkLayer Links { get; }
    public GroupLayer Groups { get; }
    public ControlsLayer Controls { get; }
    public Rectangle? Container { get; private set; }
    public Vector2 Pan { get; private set; } = Vector2.Zero;
    public float Zoom { get; private set; } = 1;
    public bool SuspendRefresh { get; set; }
    public bool SuspendSorting { get; set; }
    public IReadOnlyList<SelectableModel> OrderedSelectables => m_OrderedSelectables;

    public void Refresh()
    {
        if (SuspendRefresh)
            return;

        Changed?.Invoke();
    }

    public void Batch(Action Action)
    {
        if (SuspendRefresh)
        {
            // If it's already suspended, just execute the action and leave it suspended
            // It's probably handled by an outer batch
            Action();
            return;
        }

        SuspendRefresh = true;
        Action();
        SuspendRefresh = false;
        Refresh();
    }

    #region Selection

    public IEnumerable<SelectableModel> GetSelectedModels()
    {
        foreach (var t_Node in Nodes)
        {
            if (t_Node.Selected)
                yield return t_Node;
        }

        foreach (var t_Link in Links)
        {
            if (t_Link.Selected)
                yield return t_Link;

            foreach (var t_Vertex in t_Link.Vertices)
            {
                if (t_Vertex.Selected)
                    yield return t_Vertex;
            }
        }

        foreach (var t_Group in Groups)
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

    #endregion

    #region Behaviors

    public void RegisterBehavior(Behavior Behavior)
    {
        var t_Type = Behavior.GetType();
        if (m_Behaviors.ContainsKey(t_Type))
            throw new Exception($"Behavior '{t_Type.Name}' already registered");

        m_Behaviors.Add(t_Type, Behavior);
    }

    public T? GetBehavior<T>() where T : Behavior
    {
        var t_Type = typeof(T);
        return (T?)(m_Behaviors.ContainsKey(t_Type) ? m_Behaviors[t_Type] : null);
    }

    public void UnregisterBehavior<T>() where T : Behavior
    {
        var t_Type = typeof(T);
        if (!m_Behaviors.ContainsKey(t_Type))
            return;

        m_Behaviors[t_Type].Dispose();
        m_Behaviors.Remove(t_Type);
    }

    #endregion

    public void ZoomToFit(float Margin = 10)
    {
        if (Container == null || Nodes.Count == 0)
            return;

        var t_SelectedNodes = Nodes.Where(S => S.Selected);
        var t_NodesToUse = t_SelectedNodes.Any() ? t_SelectedNodes : Nodes;
        var t_Bounds = t_NodesToUse.GetBounds();
        var t_Width = t_Bounds.Width + 2 * Margin;
        var t_Height = t_Bounds.Height + 2 * Margin;
        var t_MinX = t_Bounds.Left - Margin;
        var t_MinY = t_Bounds.Top - Margin;

        SuspendRefresh = true;

        var t_Xf = Container.Width / t_Width;
        var t_Yf = Container.Height / t_Height;
        SetZoom(MathF.Min(t_Xf, t_Yf));

        var t_Nx = Container.Left + Pan.X + t_MinX * Zoom;
        var t_Ny = Container.Top + Pan.Y + t_MinY * Zoom;
        UpdatePan(Container.Left - t_Nx, Container.Top - t_Ny);

        SuspendRefresh = false;
        Refresh();
    }

    public void SetPan(float X, float Y)
    {
        Pan = new Vector2(X, Y);
        PanChanged?.Invoke();
        Refresh();
    }

    public void UpdatePan(float DeltaX, float DeltaY)
    {
        Pan += new Vector2(DeltaX, DeltaY);
        PanChanged?.Invoke();
        Refresh();
    }

    public void SetZoom(float NewZoom)
    {
        if (NewZoom <= 0)
            throw new ArgumentException($"{nameof(NewZoom)} cannot be equal or lower than 0");

        if (NewZoom < Options.Zoom.Minimum)
            NewZoom = Options.Zoom.Minimum;

        Zoom = NewZoom;
        ZoomChanged?.Invoke();
        Refresh();
    }

    public void SetContainer(Rectangle NewRect)
    {
        if (NewRect.Equals(Container))
            return;

        Container = NewRect;
        ContainerChanged?.Invoke();
        Refresh();
    }

    public Vector2 GetRelativeMousePoint(float ClientX, float ClientY)
    {
        if (Container == null)
            throw new Exception(
                "Container not available. Make sure you're not using this method before the diagram is fully loaded");

        return new Vector2((ClientX - Container.Left - Pan.X) / Zoom, (ClientY - Container.Top - Pan.Y) / Zoom);
    }

    public Vector2 GetRelativePoint(float ClientX, float ClientY)
    {
        if (Container == null)
            throw new Exception(
                "Container not available. Make sure you're not using this method before the diagram is fully loaded");

        return new Vector2(ClientX - Container.Left, ClientY - Container.Top);
    }

    public Vector2 GetScreenPoint(float ClientX, float ClientY)
    {
        if (Container == null)
            throw new Exception(
                "Container not available. Make sure you're not using this method before the diagram is fully loaded");

        return new Vector2(Zoom * ClientX + Container.Left + Pan.X, Zoom * ClientY + Container.Top + Pan.Y);
    }

    #region Ordering

    public void SendToBack(SelectableModel Model)
    {
        var t_MinOrder = GetMinOrder();
        if (Model.Order == t_MinOrder)
            return;

        if (!m_OrderedSelectables.Remove(Model))
            return;

        m_OrderedSelectables.Insert(0, Model);

        // Todo: can optimize this by only updating the order of items before model
        Batch(() =>
        {
            SuspendSorting = true;
            for (var t_I = 0; t_I < m_OrderedSelectables.Count; t_I++)
            {
                m_OrderedSelectables[t_I].Order = t_I + 1;
            }
            SuspendSorting = false;
        });
    }

    public void SendToFront(SelectableModel Model)
    {
        var t_MaxOrder = GetMaxOrder();
        if (Model.Order == t_MaxOrder)
            return;

        if (!m_OrderedSelectables.Remove(Model))
            return;

        m_OrderedSelectables.Add(Model);

        SuspendSorting = true;
        Model.Order = t_MaxOrder + 1;
        SuspendSorting = false;
        Refresh();
    }

    public int GetMinOrder()
    {
        return m_OrderedSelectables.Count > 0 ? m_OrderedSelectables[0].Order : 0;
    }

    public int GetMaxOrder()
    {
        return m_OrderedSelectables.Count > 0 ? m_OrderedSelectables[^1].Order : 0;
    }

    /// <summary>
    /// Sorts the list of selectables based on their order
    /// </summary>
    public void RefreshOrders(bool Refresh = true)
    {
        m_OrderedSelectables.Sort((A, B) => A.Order.CompareTo(B.Order));
        
        if (Refresh)
        {
            this.Refresh();
        }
    }

    private void OnSelectableAdded(SelectableModel Model)
    {
        var t_MaxOrder = GetMaxOrder();
        m_OrderedSelectables.Add(Model);

        if (Model.Order == 0)
        {
            Model.Order = t_MaxOrder + 1;
        }

        Model.OrderChanged += OnModelOrderChanged;
    }

    private void OnSelectableRemoved(SelectableModel Model)
    {
        Model.OrderChanged -= OnModelOrderChanged;
        m_OrderedSelectables.Remove(Model);
    }

    private void OnModelOrderChanged(Model Model)
    {
        if (SuspendSorting)
            return;

        RefreshOrders();
    }

    #endregion

    #region Events

    public void TriggerPointerDown(Model? Model, PointerEventArgs E) => PointerDown?.Invoke(Model, E);

    public void TriggerPointerMove(Model? Model, PointerEventArgs E) => PointerMove?.Invoke(Model, E);

    public void TriggerPointerUp(Model? Model, PointerEventArgs E) => PointerUp?.Invoke(Model, E);

    public void TriggerPointerEnter(Model? Model, PointerEventArgs E) => PointerEnter?.Invoke(Model, E);

    public void TriggerPointerLeave(Model? Model, PointerEventArgs E) => PointerLeave?.Invoke(Model, E);

    public void TriggerKeyDown(KeyboardEventArgs E) => KeyDown?.Invoke(E);

    public void TriggerWheel(WheelEventArgs E) => Wheel?.Invoke(E);

    public void TriggerPointerClick(Model? Model, PointerEventArgs E) => PointerClick?.Invoke(Model, E);

    public void TriggerPointerfloatClick(Model? Model, PointerEventArgs E) => PointerfloatClick?.Invoke(Model, E);

    #endregion
}