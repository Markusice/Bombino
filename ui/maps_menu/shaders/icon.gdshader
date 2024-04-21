shader_type canvas_item;
uniform sampler2D tex : filter_nearest;

void fragment() {
    vec4 pixelColor = texture(tex, UV);
	
	COLOR = pixelColor;
}
