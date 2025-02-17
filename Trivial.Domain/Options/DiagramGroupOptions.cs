using Trivial.Graph.Domain.Models;

namespace Trivial.Graph.Domain.Options;

public class DiagramGroupOptions
{
    public bool Enabled { get; set; }
    
    public GroupFactory Factory { get; set; } = (Diagram, Children) => new GroupModel(Children);
}