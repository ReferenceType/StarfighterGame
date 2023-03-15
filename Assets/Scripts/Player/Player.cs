using Assets;
using NetworkLibrary;
using NetworkLibrary.Utils;
using ProtoBuf;
using Protobuff;
using Protobuff.P2P;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[ProtoContract]
public class PlayerInfo : IProtoMessage
{
    [ProtoMember(1)]
    public string PlayerName;
    [ProtoMember(2)]
    public Guid PlayerId;
}
public class Player : MonoBehaviour
{
    public GameObject instantiationReference;
    public AudioSource MissileTargettingSound;
    public GameObject Spaceship;

    private static RelayClient client;
    private string playerName;
    private BulletPool bulletPool;
    private MissilePool missilePool;
    private UserDamageTracker damageTracker;
    private DamageUIText damageText;
    private PlayerControl playerController;
    private ConcurrentQueue<Action> marshallQueue = new ConcurrentQueue<Action>();
    private ConcurrentDictionary<Guid, RemotePlayer> remotePlayers = new ConcurrentDictionary<Guid, RemotePlayer>();

    internal static string sessionId = "";
    public Guid PlayerId => client.sessionId;
    public static Player Instance;

    private ConcurrentDictionary<Guid,bool> deadPlayers =  new ConcurrentDictionary<Guid, bool>();
    void Start()
    {
        Instance = this;
        QualitySettings.SetQualityLevel(3, true);
        Application.targetFrameRate = 60;
        Serializer.PrepareSerializer<MessageEnvelope>();
        BufferPool.StopCollectGcOnIdle();
        MiniLogger.LogInfo += (s) => Debug.Log(s);
        MiniLogger.LogWarn += (s) => Debug.Log(s);
        MiniLogger.LogDebug += (s) => Debug.Log(s);
        MiniLogger.LogError += (s) => Debug.LogError(s);

        var sp = SpawnPointSelector.GetSpawnPoint();
        this.transform.position= sp.transform.position;
        this.transform.rotation= sp.transform.rotation;

        playerName = StartUi.playerName??"Ufo";
        bulletPool = FindObjectOfType<BulletPool>();
        missilePool = FindObjectOfType<MissilePool>();
        damageTracker = GetComponent<UserDamageTracker>();
        damageText = FindObjectOfType<DamageUIText>();
        playerController = GetComponent<PlayerControl>();
        damageTracker.DamageReceived += OnDamageReceived;
        damageTracker.ImDead += OnPlayerDead;
        damageTracker.Resetting += OnPlayerReborn;

        var path = Application.streamingAssetsPath + "/client.pfx";
        var cert = new X509Certificate2(path, "greenpass");
        client = new RelayClient(cert);
        client.OnPeerRegistered += PeerRegistered;
        client.OnPeerUnregistered += PeerUnRegistered;
        client.OnMessageReceived += MessageReceived;
        client.OnUdpMessageReceived += MessageReceived;
        StartClient();
    }

   
    void LoadConnectionInfo(out string serverIp, out int port)
    {
        string configFile = Application.streamingAssetsPath + "/Config.xml";
        var doc = XDocument.Load(configFile);
        serverIp = doc.Root.Descendants().Where(x => x.Name == "ServerIp").First().Value;
        port =int.Parse( doc.Root.Descendants().Where(x => x.Name == "ServerPort").First().Value);
    }
    private void OnPlayerReborn()
    {
        deadPlayers.TryRemove(PlayerId, out _);
        var msg = new MessageEnvelope() { Header = "PlayerReborn" };
        foreach (var playerId in remotePlayers.Keys)
        {
            client.SendAsyncMessage(playerId, msg);
        }
    }

