using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
public class TaskCompleted : ITaskCompleted
{
    public void CompletedSuccessfully(string Room, string Task)
    {
        var data = new Dictionary<string, string>()
        {
            { "Room", Room },
            { "Task", Task },
        };

        var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        var sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(51, data, options, sendOptions);
    }
}
