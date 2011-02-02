uniform sampler2D Source;
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
	gl_FragColor = vec4(texture2D(Source, Coords).rgb, 1.0);
}
#endif