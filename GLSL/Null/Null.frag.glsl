#version 460 core
in vec2 FragmentUV;

out vec4 FinalColor;

uniform sampler2D Texture;

void main() {
	FinalColor = texture(Texture, FragmentUV);
}