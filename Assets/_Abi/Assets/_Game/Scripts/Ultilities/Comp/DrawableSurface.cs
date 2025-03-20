using UnityEngine;
using UnityEngine.EventSystems;

public class DrawableSurface : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public Camera camera;
    [SerializeField] private Texture2D brushTex;
    private RenderTexture dirtMask;
    [SerializeField] private Texture2D dirtTex;
    [SerializeField] private Mesh meshToDraw;
    public Renderer rend;
    [SerializeField] private bool isAutoBake;

    private float[,] brushBaked;
    private float[,] dirtBaked;
    private int offsetX;
    private int offsetY;
    private float dirtMaskWidth;
    private float dirtMaskHeight;
    private float brushWidth;
    private float brudhHeight;
    private int boundX;
    private int boundY;

    private float timePoint;
    [SerializeField] private bool isCheck;
    [SerializeField] private bool isDrawOnDrag = true;
    [SerializeField] private float checkDelay = .62f;
    // private Color cur = Color.red;
    // private Color[] curPixels;

    [SerializeField] private Material blitMat;
    [SerializeField] private float maxBrushDistOnDragPercent = .25f;
    private float maxDragDist;
    private Vector2 lastPos;
    private bool isSetLAstPos;
    private Vector2 curCoord;
    private Vector2 lastCoord;
    private static readonly int MainTex = Shader.PropertyToID(MAIN_TEX);
    private static readonly int BrushWidth = Shader.PropertyToID(BLIT_BRUSH_WIDTH);
    private static readonly int BrushHeight = Shader.PropertyToID(BLIT_BRUSH_HEIGHT);
    private static readonly int DirtWidth = Shader.PropertyToID(BLIT_DIRT_WIDTH);
    private static readonly int DirtHeight = Shader.PropertyToID(BLIT_DIRT_HEIGHT);
    private static readonly int Pos = Shader.PropertyToID(BLIT_POS);

    private const string MAIN_TEX = "_MainTex";
    private const string BLIT_DIRT_WIDTH = "_DirtWidth";
    private const string BLIT_DIRT_HEIGHT = "_DirtHeight";
    private const string BLIT_BRUSH_WIDTH = "_BrushWidth";
    private const string BLIT_BRUSH_HEIGHT = "_BrushHeight";
    private const string BLIT_POS = "_Pos";

    private static readonly int GLOBAL_PIXEL_COUNT = Shader.PropertyToID("_PixelCount");
    
    private void Start()
    {
        if(isAutoBake) Bake();
    }

    private void OnDestroy()
    {
        dirtMask.Release();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isSetLAstPos = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDrawOnDrag) DrawToTEx();
        if (!isCheck) return;
        timePoint = Time.time + checkDelay;
    }

    private void Update()
    {
        if (!isCheck) return;
        if (!(timePoint < Time.time)) return;
        timePoint = float.MaxValue;
        if (CheckDone())
        {
            isCheck = false;
            OnCompleteDraw();
        }
        else
        {
            OnDrawCheckFail();
        }
    }

    protected virtual void OnCompleteDraw() { }
    protected  virtual void OnDrawCheckFail() { }

    public void StartCheck()
    {
        isCheck = true;
    }

    public void SetUpBake(Texture2D tex)
    {
        dirtTex = tex;
        Bake();
    }
    
    [ContextMenu("Bake")]
    private void Bake()
    {
        timePoint = float.MaxValue;
        dirtMask = new RenderTexture(dirtTex.width, dirtTex.height, 0);
        Graphics.Blit(dirtTex, dirtMask);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        rend.material.SetTexture(MainTex, dirtMask);//temp set texture for debug mask ?////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        offsetX = Mathf.RoundToInt(brushTex.width / 2);
        offsetY = Mathf.RoundToInt(brushTex.height / 2);
        dirtMaskWidth = dirtMask.width;
        dirtMaskHeight = dirtMask.height;
        brushWidth = brushTex.width;
        brudhHeight = brushTex.height;
        boundX = Mathf.RoundToInt(dirtMaskWidth) - 1;
        boundY = Mathf.RoundToInt(dirtMaskHeight) - 1;
        var localScale = transform.localScale;
        blitMat.SetFloat(BrushWidth, brushWidth / localScale.x);
        blitMat.SetFloat(BrushHeight, brudhHeight / localScale.y);
        blitMat.SetFloat(DirtWidth, dirtMaskWidth);
        blitMat.SetFloat(DirtHeight, dirtMaskHeight);
        blitMat.SetTexture(MainTex, brushTex);
        maxDragDist = brudhHeight * maxBrushDistOnDragPercent;

        if (!isCheck) return;
        brushBaked = new float[Mathf.RoundToInt(brushTex.width), Mathf.RoundToInt(brushTex.height)];
        dirtBaked = new float[Mathf.RoundToInt(dirtMask.width), Mathf.RoundToInt(dirtMask.height)];
        for (var i = 0; i < dirtMaskWidth; i++)
        {
            for (var j = 0; j < dirtMaskHeight; j++)
            {
                dirtBaked[i, j] = dirtTex.GetPixel(i, j).r;
            }
        }

        for (var i = 0; i < brushWidth; i++)
        {
            for (var j = 0; j < brudhHeight; j++)
            {
                brushBaked[i, j] = brushTex.GetPixel(i, j).r;
            }
        }
    }

    private void DrawToTEx()
    {
        // Debug.Log("Drag ");
        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out var raycastHit, Mathf.Infinity, Variables.LAYER_MASK_DRAWABLE))
        {
            ExecuteDraw(raycastHit.textureCoord);
        }
        else if(!isSetLAstPos)
        {
            if (AABBContainsSegment(curCoord.x, curCoord.y, lastCoord.x, lastCoord.y, 0, 0, 1, 1, out var coord))
            {
                ExecuteDraw(coord);
            }
            
            isSetLAstPos = true;
        }
    }
    
     private static bool AABBContainsSegment (float x1, float y1, float x2, float y2, float minX, float minY, float maxX, float maxY, out Vector2 coord) {  
        coord = Vector2.zero;
        // Completely outside.
        if ((x1 <= minX && x2 <= minX) || (y1 <= minY && y2 <= minY) || (x1 >= maxX && x2 >= maxX) || (y1 >= maxY && y2 >= maxY))
            return false;

        var m = (y2 - y1) / (x2 - x1);

        var isLeft = x1 < x2;
        var isTop = y1 > y2;

        //check left
        var y = m * (minX - x1) + y1;
        if (isLeft&& (y > minY && y < maxY))
        {
            coord = new Vector2(minX, y);
            return true;
        }

        //check right
        y = m * (maxX - x1) + y1;
        if (!isLeft&&(y > minY && y < maxY))
        {
            coord = new Vector2(maxX, y);
            return true;
        }

        //check bottom
        var x = (minY - y1) / m + x1;
        if (!isTop && (x > minX && x < maxX))
        {
            coord = new Vector2(x,minY);
            return true;
        }

        //check top
        x = (maxY - y1) / m + x1;
        if (isTop&&(x > minX && x < maxX))
        {
            coord = new Vector2(x, maxY);
            return true;
        }

        return false;
    }


    private void ExecuteDraw(Vector2 textureCoord)
    {
        var poss = new Vector2(textureCoord.x * dirtMaskWidth, textureCoord.y * dirtMaskHeight);
            
        if (isSetLAstPos) 
        { 
            lastPos = poss;
            isSetLAstPos = false;
            curCoord = lastCoord = textureCoord;
        }

        lastCoord = curCoord;
        curCoord = textureCoord;

        var loopNum = Mathf.CeilToInt(Vector2.Distance(poss, lastPos) / maxDragDist);

        var cur = RenderTexture.active;
        RenderTexture.active = dirtMask;
        for (var i = 1; i < loopNum; i++)
        {
            blitMat.SetVector(Pos, Vector2.Lerp(lastPos, poss, (float)i / loopNum));
            blitMat.SetPass(0);
            Graphics.DrawMeshNow(meshToDraw, Matrix4x4.identity);
        }
        ////SEtpass work one time with DrawMeshNow
        //////////////////////////////////////////////// Set material property before set pass or else you fucked ////////////////////////////////////////////////
        blitMat.SetVector(Pos, poss);
        blitMat.SetPass(0);
        Graphics.DrawMeshNow(meshToDraw, Matrix4x4.identity);
        lastPos = poss;
        RenderTexture.active = cur;

        if (!isCheck) return;
        
        var pos = new Vector2Int(Mathf.RoundToInt(poss.x), Mathf.RoundToInt(poss.y));
        for (var i = 0; i < brushWidth; i++)
        {
            for (var j = 0; j < brudhHeight; j++)
            {
                var x = Mathf.Clamp(i - offsetX + pos.x, 0, boundX);
                var y = Mathf.Clamp(j - offsetY + pos.y, 0, boundY);
                var tmp = brushBaked[i, j] * dirtBaked[x, y];
                dirtBaked[x, y] = tmp;
            }
        }
        
    }

    private bool CheckDone()
    {
        float totalDirt = 0;

        for (var i = 0; i < dirtMaskWidth; i++)
        {
            for (var j = 0; j < dirtMaskHeight; j++)
            {
                totalDirt += dirtBaked[i, j];
            }
        }

        return totalDirt < .000062f * dirtMaskWidth * dirtMaskHeight;
    }

}
