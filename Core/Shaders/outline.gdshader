shader_type canvas_item;

uniform vec4 outline_color : source_color = vec4(1.0);
uniform float outline_thickness : hint_range(0, 2) = 1.0;
uniform bool all_mode;

const vec2 OFFSETS_ALL[8] = {
	vec2(-1, -1), vec2(-1, 0), vec2(-1, 1), vec2(0, -1), vec2(0, 1), 
	vec2(1, -1), vec2(1, 0), vec2(1, 1)
};

const vec2 OFFSETS_CORNERS[4] = {
	vec2(-1, 0), vec2(0, -1), vec2(0, 1), vec2(1, 0)
};

void fragment() {
	vec2 size = TEXTURE_PIXEL_SIZE * outline_thickness * vec2(6, 6);
	float outline = 0.0;
	
	if(all_mode){
		for (int i = 0; i < OFFSETS_ALL.length(); i++) {
			outline += texture(TEXTURE, UV + size * OFFSETS_ALL[i]).a;
		}
	}
	else{
		for (int i = 0; i < OFFSETS_CORNERS.length(); i++) {
			outline += texture(TEXTURE, UV + size * OFFSETS_CORNERS[i]).a;
		}
	}
	

	outline = min(outline, 1.0);
	
	vec4 color = texture(TEXTURE, UV);
	COLOR = mix(color, outline_color, outline - color.a);
}