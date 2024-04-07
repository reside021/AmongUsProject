using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabletUI : MonoBehaviour
{
    public GameObject BlockSkippedButton;
    public GameObject ChatBlock;
    public Button SkipVoteBtn;
    public Button OpenChatBtn;
    public Button OpenKickTableBtn;
    public Button VoteCancelBtn;
    public Button VoteConfirmBtn;
    public TextMeshProUGUI TimerText;
    public Transform Content;
    public Transform UsersBlock;

    public static Action<int> OnKickPlayer;

    private int _playerCount = 0;
    private NonAllocDictionary<int, PhotonView>.ValueIterator _players = new NonAllocDictionary<int, PhotonView>.ValueIterator();
    private Transform[] _users;
    private bool _isMineDead;
    private bool _isVoted;
    private bool _isTimeForVote;

    private int _votingBeginsIn = 15;
    private int _votingEndsIn = 35;
    private int _proceedingIn = 5;

    private int _deadPlayerCount = 0;
    private int _votedPlayerCount = 0;

    private Dictionary<int, List<int>> _votingSheet = new();

    private int _chooseForKick = -1;

    private const int SKIP = 1111;




    private void OnEnable()
    {
        LevelManager.OnTabletOpened += OnTabletOpened;
        LevelManager.OnPlayerVoted += OnPlayerVoted;
    }





    void Start()
    {
        AddListenerForButton();

        _playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        _users = SetUsers();
        CheckUsersCount();
        SaveListPlayer();

        _votingSheet[SKIP] = new List<int>();


    }

    #region StartMethods
    private void AddListenerForButton()
    {
        OpenChatBtn.onClick.AddListener(() =>
        {
            ChatBlock.SetActive(true);
        });

        SkipVoteBtn.onClick.AddListener(() =>
        {
            if (_isMineDead) return;

            if (!_isTimeForVote) return;

            BlockSkippedButton.SetActive(true);
            _chooseForKick = SKIP;
        });

        OpenKickTableBtn.onClick.AddListener(() =>
        {
            ChatBlock.SetActive(false);
        });

        VoteCancelBtn.onClick.AddListener(() =>
        {
            VoteCanceled(BlockSkippedButton);
        });

        VoteConfirmBtn.onClick.AddListener(() =>
        {
            VoteConfirmed(BlockSkippedButton);
        });
    }

    private Transform[] SetUsers()
    {
        var childCound = UsersBlock.transform.childCount;
        Transform[] usersBlocks = new Transform[childCound];
        for (var i = 0; i < childCound; i++)
        {
            usersBlocks[i] = UsersBlock.GetChild(i);
        }
        return usersBlocks;
    }

    private void CheckUsersCount()
    {
        for (var i = 0; i < 10; i++)
        {
            if (i + 1 > _playerCount)
            {
                _users[i].gameObject.SetActive(false);
            }
        }
    }

    private void SaveListPlayer()
    {
        _players = PhotonNetwork.PhotonViewCollection;
    }

    #endregion



    #region Button

    private void VoteConfirmed(GameObject blockButton)
    {
        var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        var sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(10, _chooseForKick, options, sendOptions);

        _isVoted = true;

        blockButton.SetActive(false);
    }

    public void VoteCanceled(GameObject blockButton)
    {
        blockButton.SetActive(false);
    }

    #endregion



    

    

    #region CallbackMethods

    private void CheckStatusMineDead()
    {
        foreach (var player in PhotonNetwork.PhotonViewCollection)
        {
            if (!player.CompareTag("Player")) continue;

            if (!player.IsMine) continue;

            _isMineDead = player.gameObject.GetComponent<PlayerController>().IsDead;
        }
    }
    private void CreateTabletContent(int finderID)
    {
        var index = -1;

        foreach (var player in _players)
        {
            if (!player.CompareTag("Player")) continue;

            index++;

            var nameUser = _users[index].Find("Content/NameUser");
            nameUser.GetComponent<TextMeshProUGUI>().text = player.Controller.NickName;

            _votingSheet[player.ControllerActorNr] = new List<int>();

            if (player.ViewID == finderID)
            {
                _users[index].Find("Content/SignReported").gameObject.SetActive(true);
            }

            var playerDead = player.gameObject.GetComponent<PlayerController>().IsDead;

            if (playerDead)
            {
                _users[index].Find("DeathPlayer").gameObject.SetActive(true);
                _deadPlayerCount++;
            }

            if (_isMineDead) continue;

            if (player.IsMine || playerDead) continue;

            var blockButton = _users[index].Find("Content/BlockButton");

            _users[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!_isTimeForVote) return;

                if (_isVoted) return;

                blockButton.gameObject.SetActive(true);
                _chooseForKick = player.ControllerActorNr;
            });

            var voteConfirm = blockButton.Find("VoteConfirm").gameObject.GetComponent<Button>();
            var voteCancel = blockButton.Find("VoteCancel").gameObject.GetComponent<Button>();

            voteConfirm.onClick.AddListener(() => VoteConfirmed(blockButton.gameObject));
            voteCancel.onClick.AddListener(() => VoteCanceled(blockButton.gameObject));
        }
    }

    private void UpdateVotedPlayer(int actorSender)
    {
        var index = -1;

        foreach (var player in _players)
        {
            if (!player.CompareTag("Player")) continue;

            index++;

            if (player.ControllerActorNr == actorSender)
            {
                _users[index].Find("Content/SignVoted").gameObject.SetActive(true);
            }

        }
    }
    #endregion

    #region Coroutine
    private IEnumerator TimerVotingBegins()
    {
        var currentTime = _votingBeginsIn;

        while (currentTime > -1)
        {
            TimerText.text = $"Voting Begins In: {currentTime}s";
            currentTime--;
            yield return new WaitForSeconds(1);
        }

        StartCoroutine(TimerVotingEnds());
    }
    private IEnumerator TimerVotingEnds()
    {
        _isTimeForVote = true;

        var currentTime = _votingEndsIn;

        while (currentTime > -1)
        {
            TimerText.text = $"Voting Ends In: {currentTime}s";
            currentTime--;
            yield return new WaitForSeconds(1);

            if (_playerCount == _deadPlayerCount + _votedPlayerCount)
            {
                break;
            }
        }

        _isTimeForVote = false;
        StartCoroutine(TimerProceeding());
    }
    private IEnumerator TimerProceeding()
    {
        ShowVoteCount();

        var actNumForKick = GetPlayerForKick();



        var currentTime = _proceedingIn;

        while (currentTime > -1)
        {
            TimerText.text = $"Proceeding In: {currentTime}s";
            currentTime--;
            yield return new WaitForSeconds(1);
        }

        OnKickPlayer?.Invoke(actNumForKick);

        this.transform.parent.GetComponent<Animator>().SetTrigger("CloseVotingUI");

        ResetTablet();
    }

    #endregion

    private void ShowVoteCount()
    {
        var index = -1;

        foreach (var player in _players)
        {
            if (!player.CompareTag("Player")) continue;

            index++;

            var currentVoted = _votingSheet[player.ControllerActorNr];

            if (currentVoted.Count == 0) continue;

            var blockVoted = _users[index].Find("Content/BlockVoted");

            for (var i = 0; i < currentVoted.Count; i++)
            {
                blockVoted.GetChild(i).gameObject.SetActive(true);
            }
        }

        var currentSkipped = _votingSheet[SKIP];

        if (currentSkipped.Count != 0)
        {
            var blockSkipped = transform.Find("Panel/SkipVoting/BlockVoted");
            for (var i = 0; i < currentSkipped.Count; i++)
            {
                blockSkipped.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    private int GetPlayerForKick()
    {
        var listVoted = _votingSheet[SKIP];
        var actNum = -1;

        foreach (var el in _votingSheet)
        {

            if (el.Value.Count == listVoted.Count)
            {
                actNum = -1;
            }

            if (el.Value.Count > listVoted.Count)
            {
                listVoted = el.Value;
                actNum = el.Key;
            }
        }

        if (listVoted.Count < (_playerCount - _deadPlayerCount) / 2)
        {
            actNum = -1;
        }

        return actNum;
    }


    private void ResetTablet()
    {
        for (var i = 0; i < _playerCount; i++)
        {
            var blockVoted = _users[i].Find("Content/BlockVoted");

            for (var j = 0; j < blockVoted.childCount; j++)
            {
                blockVoted.GetChild(j).gameObject.SetActive(false);
            }

            _users[i].Find("Content/SignVoted").gameObject.SetActive(false);
            _users[i].Find("Content/SignReported").gameObject.SetActive(false);
            _users[i].Find("DeathPlayer").gameObject.SetActive(false);
        }

        var blockSkipped = transform.Find("Panel/SkipVoting/BlockVoted");
        for (var i = 0; i < blockSkipped.childCount; i++)
        {
            blockSkipped.GetChild(i).gameObject.SetActive(false);
        }

        var msgInChat = new Transform[Content.childCount];

        for (var i = 0; i < Content.childCount; i++)
        {
            msgInChat[i] = Content.GetChild(i);
        }

        for (var i = 0; i < msgInChat.Length; i++)
        {
            DestroyImmediate(msgInChat[i].gameObject);
        }
    }


    #region Callback

    private void OnTabletOpened(int finderID)
    {
        _deadPlayerCount = 0;
        _votedPlayerCount = 0;

        _isVoted = false;

        CheckStatusMineDead();
        CreateTabletContent(finderID);

        StartCoroutine(TimerVotingBegins());
    }

    private void OnPlayerVoted(int actorKicked, int actorSender)
    {
        _votingSheet[actorKicked].Add(actorSender);
        UpdateVotedPlayer(actorSender);
        _votedPlayerCount++;
    }

    #endregion



    private void OnDisable()
    {
        LevelManager.OnTabletOpened -= OnTabletOpened;
        LevelManager.OnPlayerVoted -= OnPlayerVoted;
    }


}
