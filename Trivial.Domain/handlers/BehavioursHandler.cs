using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.handlers;

public class BehavioursHandler(Diagram Graph)
{
    private readonly Dictionary<Type, BaseBehaviour> m_Behaviors = new();

    public void RegisterBehavior(BaseBehaviour Behavior, bool Force = false)
    {
        var t_Type = Behavior.GetType();
        if (m_Behaviors.ContainsKey(t_Type) && !Force)
            throw new Exception($"Behavior '{t_Type.Name}' already registered");

        if(Force)
            m_Behaviors.Retrieve(t_Type).Then(B => B.Dispose());

        m_Behaviors[t_Type] = Behavior;
    }

    public T? GetBehavior<T>() where T : BaseBehaviour =>
        (T?)m_Behaviors.Retrieve(typeof(T)).ValueOr(null);
        
    public void UnregisterBehavior<T>() where T : BaseBehaviour =>
        m_Behaviors.Retrieve(typeof(T)).Then(B => {
            m_Behaviors[typeof(T)].Dispose();
            m_Behaviors.Remove(typeof(T));
        });

    public bool HasBehaviour<T>() where T : BaseBehaviour => m_Behaviors.ContainsKey(typeof(T));
}