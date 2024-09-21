using Melanchall.DryWetMidi.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public AudioSource audioSource;
    public GameObject beginLine;
    public GameObject endLine;
    public SpriteRenderer backgroundDecorMat;
    public float decorEffectSpeed;
    public List<GameObject> shapeObjs;
    public float shapeMoveSpeedMax;
    public float shapeMoveSpeedMin;
    public float rotateSpeedMin;
    public float rotateSpeedMax;
    class Shape
    {
        public GameObject shapeObj;
        public Vector3 destination;
        public Vector3 moveDirection;
        public float moveSpeed;
        public float rotateSpeed;
        public Shape(GameObject shapeObj)
        {
            this.shapeObj = shapeObj;
        }
    }

    List<Shape> shapes = new List<Shape>();
    private bool running = false;
    public TilesManager tilesManager;
    public ScoreManager scoreManager;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        tilesManager = new TilesManager();
        scoreManager = new ScoreManager();

        foreach (var shapeObj in shapeObjs)
        {
            Shape shape = new Shape(shapeObj);
            ResetShape(shape);
            shapes.Add(shape);
        }

        StartCoroutine(RunDecorEffect());
    }


    // Update is called once per frame
    void Update()
    {
        ProcessBackgroundShape();
        if (!running) return;
        tilesManager.Update();
    }

    public void StartGame()
    {
        MidiFile midiFile = MidiFile.Read(Application.streamingAssetsPath + "/Monody/Monody.mid");
        StartCoroutine(tilesManager.LoadSong(midiFile));
    }

    public IEnumerator CountDown()
    {
        float time = 3;
        running = true;
        while (time > 0)
        {
            time -= Time.deltaTime;
            UIManager.Instance.SetCountdownIngame(time);
            yield return null;
        }
        PlaySong();
    }

    void ProcessBackgroundShape()
    {
        foreach(var shape in  shapes)
        {
            if(shape.shapeObj.transform.position.x >= shape.destination.x)
            {
                ResetShape(shape);
            }
            else
            {
                shape.shapeObj.transform.position +=  shape.moveSpeed * Time.deltaTime * shape.moveDirection;
                if (shape.shapeObj.tag == "Square") shape.shapeObj.transform.Rotate(0, 0, shape.rotateSpeed * Time.deltaTime);
            }
        }
    }

    void ResetShape(Shape shape)
    {
        shape.moveSpeed = Random.Range(shapeMoveSpeedMin, shapeMoveSpeedMax);
        shape.rotateSpeed = Random.Range(rotateSpeedMin , rotateSpeedMax) * (Random.Range(0, 2) == 0 ? 1 : -1);
        shape.shapeObj.transform.localPosition = new Vector3(Random.Range(-4f, -3f), Random.Range(-5f, 5f), -1);
        shape.destination = new Vector3(4, Random.Range(-5, 5), -1);
        shape.moveDirection = (shape.destination - shape.shapeObj.transform.position).normalized;
    }

    IEnumerator RunDecorEffect()
    {
        int direction = 1;
        while(true)
        {
            Color color = backgroundDecorMat.color;
            color.a += direction * decorEffectSpeed / 255 * Time.deltaTime;
            color.a = Mathf.Clamp01(color.a);
            backgroundDecorMat.color = color;
            if (backgroundDecorMat.color.a == 1 || backgroundDecorMat.color.a <= 0.5 ) direction = -direction;
            yield return null;
        }
    }

    public void PlaySong()
    {
        SongConfig songConfig = Resources.Load("Config/Monody") as SongConfig;
        audioSource.clip = songConfig.audio;
        audioSource.Play();
        StartCoroutine(GameEndEvent(audioSource.clip.length));
    }

    private IEnumerator GameEndEvent(float time)
    {
        yield return new WaitForSeconds(time);
        UIManager.Instance.ShowGameEnd();
        running = false;
    }
}
