using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Variables
{
    //prefkey
    public const string PREF_KEY_LOAD_TIME_START = "PRF_LoadTime_Start";
    public const string PREF_KEY_LOAD_TIME_END = "PRF_LoadTime_End";

    //new
    public const string TAG_PLAYER = "Player";
    public const string TAG_KEY = "Key";
    public const string TAG_MINION = "Minion";
    public const string TIME_FORMAT = "HH:mm:ss"; //use for datetime.now

    public const double TIME_SPIN_THRESHOLD = 14400;
    public static System.DateTime rootDate = new System.DateTime(2020, 1, 1);
    public const double TIME_SPACE_SECOND = 1;
    public const double TIME_SPACE_MINUTE = 60;
    public const double TIME_SPACE_5_MINUTE = 300;
    public const double TIME_SPACE_HALF_HOUR = 1800;
    public const double TIME_SPACE_HOUR = 3600;
    public const double TIME_SPACE_DAY = 86400;
    public const double TIME_SPACE_HALF_DAY = 43200;
    public const double TIME_SPACE_WEEK = 604800;

    public const string TO_STRING_FLOAT_INT_FORMAT = "0";
    public const string TO_STRING_FLOAT_ONE_DIGIT = "0.0";

    public const string LEVEL = "Level ";
    public const string MAX = "Max";
    public const string SLASH = "/";
    public const string PRE_ICON = "<sprite=0> ";
    public const string MAX_LEVEL = "Max Level";
    public const string VND = " VND";

    public const int LAYER_MASK_DRAWABLE = 1 << 7;

    public const string BLIT_DIRT_WIDTH = "_DirtWidth";
    public const string BLIT_DIRT_HEIGHT = "_DirtHeight";
    public const string BLIT_BRUSH_WIDTH = "_BrushWidth";
    public const string BLIT_BRUSH_HEIGHT = "_BrushHeight";
    public const string BLIT_POS = "_Pos";

    public static readonly int GLOBAL_PIXEL_COUNT = Shader.PropertyToID("_PixelCount");
    public static readonly int COMPUTE_PIX_COUNT = Shader.PropertyToID("PixCount");
    public static readonly int COMPUTE_MAIN_TEX = Shader.PropertyToID("_MainTexCheck");

    public static readonly int ALPHA = Shader.PropertyToID("Alpha");
    public static readonly int FILL = Shader.PropertyToID("Fill");
    public static readonly int SPRITE_FILL = Shader.PropertyToID("_Fill");
    public static readonly int TEXTURE = Shader.PropertyToID("Text");
    public static readonly int IS_GRAY = Shader.PropertyToID("IsGray");
    public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
    public static readonly int COLOR = Shader.PropertyToID("_Color");
    public static readonly int COLOR_2 = Shader.PropertyToID("_Color2");
    public static readonly int FLOAT = Shader.PropertyToID("_Float");
    public static readonly int FLOAT_2 = Shader.PropertyToID("_Float2");
    public static readonly int ALBEDO = Shader.PropertyToID("_Albedo");
    public static readonly int MAIN_TEX_ST = Shader.PropertyToID("_MainTex_ST");

    public static readonly int GLB_COLOR = Shader.PropertyToID("GLB_COLOR");
    public static readonly int GLB_COLOR_2 = Shader.PropertyToID("GLB_COLOR_2");
    public static readonly int GLB_FLOAT = Shader.PropertyToID("GLB_FLOAT");

    public static readonly int GRAY_PER = Shader.PropertyToID("_Gray_Per");
    public static readonly int GLOBAL_RAINBOW_PER = Shader.PropertyToID("_Global_Rainbow_Per");

// Use for Time.frame
    public const int TIME_TICK_MAX = 30;
    public const int TIME_TICK_1 = 1;
    public const int TIME_TICK_2 = 2;
    public const int TIME_TICK_3 = 3;
    public const int TIME_TICK_4 = 4;
    public const int TIME_TICK_5 = 5;
    public const int TIME_TICK_6 = 6;
    public const int TIME_TICK_7 = 7;
    public const int TIME_TICK_8 = 8;
    public const int TIME_TICK_9 = 9;
    public const int TIME_TICK_10 = 10;
    public const int TIME_TICK_11 = 11;
    public const int TIME_TICK_12 = 12;
    public const int TIME_TICK_13 = 13;
    public const int TIME_TICK_14 = 14;
    public const int TIME_TICK_15 = 15;
    public const int TIME_TICK_16 = 16;
    public const int TIME_TICK_17 = 17;
    public const int TIME_TICK_18 = 18;
    public const int TIME_TICK_19 = 19;
    public const int TIME_TICK_20 = 20;
}