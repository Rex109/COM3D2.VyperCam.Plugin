using System;
using UnityInjector;
using UnityEngine;
using UnityInjector.Attributes;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExIni;
using System.Linq;
using System.IO;
using System.Collections;

namespace COM3D2.VyperCam.Plugin
{
    [PluginFilter("COM3D2x64"), PluginFilter("COM3D2VRx64"), PluginFilter("COM3D2OHx64"), PluginFilter("COM3D2OHVRx64"), PluginName("VyperCam"), PluginVersion("1.2.0.0")]
    public class VyperCam : PluginBase
    {
        private WebCamDevice[] devices = WebCamTexture.devices;
        private static WebCamTexture webcamTex;
        private static Texture2D ipwebcamTex;

        private bool objectcreated = false;

        private GameObject panel;

        private KeyCode togglekey = KeyCode.F6;
        private int WebcamMode = 0;
        private String webcamURL = "";

        private int webcamid = 0;
        private float camsizex = 500f;
        private float camsizey = 500f;
        private float camoffsetx = 0f;
        private float camoffsety = 0f;

        private bool vr;

        private Stream stream;
        private RawImage ImageRect;

        public void Start()
        {
            SceneManager.sceneLoaded += OnLevelFinishedLoading;

            vr = Environment.GetCommandLineArgs().Any(s => s.ToLower().Contains("/vr"));

            if (!vr)
            {
                Console.WriteLine("[VyperCam] Game is not in vr mode, disabling plugin...");
            }
            else
            {
                LoadConfig();

                if (WebcamMode == 0)
                {
                    if (devices.Length > 0)
                    {
                        webcamTex = new WebCamTexture(WebCamTexture.devices[webcamid].name);
                        Console.WriteLine("[VyperCam] Webcam loaded: " + webcamTex.deviceName);
                    }
                    else
                        Console.WriteLine("[VyperCam] Can't find any camera");
                }
            }
        }

        private void LoadConfig()
        {
            try
            {
                IniKey key = Preferences["Config"]["ToggleKey"];
                IniKey webmode = Preferences["Config"]["WebcamMode"];
                IniKey weburl = Preferences["Config"]["WebcamURL"];
                IniKey id = Preferences["Config"]["WebcamID"];
                IniKey sizex = Preferences["Config"]["WebcamSizeX"];
                IniKey sizey = Preferences["Config"]["WebcamSizeY"];
                IniKey offsetx = Preferences["Config"]["WebcamOffsetX"];
                IniKey offsety = Preferences["Config"]["WebcamOffsetY"];

                camsizex = float.Parse(sizex.Value);
                camsizey = float.Parse(sizey.Value);

                camoffsetx = float.Parse(offsetx.Value);
                camoffsety = float.Parse(offsety.Value);

                WebcamMode = int.Parse(webmode.Value);
                webcamURL = weburl.Value;

                togglekey = (KeyCode)Enum.Parse(typeof(KeyCode), key.Value, true);
                webcamid = int.Parse(id.Value);

                Console.WriteLine("[VyperCam] Configuration loaded correctly!");
            }
            catch (Exception e)
            {
                Console.WriteLine("[VyperCam] There was an error during the configuration loading, default configuration loaded. Error details: " +  e.ToString());
            }
        }

        void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            if (vr)
            {
                Console.WriteLine("[VyperCam] Updating camera UI...");

                if (!objectcreated)
                {
                    var uiRoot = GameObject.Find("SystemUI Root");

                    panel = new GameObject();
                    panel.name = "VyperCam UI";
                    panel.transform.parent = uiRoot.transform;
                    panel.transform.localPosition = Vector3.zero;
                    panel.transform.localRotation = Quaternion.identity;
                    panel.transform.localScale = Vector3.one;
                    panel.layer = uiRoot.layer;

                    var canvas = panel.AddComponent<Canvas>();
                    canvas.overrideSorting = true;
                    canvas.renderMode = RenderMode.WorldSpace;

                    var rect = panel.GetComponent<RectTransform>();
                    rect.sizeDelta = new Vector2(camsizex, camsizey);
                    rect.pivot = new Vector2(1, 0);
                    rect.localPosition = new Vector2(960 + camoffsetx, -540 + camoffsety);

                    ImageRect = panel.AddComponent<RawImage>();
                    ImageRect.transform.SetParent(panel.transform, false);
                    ImageRect.enabled = false;
                    objectcreated = true;
                }

                if (WebcamMode == 0)
                {
                    ImageRect.material.mainTexture = webcamTex;
                    webcamTex.Play();
                }
                else if (WebcamMode == 1)
                {
                    StopCoroutine(Fetch()); //Just to prevent overlapping coroutines
                    ipwebcamTex = new Texture2D(2, 2);
                    ImageRect.material.mainTexture = ipwebcamTex;
                    StartCoroutine(Fetch());
                }

                Console.WriteLine("[VyperCam] Done!");
            }
        }

        public IEnumerator Fetch()
        {
            while (true)
            {
                WWWForm form = new WWWForm();
                WWW www = new WWW(webcamURL);
                yield return www;

                if (!string.IsNullOrEmpty(www.error))
                    throw new UnityException(www.error);

                www.LoadImageIntoTexture(ipwebcamTex);
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(togglekey) && vr)
            {
                Console.WriteLine("[VyperCam] Hide/Show Camera");
                panel.GetComponent<RawImage>().enabled = !panel.GetComponent<RawImage>().enabled;
            }
        }
    }
}