    private void OnPlayerDead(Guid killerId)
    {
        ScoreBoard.IncrementKills(killerId);
        ScoreBoard.IncrementDeaths(PlayerId);

        deadPlayers.TryAdd(PlayerId, false);

        if (remotePlayers.TryGetValue(killerId, out var remote))
        {
            UIDeathText.WriteText("Killed by\n" + remote.PlayerName);
        }
        else
        {
            UIDeathText.WriteText("You have killed yourself\nGood Work!");
        }

        var msg = new MessageEnvelope() { Header = "PlayerDead" };
        msg.KeyValuePairs = new Dictionary<string, string>();
        var bytes = killerId.ToByteArray();
        msg.SetPayload(bytes, 0, bytes.Length);
        foreach (var playerId in remotePlayers.Keys)
        {
            client.SendAsyncMessage(playerId, msg);
        }
    }
    internal bool IsPlayerDead(Guid playerId)
    {
        if (deadPlayers.ContainsKey(playerId))
        {
            return true;
        }
        return false;
    }
    private void OnDamageReceived(Guid remotePlayerId, int obj)
    {
        var dmgMessage = new MessageEnvelope();
        dmgMessage.Header = "DamageReceived";
        dmgMessage.KeyValuePairs = new Dictionary<string, string>();
        dmgMessage.KeyValuePairs["D"] = obj.ToString();
        client.SendAsyncMessage(remotePlayerId, dmgMessage);
    }

    private void StartClient()
    {
        Task.Run(async () =>
        {
            try
            {
                LoadConnectionInfo(out var serverIp, out int serverPort);
                await client.ConnectAsync(Dns.GetHostAddresses(serverIp)[0].ToString(), serverPort);
            
                marshallQueue.Enqueue(() => sessionId = client.sessionId.ToString());
                marshallQueue.Enqueue(() => MapColliders(Spaceship, sessionId));
                marshallQueue.Enqueue(() => ScoreBoard.AddPlayer(playerName, PlayerId, true));
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return;
            }
            client.StartPingService();
        });

    }

    private void MapColliders(GameObject Spaceship, string name)
    {
        var transforms = Spaceship.GetComponentsInChildren<Transform>();
        foreach (Transform t in transforms)
        {
            t.gameObject.name = name;
            EnvironmentObjectTracker.sceneObjectsByObject.Add(t.gameObject, name);
        }
        EnvironmentObjectTracker.sceneObjectsById.Add(name, Spaceship);
    }

    private void MessageReceived(MessageEnvelope message)
    {
        try
        {
            switch (message.Header)
            {
                case "PlayerIdRequest":
                    HandlePlayerIdRequest(message);
                    break;
                case "PlayerIdRsponse":
                    HandlePlayerIdResponse(message);
                    break;
                case "PlayerState":
                    HandlePlayerStateUpdate(message);
                    break;
                case "DamageReceived":
                    HandleDamageNotification(message);
                    break;
                case "PlayerDead":
                    HandleRemotePlayerDead(message);
                    break;
                case "PlayerReborn":
                    HandleRemotePlayerReborn(message);
                    break;
                case "MissileTargetting":
                    HandleMissileTargetting(message);
                    break;

            }
        }
        catch (Exception e) { Debug.LogError(e.Message); }

    }

    private void HandleMissileTargetting(MessageEnvelope message)
    {
        marshallQueue.Enqueue(()=> { 
            if (!MissileTargettingSound.isPlaying)
                MissileTargettingSound.Play();
            //if(remotePlayers.TryGetValue(message.From,out var rp))
            //{
            //    damageText.UpdateText(rp.PlayerName+" is targetting you" );
            //}
          
            });
    }

    private void HandleRemotePlayerReborn(MessageEnvelope message)
    {
        deadPlayers.TryRemove(message.From, out _);

        marshallQueue.Enqueue(() =>
        remotePlayers[message.From].GetComponentInChildren<DeathAnimation>().ResetShip());
    }

    private void HandleRemotePlayerDead(MessageEnvelope message)
    {
        deadPlayers.TryAdd(message.From, false);

        message.LockBytes();
        marshallQueue.Enqueue(() =>
        {
            RemotePlayer remote = remotePlayers[message.From];
            remote.gameObject.GetComponentInChildren<DeathAnimation>().Explode();
            Guid killer = new Guid(message.Payload);
            ScoreBoard.IncrementDeaths(message.From);
            ScoreBoard.IncrementKills(killer);
            if (killer.ToString() == (sessionId))
            {
                damageText.UpdateText("Confirmed Kill");
            }
        });
    }

    private void HandleDamageNotification(MessageEnvelope message)
    {
        damageText.UpdateText(message.KeyValuePairs["D"]);
    }

