using System.Numerics;
using Trivial.Graph.Domain.Behaviors;
using Trivial.Graph.Domain.Extensions;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Layers;
using Trivial.Graph.Domain.Models.Base;
using Trivial.Graph.Domain.Events;
using Trivial.Graph.Domain.Options;
using Trivial.Graph.Domain.Controls;
using Trivial.Graph.Domain.handlers;

namespace Trivial.Graph.Domain;

public abstract class Diagram
{
    private SelectionHandler m_SelectionHandler;
    private BehavioursHandler m_BehavioursHandler;
    private ZoomHandler m_ZoomHandler;
    private PanHandler m_PanHandler;
    private OrderingHandler m_OrderingHandler;
    private ContainerHandler m_ContainerHandler;

    public event Action<Model?, PointerEventArgs>? PointerDown;
    public event Action<Model?, PointerEventArgs>? PointerMove;
    public event Action<Model?, PointerEventArgs>? PointerUp;
    public event Action<Model?, PointerEventArgs>? PointerEnter;
    public event Action<Model?, PointerEventArgs>? PointerLeave;
    public event Action<KeyboardEventArgs>? KeyDown;
    public event Action<WheelEventArgs>? Wheel;
    public event Action<Model?, PointerEventArgs>? PointerClick;
    public event Action<Model?, PointerEventArgs>? PointerDoubleClick;

    public event Action<SelectableModel>? SelectionChanged;
    public event Action? PanChanged;
    public event Action? ZoomChanged;
    public event Action? ContainerChanged;
    public event Action? Changed;

    protected Diagram(bool RegisterDefaultBehaviors = true)
    {
        m_SelectionHandler = new(this);
        m_BehavioursHandler = new(this);
        m_ZoomHandler = new(this);
        m_PanHandler = new(this);
        m_OrderingHandler = new(this);
        m_ContainerHandler = new(this);

        m_ZoomHandler.ZoomChanged += () => ZoomChanged?.Invoke();
        m_PanHandler.PanChanged += () => PanChanged?.Invoke();
        m_ContainerHandler.ContainerChanged += () => ContainerChanged?.Invoke();
        m_SelectionHandler.SelectionChanged += M => SelectionChanged?.Invoke(M);

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

        if (RegisterDefaultBehaviors)
            this.AddDefaultBehaviours();
    }

    public abstract DiagramOptions Options { get; }
    public NodeLayer Nodes { get; }
    public LinkLayer Links { get; }
    public GroupLayer Groups { get; }
    public ControlsLayer Controls { get; }
    public Rectangle? Container { get; set; }
    public Vector2 Pan { get; set; } = Vector2.Zero;
    public float Zoom { get; set; } = 1;
    public bool SuspendRefresh { get; set; }
    public bool SuspendSorting { get; set; }
    public IReadOnlyList<SelectableModel> OrderedSelectables => m_OrderingHandler.OrderedSelectables;

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

    public IEnumerable<SelectableModel> GetSelectedModels() => m_SelectionHandler.GetSelectedModels();
    public void SelectModel(SelectableModel Model, bool UnselectOthers) => m_SelectionHandler.SelectModel(Model, UnselectOthers);
    public void UnselectModel(SelectableModel Model) => m_SelectionHandler.UnselectModel(Model);
    public void UnselectAll() => m_SelectionHandler.UnselectAll();

    public void RegisterBehavior(BaseBehaviour Behavior, bool Force = false) => m_BehavioursHandler.RegisterBehavior(Behavior, Force);
    public T? GetBehavior<T>() where T : BaseBehaviour => m_BehavioursHandler.GetBehavior<T>();
    public void UnregisterBehavior<T>() where T : BaseBehaviour => m_BehavioursHandler.UnregisterBehavior<T>();

    public void ZoomToFit(float Margin = 10) => m_ZoomHandler.ZoomToFit(Margin);
    public void SetZoom(float NewZoom) => m_ZoomHandler.SetZoom(NewZoom);
    public void SetPan(float X, float Y) => m_PanHandler.SetPan(X, Y);
    public void UpdatePan(float DeltaX, float DeltaY) => m_PanHandler.UpdatePan(DeltaX, DeltaY);
    public void SetContainer(Rectangle NewRect) => m_ContainerHandler.SetContainer(NewRect);
    public Vector2 GetRelativeMousePoint(float ClientX, float ClientY) => m_ContainerHandler.GetRelativeMousePoint(ClientX, ClientY);
    public Vector2 GetRelativePoint(float ClientX, float ClientY) => m_ContainerHandler.GetRelativePoint(ClientX, ClientY);
    public Vector2 GetScreenPoint(float ClientX, float ClientY) => m_ContainerHandler.GetScreenPoint(ClientX, ClientY);

    public void SendToBack(SelectableModel Model) => m_OrderingHandler.SendToBack(Model);
    public void SendToFront(SelectableModel Model) => m_OrderingHandler.SendToFront(Model);
    public int GetMinOrder() => m_OrderingHandler.GetMinOrder();
    public int GetMaxOrder() => m_OrderingHandler.GetMaxOrder();
    public void RefreshOrders(bool Refresh = true) => m_OrderingHandler.RefreshOrders(Refresh);
    private void OnSelectableAdded(SelectableModel Model) => m_OrderingHandler.OnSelectableAdded(Model);
    private void OnSelectableRemoved(SelectableModel Model) => m_OrderingHandler.OnSelectableRemoved(Model);
    private void OnModelOrderChanged(Model Model) => m_OrderingHandler.RefreshOrders();

    public void TriggerPointerDown(Model? Model, PointerEventArgs E) => PointerDown?.Invoke(Model, E);
    public void TriggerPointerMove(Model? Model, PointerEventArgs E) => PointerMove?.Invoke(Model, E);
    public void TriggerPointerUp(Model? Model, PointerEventArgs E) => PointerUp?.Invoke(Model, E);
    public void TriggerPointerEnter(Model? Model, PointerEventArgs E) => PointerEnter?.Invoke(Model, E);
    public void TriggerPointerLeave(Model? Model, PointerEventArgs E) => PointerLeave?.Invoke(Model, E);
    public void TriggerKeyDown(KeyboardEventArgs E) => KeyDown?.Invoke(E);
    public void TriggerWheel(WheelEventArgs E) => Wheel?.Invoke(E);
    public void TriggerPointerClick(Model? Model, PointerEventArgs E) => PointerClick?.Invoke(Model, E);
    public void TriggerPointerfloatClick(Model? Model, PointerEventArgs E) => PointerDoubleClick?.Invoke(Model, E);
}