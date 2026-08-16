# DisableHover 1.3 Release Candidate Test Checklist

## Test setup

Enable only the real **DisableHover** mod. Disable all Discovery, AreaBuffer, OverlayIsolation, HideOverlay, TempSelect, and AreaColor test mods.

Test DH OFF first, then DH ON. DT can be tested separately.

## Must-pass DH checks

1. Hover a normal building: blue outline and flat blue projected footprint disappear with DH ON.
2. Click a building once: it selects on the first click.
3. Select a citizen/vehicle: the blue location/selection pin still appears.
4. Switch DH OFF: vanilla blue hover graphics return.
5. Toggle DH ON/OFF repeatedly without exceptions or log spam.

## Tool regression checks

Because projected suppression is restricted to `DefaultToolSystem`, these tools should keep their projected guidance. Still test each:

- Road placement and road upgrades
- Train track placement
- Bus line creation/editing
- Tram line creation/editing
- Metro line creation/editing
- Train line creation/editing
- Connect public-transport routes/pathways to stations
- Add/remove public-transport stops
- Building placement, relocation and bulldozer
- Zoning
- District/area tools
- Underground mode

## DT checks

- DT ON suppresses UI hover tooltips.
- DT OFF restores them.
- DH and DT work independently.
- DH/DT toolbar buttons reflect and toggle the corresponding options.

## Release blockers

Do not publish if any of these occur:

- A building needs two clicks to select.
- Transport lines cannot connect to stations/stops.
- Citizen/object selection pins disappear.
- Road/track/placement guidance disappears outside the normal selection tool.
- DH OFF fails to restore vanilla highlights.
- NullReferenceException or repeated warning/error spam appears.
