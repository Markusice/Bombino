using Bombino.map;

namespace Bombino.ui.maps_menu;

/// <summary>
/// Represents a button used for selecting a map in the UI.
/// </summary>
internal partial class MapBasicButton : MapButton
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapBasicButton"/> class.
    /// </summary>
    private MapBasicButton()
    {
        MapType = MapType.Basic;
    }
}
