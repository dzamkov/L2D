uniform sampler2D Source;
uniform float PixelSpacing;
varying vec2 Coords;

#ifdef _VERTEX_
void main()
{
	Coords = gl_Vertex.xy * 0.5 + 0.5;
	gl_Position = gl_Vertex;
}
#endif

#ifdef _FRAGMENT_
void main()
{
#ifdef VERTICAL
	vec3 col = 
		0.0625 * texture2D(Source, vec2(Coords.x, Coords.y - PixelSpacing * 2.0)).rgb +
		0.2500 * texture2D(Source, vec2(Coords.x, Coords.y - PixelSpacing)).rgb +
		0.3750 * texture2D(Source, vec2(Coords.x, Coords.y)).rgb +
		0.2500 * texture2D(Source, vec2(Coords.x, Coords.y + PixelSpacing)).rgb +
		0.0625 * texture2D(Source, vec2(Coords.x, Coords.y + PixelSpacing * 2.0)).rgb;
#else
	vec3 col = 
		0.0625 * texture2D(Source, vec2(Coords.x - PixelSpacing * 2.0, Coords.y)).rgb +
		0.2500 * texture2D(Source, vec2(Coords.x - PixelSpacing, Coords.y)).rgb +
		0.3750 * texture2D(Source, vec2(Coords.x, Coords.y)).rgb +
		0.2500 * texture2D(Source, vec2(Coords.x + PixelSpacing, Coords.y)).rgb +
		0.0625 * texture2D(Source, vec2(Coords.x + PixelSpacing * 2.0, Coords.y)).rgb;
#endif
	gl_FragColor = vec4(col, 1.0);
}
#endif