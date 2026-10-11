using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlockBlast
{
    public class GameBootstrap : MonoBehaviour
    {
        HomeScreen home;
        GameScreen game;
        GameOverScreen gameOver;
        SettingsScreen settings;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            if (FindObjectOfType<GameBootstrap>() != null) return;
            new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
        }

        void Awake()
        {
            EnsureCamera();
            EnsureEventSystem();

            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);
            canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            AudioManager.Create(transform);
            AudioManager.Instance.PlayMusic();

            home = HomeScreen.Create(canvasGo.transform, StartGame, ShowSettings);
            game = GameScreen.Create(canvasGo.transform, ShowHome, ShowGameOver);
            gameOver = GameOverScreen.Create(canvasGo.transform, StartGame, ShowHome);
            settings = SettingsScreen.Create(canvasGo.transform);

            ShowHome();
        }

        void ShowHome()
        {
            home.Refresh();
            home.gameObject.SetActive(true);
            game.gameObject.SetActive(false);
            gameOver.gameObject.SetActive(false);
            settings.gameObject.SetActive(false);
        }

        void ShowSettings()
        {
            settings.Show();
        }

        void StartGame()
        {
            home.gameObject.SetActive(false);
            gameOver.gameObject.SetActive(false);
            game.gameObject.SetActive(true);
            game.StartNewGame();
        }

        void ShowGameOver(int score, int best, bool isNewBest)
        {
            gameOver.Show(score, best, isNewBest);
        }

        static void EnsureCamera()
        {
            if (FindObjectOfType<Camera>() != null) return;
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            var cam = go.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.04f, 0.1f, 0.3f);
            go.AddComponent<AudioListener>();
        }

        static void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null) return;
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }
}
