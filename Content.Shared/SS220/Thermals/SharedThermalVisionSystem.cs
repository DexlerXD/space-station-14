// Original code github.com/CM-14 Licence MIT, EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt
using Content.Shared.Alert;

namespace Content.Shared.SS220.Thermals;

public abstract class SharedThermalVisonSystem : EntitySystem
{
    [Dependency] private readonly AlertsSystem _alerts = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<ThermalVisionComponent, ComponentStartup>(OnThermalStartup);
        SubscribeLocalEvent<ThermalVisionComponent, MapInitEvent>(OnThermalMapInit);
        SubscribeLocalEvent<ThermalVisionComponent, AfterAutoHandleStateEvent>(OnThermalHandle);
        SubscribeLocalEvent<ThermalVisionComponent, ComponentRemove>(OnThermalRemove);
    }

    private void OnThermalStartup(Entity<ThermalVisionComponent> ent, ref ComponentStartup args)
    {
        ThermalVisionChanged(ent);
    }

    private void OnThermalHandle(Entity<ThermalVisionComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        ThermalVisionChanged(ent);
    }

    private void OnThermalMapInit(Entity<ThermalVisionComponent> ent, ref MapInitEvent args)
    {
    
    }

    private void OnThermalRemove(Entity<ThermalVisionComponent> ent, ref ComponentRemove args)
    { 
    ThermalVisionRemoved(ent);
    }

    public void Toggle(Entity<ThermalVisionComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp))
            return;

        ent.Comp.State = ent.Comp.State switch
        {
            ThermalVisionState.Off => ThermalVisionState.Half,
            ThermalVisionState.Half => ThermalVisionState.Full,
            ThermalVisionState.Full => ThermalVisionState.Off,
            _ => throw new ArgumentOutOfRangeException() 
        };

        Dirty(ent);
    }

    protected virtual void ThermalVisionChanged(Entity<ThermalVisionComponent> ent)
    {
    }

    protected virtual void ThermalVisionRemoved(Entity<ThermalVisionComponent> ent)
    {
    }
}
