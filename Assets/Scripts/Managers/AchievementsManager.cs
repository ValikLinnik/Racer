using System.Collections.Generic;
using Injection;
using UnityEngine;

public class AchievementsManager : InjectorBase<AchievementsManager>  
{	
    #region INJECTED FIELDS

    [Inject]
    private StateManager _stateManager;

    [Inject]
    private ReplayManager _replayManager;

    [Inject]
    private LevelManager _levelManager;

    #endregion

    #region PUBLIC PROPERTIES

    public float BestTime
    {
        get
        {
            if (!_levelManager) return 0;
            if (!_playerInfo) LoadPlayerInfo();
            return !_playerInfo.LevelInfo[_levelManager.LevelIndex] ? 0 : _playerInfo.LevelInfo[_levelManager.LevelIndex].BestTime;
        }
    }

    #endregion

    #region PRIVATE FIELDS

    private PlayerInfo _playerInfo;

    #endregion

    #region UNITY EVENTS

    private void Start()
    {
        LoadPlayerInfo();
    }

    #endregion

    #region PUBLIC METHODS

    public void SetLevelTime(float time)
    {
        if (!_levelManager) return;
        if (!_playerInfo) LoadPlayerInfo();

        if (!_playerInfo.LevelInfo[_levelManager.LevelIndex])
            _playerInfo.LevelInfo[_levelManager.LevelIndex] = new LevelInfo();
        if (_playerInfo.LevelInfo[_levelManager.LevelIndex].BestTime == 0 || _playerInfo.LevelInfo[_levelManager.LevelIndex].BestTime > time)
        {
            _playerInfo.LevelInfo[_levelManager.LevelIndex].BestTime = time;
            if (_replayManager)
            {
                _playerInfo.LevelInfo[_levelManager.LevelIndex].BestRace = new List<RaceSnapshot>(_replayManager.CurrentRace);
                _replayManager.BestRace = _playerInfo.LevelInfo[_levelManager.LevelIndex].BestRace;
            }
            DataManager.Instance.SaveInfo();
        }
    }

    #endregion

    #region PRIVATE METHODS

    private void LoadPlayerInfo()
    {
        _playerInfo = DataManager.Instance.GetInfo();

        if (!_replayManager || !_levelManager) return;
        if (!_playerInfo.LevelInfo[_levelManager.LevelIndex])
            _playerInfo.LevelInfo[_levelManager.LevelIndex] = new LevelInfo();
        _replayManager.BestRace = _playerInfo.LevelInfo[_levelManager.LevelIndex].BestRace;
    }

    #endregion
}
