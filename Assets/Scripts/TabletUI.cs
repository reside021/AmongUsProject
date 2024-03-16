using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TabletUI : MonoBehaviour
{
    public GameObject BlockSkippedButton;
    public GameObject ChatBlock;
    public Button SkipVoteBtn;
    public Button OpenChatBtn;
    public Button OpenKickTableBtn;
    public Button VoteCancelBtn;



    public Transform UsersBlock;

    private int _playerCount = 0;
    private Transform[] _users;
    private bool _isMineDead = false;

    private void OnEnable()
    {
        LevelManager.OnTabletOpened += OnTabletOpened;
    }

    void Start()
    {
        AddListenerForButton();

        _playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        _users = SetUsers();
        CheckUsersCount();
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

    private void AddListenerForButton()
    {
        OpenChatBtn.onClick.AddListener(() =>
        {
            ChatBlock.SetActive(true);
        });

        SkipVoteBtn.onClick.AddListener(() =>
        {
            BlockSkippedButton.SetActive(true);
        });

        OpenKickTableBtn.onClick.AddListener(() =>
        {
            ChatBlock.SetActive(false);
        });

        VoteCancelBtn.onClick.AddListener(() =>
        {
            VoteCanceled(BlockSkippedButton);
        });
    }

    private void OnTabletOpened(int finderID)
    {
        CheckStatudMineDead();
        CreateTabletContent(finderID);




    }

    private void CheckStatudMineDead()
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

        foreach (var player in PhotonNetwork.PhotonViewCollection)
        {
            if (!player.CompareTag("Player")) continue;

            index++;

            var nameUser = _users[index].Find("Content/NameUser");
            nameUser.GetComponent<TextMeshProUGUI>().text = player.Controller.NickName;

            if (player.ViewID == finderID)
            {
                _users[index].Find("Content/SignReported").gameObject.SetActive(true);
            }

            var playerDead = player.gameObject.GetComponent<PlayerController>().IsDead;

            if (playerDead)
            {
                _users[index].Find("DeathPlayer").gameObject.SetActive(true);
            }
            
            if (_isMineDead) continue;

            if (player.IsMine || playerDead) continue;

            var blockButton = _users[index].Find("Content/BlockButton");
            var userBlockAsBtn = _users[index].GetComponent<Button>();

            userBlockAsBtn.onClick.AddListener(() =>
            {
                VoteCanceled(blockButton.gameObject);
                blockButton.gameObject.SetActive(true);
            });

            var voteConfirm = blockButton.Find("VoteConfirm").gameObject.GetComponent<Button>();
            var voteCancel = blockButton.Find("VoteCancel").gameObject.GetComponent<Button>();

            voteConfirm.onClick.AddListener(VoteConfirmed);
            voteCancel.onClick.AddListener(() => VoteCanceled(blockButton.gameObject));


        }
    }

    private void VoteConfirmed()
    {
        // отправка голоса 
    }

    public void VoteCanceled(GameObject blockButton)
    {
        blockButton.SetActive(false);
    }

    private void OnDisable()
    {
        LevelManager.OnTabletOpened -= OnTabletOpened;
    }

}
