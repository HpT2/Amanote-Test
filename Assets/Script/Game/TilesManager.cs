using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesManager
{
    public float errorForPerfect = 0.05f;
    public float errorForGreat = 0.1f;

    public float perfectPoint = 3f;
    public float greatPoint = 2f;
    public float okayPoint = 1f;

    private float tilesSpawnY = 10f;
    private List<float> tilesSpawnX = new List<float>();
    private Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();
    private float renderWidth = Camera.main.orthographicSize;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    public static GameObject tilePrefab = Resources.Load("Prefabs/Tile") as GameObject;
    private float timeSinceStart = 0;
    private Dictionary<float, List<Vector3>> positionWithTime = new Dictionary<float, List<Vector3>>();
    class Tile
    {
        public GameObject tileObj;
        private float timeStart;
        private float timeDelay = 3f;
        public float timeTap;
        private float speed = 4.7f;
        public bool canTap = false;
        public Tile(Note note, TempoMap tempoMap)
        {
            MetricTimeSpan time = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
            timeStart = time.Minutes * 60 + time.Seconds;
            timeTap = timeStart + timeDelay;
            tileObj = GameObject.Instantiate(TilesManager.tilePrefab);
            tileObj.tag = "Tile";
        }

        public void Move(float timeSinceStart)
        {
            if (timeSinceStart - timeStart >= 0f) tileObj.transform.position += Vector3.down * speed * Time.deltaTime;
        }

    }

    public IEnumerator LoadSong(MidiFile midiFile)
    {
        UIManager.Instance.ShowLoading();
        yield return null;
        timeSinceStart = 0;
        var notes = midiFile.GetNotes();
        var notesArr = new Note[notes.Count];
        notes.CopyTo(notesArr, 0);

        GameObject linePrefabs = Resources.Load("Prefabs/Line") as GameObject;
        for(int i = 1; i < 4; i++)
        {
            GameObject obj = GameObject.Instantiate(linePrefabs);
            LineRenderer line =  obj.GetComponent<LineRenderer>();
            line.startWidth = 0.025f;
            line.endWidth = 0.025f;
            line.startColor = Color.black;
            line.endColor = Color.black;
            float linePosX = -renderWidth / 2 + renderWidth * i / 4;
            Vector3[] vertices = new Vector3[2];
            vertices[0] = new Vector3(linePosX, -5, -1);
            vertices[1] = new Vector3(linePosX, 5, -1);
            line.SetPositions(vertices);
            lineRenderers.Add(line);
        }


        for(int i = 0; i < 4; i++)
        {
            tilesSpawnX.Add(-renderWidth / 2 + renderWidth / 8 + renderWidth * i / 4);
        }

        var tempoMap = midiFile.GetTempoMap();
        float tileWidth = renderWidth / 4.5f;
        foreach (Note note in notesArr)
        {
            Tile tile = new Tile(note, tempoMap);
            float scale = tileWidth / tile.tileObj.GetComponent<SpriteRenderer>().bounds.size.x;
            tile.tileObj.transform.localScale = new Vector3(scale, scale, 1);

            tile.tileObj.transform.position = GetPositionForTile(tile);
            tiles.Add(tile.tileObj.GetInstanceID(), tile);
        }

        GameManager.Instance.scoreManager.SetMaxScore(tiles.Count * perfectPoint);
        GameManager.Instance.scoreManager.ResetScore();
        GameManager.Instance.beginLine.SetActive(true);
        GameManager.Instance.endLine.SetActive(true);

        yield return new WaitForSeconds(1f);

        UIManager.Instance.ShowIngame();
        GameManager.Instance.StartCoroutine(GameManager.Instance.CountDown());
    }

    private Vector3 GetPositionForTile(Tile tile)
    {
        if (!positionWithTime.ContainsKey(tile.timeTap))
        {
            positionWithTime.Add(tile.timeTap, new List<Vector3>());
        }

        Vector3 position = new Vector3(tilesSpawnX[Random.Range(0, tilesSpawnX.Count)], tilesSpawnY, -2);
        foreach (var pos in positionWithTime[tile.timeTap])
        {
            if (pos == position)
            {
                return GetPositionForTile(tile);
            }
        }
        return position;
    }
    public void Update()
    {
        timeSinceStart += Time.deltaTime;
        foreach(var tile in tiles)
        {
            tile.Value.Move(timeSinceStart);
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(touchPosition.x, touchPosition.y), Camera.main.transform.forward);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "Tile")
                {
                    Tile tile = tiles[hit.collider.gameObject.GetInstanceID()];
                    HandleTileTap(tile);

                }
            }
        }
    }

    private void HandleTileTap(Tile tile)
    {
        if (!tile.canTap) return;
        float tileTime = tile.timeTap;
        float timeError = Mathf.Abs(timeSinceStart - tileTime);
        Debug.Log(timeError);
        GameManager gameManager = GameManager.Instance;
        UIManager uIManager = UIManager.Instance;
        if (timeError <= errorForPerfect)
        {
            gameManager.scoreManager.AddScore(perfectPoint);
            uIManager.UpdateScoringType("Perfect", false);
        }
        else if (timeError <= errorForGreat )
        {
            gameManager.scoreManager.AddScore(greatPoint);
            uIManager.UpdateScoringType("Great", false);
        }
        else
        {
            gameManager.scoreManager.AddScore(okayPoint);
            uIManager.UpdateScoringType("Okay", false);
        }
        GameManager.Instance.StartCoroutine(TileTapEffect(tile));
        tile.canTap = false;
        
    }

    private IEnumerator TileTapEffect(Tile tile)
    {
        SpriteRenderer tileSprite = tile.tileObj.GetComponent<SpriteRenderer>();
        while(true && tileSprite)
        {
            Color color = tileSprite.color;
            color.a -= 2f * Time.deltaTime;
            tileSprite.color = color;
            if (color.a <= 0.1f) break;
            yield return null;
        }
    }

    public void EnableTile(int id)
    {
        tiles[id].canTap = true;
    }

    public void DestroyTile(int id)
    {
        if (tiles[id].canTap) UIManager.Instance.UpdateScoringType("Miss", true);
        GameObject.Destroy(tiles[id].tileObj);
        tiles.Remove(id);
    }

    public void ClearObj()
    {
        for(int i = lineRenderers.Count - 1; i >= 0; i--)
        {
            GameObject.Destroy(lineRenderers[i].gameObject);
            lineRenderers.RemoveAt(i);
        }

        positionWithTime.Clear();
        tilesSpawnX.Clear();

        GameManager.Instance.beginLine.SetActive(false);
        GameManager.Instance.endLine.SetActive(false);
    }
}
