#region
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
#endregion

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerData[] playerTypes;
    [SerializeField] Tilemap[] levels;
    [SerializeField] Transform spawnPoint;

    [SerializeField] [CanBeNull] TextMeshProUGUI nameText;

    public SceneData SceneData;
    Color currentForegroundColor;

    int currentPlayerTypeIndex;
    int currentTilemapIndex;
    Camera myCamera;
    PlayerMovement player;

    void Awake()
    {
        myCamera = FindObjectOfType<Camera>();
        player   = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    void Start()
    {
        SetSceneData(SceneData);
        //SwitchLevel(0);
        //SwitchPlayerType(0);
    }

    // public void SwitchPlayerType(int index)
    // {
    //     _player.Data = playerTypes[index];
    //     _currentPlayerTypeIndex = index;
    // }

    // public void SwitchLevel(int index)
    // {
    //     //Switch tilemap active and apply color.
    //     levels[_currentTilemapIndex].gameObject.SetActive(false);
    //     levels[index].gameObject.SetActive(true);
    //     levels[index].color = _currentForegroundColor;
    //     levels[_currentTilemapIndex] = levels[index];
    //
    //     _player.transform.position = spawnPoint.position;
    //
    //     _currentTilemapIndex = index;
    // }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Switch to next level. Uses "?" to indicate that if the expression in the brackets before is true
            //then 0 will be passed throuh else it will increse by 1.
            //SwitchLevel((_currentTilemapIndex == levels.Length - 1) ? 0 : _currentTilemapIndex + 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //Switch to next level. Uses "?" to indicate that if the expression in the brackets before is true
            //then 0 will be passed throuh else it will increse by 1.
            //SwitchPlayerType((_currentPlayerTypeIndex == playerTypes.Length - 1) ? 0 : _currentPlayerTypeIndex + 1);
        }
    }

    public void SetSceneData(SceneData data)
    {
        SceneData = data;

        //Update the camera and tilemap color according to the new data.
        myCamera.orthographicSize         = data.camSize;
        myCamera.backgroundColor          = data.backgroundColor;
        levels[currentTilemapIndex].color = data.foregroundColor;

        currentForegroundColor = data.foregroundColor;
    }
}
