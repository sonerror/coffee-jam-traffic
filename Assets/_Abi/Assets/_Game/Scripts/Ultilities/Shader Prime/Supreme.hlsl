



void GetUvPos_float(float2 pos, float dirtWidth, float dirtHeight,float brushWidth, float brushHeight,float2 ObjectSize, float2 vertPos, out float2 uvPos){
    uvPos = float2(pos.x/dirtWidth*2-1 + (vertPos.x/ObjectSize.x)*brushWidth/dirtWidth*2-1,
                                pos.y/dirtHeight*2-1 + (vertPos.y/ObjectSize.y)*brushHeight/dirtHeight*2-1);
}

void CheckTexture_float(bool isClear, float check)
{
    isClear = isClear & (check<0.01);
}

float _PixelCount;