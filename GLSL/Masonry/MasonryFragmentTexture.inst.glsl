#version 460 core
#extension GL_ARB_bindless_texture : enable
#extension GL_ARB_gpu_shader_int64 : enable

in vec2 FragmentUV;
flat in int InstID;

out vec4 FinalColor;

layout (std430, binding = 0) buffer TexSSBO {
	uint64_t Textures[];
};

void main() {
	sampler2D Sampler = sampler2D(Textures[InstID]);
	//float Color = float(Textures[InstID]);

	//FinalColor = vec4(0,0,0,1);
	FinalColor = texture(Sampler, FragmentUV);
	//FinalColor.w = 1;
	//FinalColor = vec4(FragmentUV.xy,texture(Sampler,FragmentUV).z,1);
	//FinalColor = vec4(1,0,0,Color);
}