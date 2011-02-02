uniform sampler2D Source;
uniform float Exposure;

varying vec2 Coords;

#ifdef _VERTEX_
void main()
{
	Coords = gl_Vertex.xy * 0.5 + 0.5;
	gl_Position = gl_Vertex;
}
#endif



#ifdef _FRAGMENT_
uniform sampler2D Bloom;

vec3 HDR(vec3 L) {
	L *= Exposure;
    L.r = L.r < 1.413 ? pow(L.r * 0.38317, 1.0 / 2.2) : 1.0 - exp(-L.r);
    L.g = L.g < 1.413 ? pow(L.g * 0.38317, 1.0 / 2.2) : 1.0 - exp(-L.g);
    L.b = L.b < 1.413 ? pow(L.b * 0.38317, 1.0 / 2.2) : 1.0 - exp(-L.b);
    return L;
}

void main()
{
	vec3 col = texture2D(Source, Coords).rgb;
	gl_FragColor = vec4(HDR(col), 1.0);
}
#endif