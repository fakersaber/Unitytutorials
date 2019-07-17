#ifndef FLOW_INCLUDED
#define FLOW_INCLUDED

float2 flowUV(float2 uv, float2 flowVector,float time) {
	float progress = frac(time);
	//显示向前移动
	return uv - progress * flowVector;
}

#endif