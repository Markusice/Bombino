using Bombino.map;

namespace Bombino.ui.maps_menu;

/// <summary>
/// Represents a button used for selecting a map in the UI.
/// </summary>
internal partial class MapCrossButton : MapButton
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapCrossButton"/> class.
    /// </summary>
    private MapCrossButton()
    {
        MapType = MapType.Cross;
    }
}
