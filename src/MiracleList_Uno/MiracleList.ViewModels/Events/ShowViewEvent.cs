using System;

namespace MiracleList_WinUI.Events;

public record ShowViewEvent
{
    public ShowViewEvent(Type viewType)
    {
        ViewType = viewType;
    }

    public Type ViewType { get; }
}
