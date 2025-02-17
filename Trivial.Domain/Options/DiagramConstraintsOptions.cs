using System;
using System.Threading.Tasks;
using Trivial.Domain.Models;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Options;

public class DiagramConstraintsOptions
{
    public Func<NodeModel, ValueTask<bool>> ShouldDeleteNode { get; set; } = _ => ValueTask.FromResult(true);
    public Func<BaseLinkModel, ValueTask<bool>> ShouldDeleteLink { get; set; } = _ => ValueTask.FromResult(true);
    public Func<GroupModel, ValueTask<bool>> ShouldDeleteGroup { get; set; } = _ => ValueTask.FromResult(true);
}