shader_type canvas_item;

uniform vec4 color;

void vertex() {
	COLOR += COLOR * 0.3;
	COLOR += color;
	COLOR.a = color.a;
	
	UV.x += (TIME * 0.02) * (sin(TIME * 2.0f) * 0.1);
	UV.y += TIME * 0.05;
}