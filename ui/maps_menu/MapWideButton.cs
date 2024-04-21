using Bombino.game;
using Bombino.map;
using Bombino.ui.scripts;
using Godot;

namespace Bombino.ui.maps_menu;

/// <summary>
/// Represents a button used for selecting a map in the UI.
/// </summary>
internal partial class MapWideButton : MapButton
{
    private MapWideButton()
    {
        MapType = MapType.Wide;
    }
}