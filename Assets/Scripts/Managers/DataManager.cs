using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataManager : MonoBehaviour
{	
    #region PRIVATE FIELDS

    private PlayerInfo _playerInfo;
    private string _path = "/Data/info";

    #endregion

    #region STATIC FIELDS

    private static DataManager _instance;

    public static DataManager Instance
    {
        get
        {
            return _instance;
        }
    }

    #endregion

    #region UNITY EVENTS

    private void Awake()
    {
        if (!_instance) _instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
        LoadInfo();
    }

    #endregion

    #region PUBLIC METHODS

    public PlayerInfo GetInfo()
    {
        if (!_playerInfo) LoadInfo();
        if(_playerInfo.LevelInfo.IsNullOrEmpty()) _playerInfo.LevelInfo = new LevelInfo[GameConfig.LevelCount];
        return _playerInfo;
    }

    public void SaveInfo()
    {
        var formatter = new BinaryFormatter();

        using(var stream = new FileStream(Application.dataPath + _path, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            formatter.Serialize(stream, _playerInfo);
        }
    }

    #endregion

    #region PRIVATE METHODS

    private void LoadInfo()
    {
        if(!File.Exists(Application.dataPath + _path))
        {
            _playerInfo = new PlayerInfo();
            return;
        }

        var formatter = new BinaryFormatter();

        using(var stream = new FileStream(Application.dataPath + _path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            _playerInfo = formatter.Deserialize(stream) as PlayerInfo;
        }
    }

    #endregion
}
