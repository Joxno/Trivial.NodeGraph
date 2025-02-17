using System;
using System.Threading.Tasks;
using Trivial.Domain.Geometry;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Trivial.Graph.Extensions;

public static class JsRuntimeExtensions
{
    public static async Task<Rectangle> GetBoundingClientRect(this IJSRuntime JsRuntime, ElementReference Element)
    {
        return await JsRuntime.InvokeAsync<Rectangle>("ZBlazorDiagrams.getBoundingClientRect", Element);
    }

    public static async Task ObserveResizes<T>(this IJSRuntime JsRuntime, ElementReference Element,
        DotNetObjectReference<T> Reference) where T : class
    {
        try
        {
            await JsRuntime.InvokeVoidAsync("ZBlazorDiagrams.observe", Element, Reference, Element.Id);
        }
        catch (ObjectDisposedException)
        {
            // Ignore, DotNetObjectReference was likely disposed
        }
    }

    public static async Task UnobserveResizes(this IJSRuntime JsRuntime, ElementReference Element)
    {
        await JsRuntime.InvokeVoidAsync("ZBlazorDiagrams.unobserve", Element, Element.Id);
    }
}