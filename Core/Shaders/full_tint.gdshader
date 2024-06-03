shader_type canvas_item;

uniform float tint_factor : hint_range(0, 1) = 0.0;
uniform vec3 tint_colour : source_color = vec3(1.0);

void fragment() {
	//FULL TINT
	COLOR = vec4(mix(COLOR.xyz, tint_colour.xyz, tint_factor), COLOR.a);
}