#ifndef FLOW_INCLUDED
#define FLOW_INCLUDED

float2 flowUV(float2 uv, float2 flowVector,float time) {
	float progress = frac(time);
	//��ʾ��ǰ�ƶ�
	return uv - progress * flowVector;
}

#endif