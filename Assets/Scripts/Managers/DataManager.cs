using UnityEngine;

public class DataManager : MonoBehaviour
{	
    #region PRIVATE FIELDS

    private PlayerInfo _playerInfo;

    #endregion

    #region PRIVATE FIELDS

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
        return _playerInfo;
    }

    public void SaveInfo()
    {
        Debug.LogFormat("<size=18><color=olive>{0}</color></size>", "save data");
    }

    #endregion

    #region PRIVATE METHODS

    private void LoadInfo()
    {
        Debug.LogFormat("<size=18><color=olive>{0}</color></size>", "load data");
        _playerInfo = new PlayerInfo();
    }

    #endregion
}
