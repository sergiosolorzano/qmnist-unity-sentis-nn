using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Sentis;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using TMPro;

public class writeToCanvas : MonoBehaviour
{
    private Camera mainCamera;
    Coroutine drawing;
    private Vector2 lastInstancePosition;
    private List<LineRenderer> lineRendererList = new List<LineRenderer>();

    private int captureWidth = 1024;
    private int captureHeight = 1024;

    int model_WH=28;

    Texture2D snap;
    public ClassifyHandwrittenDigit m_ClassifyHandwrittenDigit;
    [SerializeField]public Image testTex;

    public GameObject ShowButton;
    public GameObject TextOnScreen;
    private List<GameObject> PlayersOnScreenList = new List<GameObject>();

    public TextMeshProUGUI numOutputText;


    void Start()
    {
        mainCamera = Camera.main;
        captureWidth= Mathf.RoundToInt(mainCamera.pixelWidth*.7f);
        captureHeight = mainCamera.pixelHeight;
        mainCamera.backgroundColor = Color.black;
        mainCamera.clearFlags = CameraClearFlags.Skybox;
        lastInstancePosition = new Vector2 (0, 0);
    }

    void Update()
    {
        // Check if the left mouse button is clicked down
        if (Input.GetMouseButtonDown(0))
        {
            StartLine();
        }
        if (Input.GetMouseButtonUp(0))
        {
            FinishLine();
        }

        void StartLine()
        {
            if(drawing!=null)
                StopCoroutine(drawing);

            drawing = StartCoroutine(DrawLine());
        }
    }

    void FinishLine()
    {
        StopCoroutine(drawing);
    }

    public void ClearClutter()
    {
        numOutputText.text = "";

        lastInstancePosition = new Vector2(0, 0);

        foreach (var player in PlayersOnScreenList)
        {
            Destroy(player);
        }
        PlayersOnScreenList.Clear();

        foreach (var line in lineRendererList)
        {
            Destroy(line.gameObject);
        }
        lineRendererList.Clear();
    }

    IEnumerator DrawLine() {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Pointer is over a UI element, do nothing
            Debug.Log("On top of UI element");
            yield return null;
        }
        else
        {
            GameObject newGO = Instantiate(Resources.Load("LineRend") as GameObject, new Vector3(0, 0, 0), Quaternion.identity);
            LineRenderer line = newGO.GetComponent<LineRenderer>();
            lineRendererList.Add(line);

            line.positionCount = 0;

            while (true)
            {
                Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                line.positionCount++;
                line.SetPosition(line.positionCount - 1, pos);
                yield return null;
            }
        }
    }

    public void BlitCam()
    {
        StartCoroutine(GetModelOutput());
    }

    public IEnumerator GetModelOutput()
    {
        //clear and disable clut on screen to capture what's written
        foreach(var player in PlayersOnScreenList)
        {
            Destroy(player);
        }
        PlayersOnScreenList.Clear();
        TextOnScreen.SetActive(false);
        ShowButton.SetActive(false);

        //capture what's written
        CaptureCameraView();
        int modelNum = m_ClassifyHandwrittenDigit.ClassifyNumber(snap);
        numOutputText.text = modelNum.ToString();


        //instantiate according to num
        InstantiatePlayers(modelNum);

        //re-enable clutter
        TextOnScreen.SetActive(true);
        ShowButton.SetActive(true);
        lastInstancePosition = new Vector2(0, 0);

        yield return null;
    }

    private void InstantiatePlayers(int num)
    {
        Debug.Log("***Got" + num);
        GameObject prefab = (GameObject)Resources.Load("PlayerSprite");

        for(int i=0; i<num; i++)
        {
            GameObject newGO = Instantiate(prefab, GameObject.Find("Canvas").transform);

            //get sprite
            // Set the sprite of the Image component
            var thisSprite = newGO.GetComponent<Sprite>();

            // Set the RectTransform properties for the top-left corner
            RectTransform rectTransform = newGO.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = lastInstancePosition;
            lastInstancePosition.x += rectTransform.rect.width;

            PlayersOnScreenList.Add(newGO);
        }
    }

    public void CaptureCameraView()
    {
        RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        mainCamera.targetTexture = renderTexture;
        mainCamera.Render();

        RenderTexture.active = renderTexture;
        snap = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBA32, false);
        snap.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        snap.Apply();

        mainCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        snap = ResizeTexture(snap, model_WH, model_WH);

        Rect rect = new Rect(0, 0, model_WH, model_WH);
        Sprite sprite = Sprite.Create(snap, rect, Vector2.zero);
        testTex.sprite = sprite;
    }

    Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = new RenderTexture(newWidth, newHeight, 24);
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D result = new Texture2D(newWidth, newHeight);
        result.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        result.Apply();
        RenderTexture.active = null;
        rt.Release();
        return result;
    }
    }
