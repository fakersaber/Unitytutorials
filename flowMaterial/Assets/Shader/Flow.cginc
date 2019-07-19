#ifndef FLOW_INCLUDED
#define FLOW_INCLUDED

float3 flowUV(
	float2 uv, float2 flowVector, float2 jump, 
	float tiling, float time,bool flowB
) {
	//ʹ��(0,0)��(1,1)�ı任 Ϊ(0,0)��(0.5,1) ��(0.5,1)��(1,1)
    //0 = k * 0 + b         1 = k * 0.5 +b;
    //1 = k * 0.5 + b;      0 = k * 1 + b;
    //y = 2x                y = -2x + 2

	//ԭ��������x= 0.5�ԳƵĺ���Ϊ 2-2x
	float phaseOffset = flowB ? 0.5 : 0;
	float progress = frac(time + phaseOffset);
	float3 uvw;
	//��ʾ��ǰ�ƶ�
	uvw.xy = uv - flowVector * progress;
	uvw.xy *= tiling;
	uvw.xy += phaseOffset;
	uvw.xy += (time - progress) * jump;
	//����ϵ�������浽zֵ��
	uvw.z = 1 - abs(1 - 2 * progress);
	return uvw;
}

#endif