    private void HandlePlayerIdRequest(MessageEnvelope message)
    {
        Task.Run(async () =>
        {
            while (client.sessionId.Equals(Guid.Empty))
            {
                Debug.LogError("session Id was empty!");
                await Task.Delay(20);

            }
            try
            {
                var idMsg = new PlayerInfo() { PlayerName = playerName, PlayerId = client.sessionId };
                client.SendAsyncMessage(message.From, idMsg, "PlayerIdRsponse");
            } catch { }
            
        });
      
    }
    private async void HandlePlayerIdResponse(MessageEnvelope message)
    {
        var info = message.UnpackPayload<PlayerInfo>();
        try
        {
            var playerClone = await RemotePlayerInstantiator.InstantiatePlayer(instantiationReference);
            marshallQueue.Enqueue(() =>
            {
                playerClone.Id = info.PlayerId;
                playerClone.PlayerName = info.PlayerName ?? "Ufo";

                RemotePlayerNameManager.AddPlayer(playerClone.gameObject, playerClone.PlayerName);
                GetComponentInChildren<Radar>().AddEnemy(playerClone.gameObject);

                MapColliders(playerClone.gameObject, info.PlayerId.ToString());
                ScoreBoard.AddPlayer(playerClone.PlayerName, info.PlayerId);
            });
            remotePlayers.TryAdd(message.From, playerClone);
        }
        catch (Exception e) { Debug.LogError(e.Message); }

    }

    private void PeerRegistered(Guid peerId)
    {
        var reqId = new MessageEnvelope() { Header = "PlayerIdRequest" };
        client.SendAsyncMessage(peerId, reqId);
        if (client.sessionId.CompareTo(peerId) > 0)
        {
            Task.Run(async () => 
            {
                try
                {
                    await Task.Delay(1000);
                    var res = await client.RequestHolePunchAsync(peerId, 5000);
                    if (!res)
                        Debug.LogError("Holepunch failed");
                }
                catch (Exception e){ Debug.LogError(e.Message); }
              

            });
        }
    }
    private void PeerUnRegistered(Guid obj)
    {
        marshallQueue.Enqueue(() =>
        {
            if (remotePlayers.TryRemove(obj, out var remoteClone))
            {
                Destroy(remoteClone.gameObject);
                GetComponentInChildren<Radar>().RemoveEnemy(remoteClone.gameObject);
                RemotePlayerNameManager.RemovePlayer(remoteClone.gameObject);
            }
        });
    }
    void FixedUpdate()
    {
        PublishState();

        while (marshallQueue.TryDequeue(out var todo))
        {
            todo.Invoke();
        }
    }

    int skip = 0;
    ulong stateSeqNumber = 0;
    private void PublishState()
    {
       // skip++;
        //if (skip % 10 == 0 || skip % 11 == 0)
        //    return;
        PlayerState state = new PlayerState();
        state.bulletStates = bulletPool.GetBulletStates();
        state.missileStates = missilePool.GetMissileStates();
        state.playerCoordinates = new TransformState(Spaceship.transform.position, Spaceship.transform.rotation);
        state.TimeStamp = DateTime.UtcNow;
        state.sequenceNumber = stateSeqNumber++;
        state.UserInputBoosterOn = playerController.BoosterOn;
        state.UserInputKeyboardDirection = new ProtoVector2( playerController.UserInputDirection);
        state.UserInputMouseDirection = new ProtoVector2( playerController.UserMousePosition);
        foreach (var rPlayer in remotePlayers)
        {
            client.SendUdpMesssage(rPlayer.Key, state);
        }

    }
    private void HandlePlayerStateUpdate(MessageEnvelope message)
    {
        var state = message.UnpackPayload<PlayerState>();
        state.PlayerId = message.From;
        if (remotePlayers.TryGetValue(message.From, out var remotePlayer))
        {
            remotePlayer.UpdatePosition(state.sequenceNumber, state);
            marshallQueue.Enqueue(() =>
            {
                if (state.sequenceNumber <= remotePlayer.lastSequence)
                {
                    return;
                }
                remotePlayer.lastSequence = state.sequenceNumber;
                remotePlayer.lastTimestamp = state.TimeStamp;
                missilePool.UpdateMissiles(message.From, state.missileStates);
               
            });

        }

    }

    public static void SendTargettedMessage(RemotePlayer remotePlayer)
    {
        client.SendUdpMesssage(remotePlayer.Id, new MessageEnvelope() { Header = "MissileTargetting" });
    }
    private void OnDestroy()
    {
        client.Disconnect();
    }
}
