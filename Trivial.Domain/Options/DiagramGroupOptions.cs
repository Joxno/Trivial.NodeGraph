using Trivial.Domain.Models;

namespace Trivial.Domain.Options;

public class DiagramGroupOptions
{
    public bool Enabled { get; set; }
    
    public GroupFactory Factory { get; set; } = (Diagram, Children) => new GroupModel(Children);
}