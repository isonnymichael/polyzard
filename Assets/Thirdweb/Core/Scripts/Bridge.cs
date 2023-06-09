
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Thirdweb
{
    public class Bridge
    {
        [System.Serializable]
        private struct Result<T>
        {
            public T result;
        }

        [System.Serializable]
        private struct RequestMessageBody
        {
            public RequestMessageBody(string[] arguments)
            {
                this.arguments = arguments;
            }
            public string[] arguments;
        }

        private struct GenericAction
        {
            public Type t;
            public Delegate d;

            public GenericAction(Type t, Delegate d)
            {
                this.t = t;
                this.d = d;
            }
        }

        private static Dictionary<string, TaskCompletionSource<string>> taskMap = new Dictionary<string, TaskCompletionSource<string>>();
        private static Dictionary<string, GenericAction> taskActionMap = new Dictionary<string, GenericAction>();

        [AOT.MonoPInvokeCallback(typeof(Action<string, string, string>))]
        private static void jsCallback(string taskId, string result, string error)
        {
            if (taskMap.ContainsKey(taskId))
            {
                if (error != null)
                {
                    taskMap[taskId].TrySetException(new Exception(error));
                }
                else
                {
                    taskMap[taskId].TrySetResult(result);
                }
                taskMap.Remove(taskId);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void jsAction(string taskId, string result)
        {
            if (taskActionMap.ContainsKey(taskId))
            {
                Type tempType = taskActionMap[taskId].t;
                taskActionMap[taskId].d.DynamicInvoke(tempType == typeof(string) ? result : JsonConvert.DeserializeObject(result, tempType));
            }
        }

        public static void Initialize(string chainOrRPC, ThirdwebSDK.Options options)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("Initializing the thirdweb SDK is not supported in the editor. Please build and run the app instead.");
                return;
            }
            ThirdwebInitialize(chainOrRPC, Utils.ToJson(options));
        }

        public static async Task<string> Connect(WalletConnection walletConnection)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("Connecting wallets is not supported in the editor. Please build and run the app instead.");
                return Utils.AddressZero;
            }
            var task = new TaskCompletionSource<string>();
            string taskId = Guid.NewGuid().ToString();
            taskMap[taskId] = task;
            ThirdwebConnect(taskId, walletConnection.provider.ToString(), walletConnection.chainId, jsCallback);
            string result = await task.Task;
            return result;
        }

        public static async Task Disconnect()
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("Disconnecting wallets is not supported in the editor. Please build and run the app instead.");
                return;
            }
            var task = new TaskCompletionSource<string>();
            string taskId = Guid.NewGuid().ToString();
            taskMap[taskId] = task;
            ThirdwebDisconnect(taskId, jsCallback);
            await task.Task;
        }

        public static async Task SwitchNetwork(int chainId)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("Switching networks is not supported in the editor. Please build and run the app instead.");
                return;
            }
            var task = new TaskCompletionSource<string>();
            string taskId = Guid.NewGuid().ToString();
            taskMap[taskId] = task;
            ThirdwebSwitchNetwork(taskId, chainId, jsCallback);
            await task.Task;
        }

        public static async Task<T> InvokeRoute<T>(string route, string[] body)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("Interacting with the thirdweb SDK is not supported in the editor. Please build and run the app instead.");
                return default(T);
            }
            var msg = Utils.ToJson(new RequestMessageBody(body));
            string taskId = Guid.NewGuid().ToString();
            var task = new TaskCompletionSource<string>();
            taskMap[taskId] = task;
            Debug.Log("InvokeRoute before");
            Debug.Log($"taskId : {taskId}");
            Debug.Log($"route : {taskId}");
            Debug.Log($"route : {msg}");
            
            ThirdwebInvoke(taskId, route, msg, jsCallback);
            string result = await task.Task;
            Debug.Log($"InvokeRoute Result: {result}");

            if(result == "user rejected transaction"){
                return default(T);
            }else{
                Debug.Log(JsonConvert.DeserializeObject<Result<T>>(result).result);
                return JsonConvert.DeserializeObject<Result<T>>(result).result;
            }

        }

        public static string InvokeListener<T>(string route, string[] body, Action<T> action)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("Interacting with the thirdweb SDK is not supported in the editor. Please build and run the app instead.");
                return null;
            }

            string taskId = Guid.NewGuid().ToString();
            taskActionMap[taskId] = new GenericAction(typeof(T), action);
            var msg = Utils.ToJson(new RequestMessageBody(body));
            ThirdwebInvokeListener(taskId, route, msg, jsAction);
            return taskId;
        }

        public static async Task FundWallet(FundWalletOptions payload)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("Interacting with the thirdweb SDK is not supported in the editor. Please build and run the app instead.");
                return;
            }
            var msg = Utils.ToJson(payload);
            string taskId = Guid.NewGuid().ToString();
            var task = new TaskCompletionSource<string>();
            taskMap[taskId] = task;
            ThirdwebFundWallet(taskId, msg, jsCallback);
            await task.Task;
        }

        [DllImport("__Internal")]
        private static extern string ThirdwebInvoke(string taskId, string route, string payload, Action<string, string, string> cb);
        [DllImport("__Internal")]
        private static extern string ThirdwebInvokeListener(string taskId, string route, string payload, Action<string, string> action);
        [DllImport("__Internal")]
        private static extern string ThirdwebInitialize(string chainOrRPC, string options);
        [DllImport("__Internal")]
        private static extern string ThirdwebConnect(string taskId, string wallet, int chainId, Action<string, string, string> cb);
        [DllImport("__Internal")]
        private static extern string ThirdwebDisconnect(string taskId, Action<string, string, string> cb);
        [DllImport("__Internal")]
        private static extern string ThirdwebSwitchNetwork(string taskId, int chainId, Action<string, string, string> cb);
        [DllImport("__Internal")]
        private static extern string ThirdwebFundWallet(string taskId, string payload, Action<string, string, string> cb);
    }
}
