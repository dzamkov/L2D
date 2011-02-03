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
uniform sampler2D Bloom0; uniform float W0;
uniform sampler2D Bloom1; uniform float W1;
uniform sampler2D Bloom2; uniform float W2;
uniform sampler2D Bloom3; uniform float W3;
uniform sampler2D Bloom4; uniform float W4; 
uniform sampler2D Bloom5; uniform float W5;
uniform sampler2D Bloom6; uniform float W6;
uniform sampler2D Bloom7; uniform float W7;

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
	if(W0 > 0.0) col += texture2D(Bloom0, Coords).rgb * W0;
	if(W1 > 0.0) col += texture2D(Bloom1, Coords).rgb * W1;
	if(W2 > 0.0) col += texture2D(Bloom2, Coords).rgb * W2;
	if(W3 > 0.0) col += texture2D(Bloom3, Coords).rgb * W3;
	if(W4 > 0.0) col += texture2D(Bloom4, Coords).rgb * W4;
	if(W5 > 0.0) col += texture2D(Bloom5, Coords).rgb * W5;
	if(W6 > 0.0) col += texture2D(Bloom6, Coords).rgb * W6;
	if(W7 > 0.0) col += texture2D(Bloom7, Coords).rgb * W7;
	gl_FragColor = vec4(HDR(col), 1.0);
}
#endif