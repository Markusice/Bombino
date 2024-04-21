shader_type canvas_item;
uniform sampler2D tex : filter_nearest;
uniform bool is_hovered;

void fragment() {
    vec4 pixelColor = texture(tex, UV * 1.2 - 0.1);
	
	if (is_hovered && (UV.y < 0.04 || UV.y > 0.96)) {
		pixelColor.rgb = vec3(0.902,0.659,0.09);
	}
	
	COLOR = pixelColor;
}
