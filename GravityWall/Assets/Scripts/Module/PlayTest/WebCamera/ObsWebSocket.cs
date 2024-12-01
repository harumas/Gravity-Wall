using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using WebSocketSharp;

// OBS Document
// https://github.com/obsproject/obs-websocket/blob/master/docs/generated/protocol.md
public class ObsWebSocket
{
    private WebSocket socket;
    private string password;

    public class BaseMessage
    {
        [JsonProperty("op")] public virtual int Op { get; set; }
    }

    private class MessageHelloDataKeys
    {
        [JsonProperty("obsWebSocketVersion")] public string ObsWebSocketVersion { get; set; }
        [JsonProperty("rpcVersion")] public int RPCVersion { get; set; }
        [JsonProperty("authentication")] public Authentication Auth { get; set; }
    }

    private class Authentication
    {
        [JsonProperty("challenge")] public string Challenge { get; set; }
        [JsonProperty("salt")] public string Salt { get; set; }
    }

    private class MessageHello : BaseMessage
    {
        [JsonProperty("op")] public override int Op { get; set; }

        [JsonProperty("d")] public MessageHelloDataKeys D { get; } = new();
    }

    private class MessageIdentifyDataKeys
    {
        [JsonProperty("rpcVersion")] public int RPCVersion { get; } = 1;

        [JsonProperty("authentication")] public string Auth { get; set; }
    }

    private class MessageIdentify : BaseMessage
    {
        [JsonProperty("op")] public override int Op { get; set; } = 1;

        [JsonProperty("d")] public MessageIdentifyDataKeys D { get; set; } = new();

        public MessageIdentify()
        {
        }

        public MessageIdentify(string auth)
        {
            D.Auth = auth;
        }
    }

    public class MessageRequest : BaseMessage
    {
        [JsonProperty("op")] public override int Op { get; set; } = 6;

        [JsonProperty("d")] public MessageRequestDataKeys D { get; set; }

        public MessageRequest(string requestType, string requestId, RequestData requestData)
        {
            D = new MessageRequestDataKeys(requestType, requestId, requestData);
        }
    }
    
    public class ProfileMessageRequest : BaseMessage
    {
        [JsonProperty("op")] public override int Op { get; set; } = 6;

        [JsonProperty("d")] public ProfileRequestDataKeys D { get; set; }

        public ProfileMessageRequest(string requestType, string requestId, ProfileRequestData requestData)
        {
            D = new ProfileRequestDataKeys(requestType, requestId, requestData);
        }
    }

    public class MessageRequestDataKeys
    {
        [JsonProperty("requestType")] public string RequestType { get; }

        [JsonProperty("requestId")] public string RequestId { get; }

        [JsonProperty("requestData")] public RequestData RequestData { get; }

        public MessageRequestDataKeys(string requestType, string requestId, RequestData requestData)
        {
            RequestType = requestType;
            RequestId = requestId;
            RequestData = requestData;
        }
    }
    
    public class ProfileRequestDataKeys
    {
        [JsonProperty("requestType")] public string RequestType { get; }

        [JsonProperty("requestId")] public string RequestId { get; }

        [JsonProperty("requestData")] public ProfileRequestData RequestData { get; }

        public ProfileRequestDataKeys(string requestType, string requestId, ProfileRequestData requestData)
        {
            RequestType = requestType;
            RequestId = requestId;
            RequestData = requestData;
        }
    }

    public class RequestData
    {
        [JsonProperty("inputName")] public string InputName { get; }
        [JsonProperty("mediaAction")] public string MediaAction { get; }

        public RequestData(string inputName, string mediaAction)
        {
            InputName = inputName;
            MediaAction = mediaAction;
        }
    }

    public class ProfileRequestData
    {
        [JsonProperty("parameterCategory")] public string ParameterCategory { get; }
        [JsonProperty("parameterName")] public string ParameterName { get; }
        [JsonProperty("parameterValue")] public string ParameterValue { get; }

        public ProfileRequestData(string parameterCategory, string parameterName, string parameterValue)
        {
            ParameterCategory = parameterCategory;
            ParameterName = parameterName;
            ParameterValue = parameterValue;
        }
    }

    public void Connect(int port, string password)
    {
        this.password = password;
        socket = new WebSocket($"ws://localhost:{port}");

        socket.OnOpen += (_, _) => { Debug.Log("WebSocket 接続成功"); };

        socket.OnMessage += (_, e) =>
        {
            Debug.Log("OnMessage");
            Debug.Log($"Res: {e.Data}");

            var res = JsonConvert.DeserializeObject<BaseMessage>(e.Data);

            if (res.Op == 0)
            {
                var hello = JsonConvert.DeserializeObject<MessageHello>(e.Data);
                if (hello == null) return;

                Debug.Log($"Receive Hello: {hello.D.ObsWebSocketVersion}, {hello.D.RPCVersion}");

                if (hello.D.Auth == null)
                {
                    socket.Send(CreateMessage(new MessageIdentify()));
                }
                else
                {
                    Debug.Log($"Auth: Challenge: {hello.D.Auth.Challenge}, Salt: {hello.D.Auth.Salt}");
                    var auth = CreateAuth(hello.D.Auth);
                    socket.Send(CreateMessage(new MessageIdentify(auth)));
                }
            }
        };

        socket.OnError += (_, e) => { Debug.Log("エラー: " + e.Message); };
        socket.OnClose += (_, e) => { Debug.Log($"WebSocket 切断： {e.Reason}"); };
        socket.Connect();
    }

    public void SendMessage(BaseMessage message)
    {
        socket.Send(CreateMessage(message));
    }

    private string CreateMessage(BaseMessage message)
    {
        var json = JsonConvert.SerializeObject(message);
        Debug.Log(json);
        return json;
    }

    private string CreateAuth(Authentication auth)
    {
        string base64Secret = CreateSHA256($"{password}{auth.Salt}");
        string authentication = CreateSHA256($"{base64Secret}{auth.Challenge}");
        return authentication;
    }

    private string CreateSHA256(string input)
    {
        byte[] passConcat = Encoding.UTF8.GetBytes(input);
        using var sha256 = SHA256.Create();
        byte[] sha256Hash = sha256.ComputeHash(passConcat);
        return CreateBase64Secret(sha256Hash);
    }

    private string CreateBase64Secret(byte[] input)
    {
        return Convert.ToBase64String(input);
    }

    public void Close()
    {
        socket.Close();
        socket = null;
    }
}