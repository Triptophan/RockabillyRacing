//using UnityEngine;
//using System.Collections;

//public class KongregateAPI : MonoBehaviour
//{
//    #region Private members
//    #region Static
//    private static KongregateAPI _instance = null;

//    private static bool _isGuest = false;

//    private static string _username = "";
//    private static string _userId = "0";
//    private static string _gameAuthToken = "";

//    private static string _items = "";
//    #endregion Static

//    bool _jsAPILoaded;
//    string jsReturnValue;
//    #endregion Private members
    
//    static KongregateAPI Instance
//    {
//        get
//        {
//            if(!_instance)
//            {
//                new GameObject("KongregateAPI", typeof(KongregateAPI));
//            }

//            return _instance;
//        }
//    }

//    //Public API (Static Members)
//    #region Public Members
//    public static bool IsConnected { get { return Instance._jsAPILoaded; } }
//    public static bool IsGuest 
//    { 
//        get
//        {
//            Instance.CallJSFunction("isGuest()", "SetIsGuest");
//            return _isGuest;
//        }
//    }
//    public static string Username
//    {
//        get
//        {
//            Instance.CallJSFunction("getUserName()", "SetUsername");
//            return _username;
//        }
//    }
//    public static string UserId
//    {
//        get
//        {
//            Instance.CallJSFunction("getUserId()", "SetUserId");
//            return _userId;
//        }
//    }
//    public static string GameAuthToken
//    {
//        get
//        {
//            //Instance.CallJSFunction("getGameAuthToken()", "SetGameAuthToken");
//            return _gameAuthToken;
//        }
//    }
//    public static string[] Items
//    {
//        get
//        {
//            Instance.CallJSFunction("getUserItems()");
//            if (string.IsNullOrEmpty(_items)) return new string[0];
//            else return _items.Split(',');
//        }
//    }

//    #endregion Public Members

//    #region Public Methods
//    #region Static
//    public static string GetStatisticName(STATISTIC_NAME statName)
//    {
//        switch(statName)
//        {
//            case STATISTIC_NAME.TIMES_PLAYED:
//                return "TimesPlayed";
//            case STATISTIC_NAME.ENEMIES_KILLED:
//                return "EnemiesKilled";
//            default:
//                return "";
//        }
//    }

//    public static void ShowSignIn()
//    {
//        Instance.CallJSFunction("showSignInBox()");
//    }

//    public static void SubmitStat(string statistic, int value)
//    {
//        Instance.CallJSFunction(string.Format("submitStat('{0}', {1})", statistic, value));
//    }

//    public static void PurchaseItem(string item)
//    {
//        Debug.Log("Attempting purchase of " + item);
//        Instance.CallJSFunction(string.Format("purchaseItem('{0}')", item));
//    }
//    #endregion Static

//    public void LogMessage(string message)
//    {
//        Debug.Log(message);
//    }

//    public void SetIsGuest(object returnValue)
//    {
//        if(bool.TryParse(returnValue.ToString(), out _isGuest))
//        {
//            //Request username again if guest
//            if (_isGuest)
//                _username = "";
//        }
//    }
    
//    public void SetUsername(object returnValue)
//    {
//        _username = returnValue.ToString();
//    }

//    public void SetGameAuthToken(object returnValue)
//    {
//        _gameAuthToken = returnValue.ToString();
//    }
//    #endregion Public Methods

//    public enum STATISTIC_NAME
//    {
//        TIMES_PLAYED,
//        ENEMIES_KILLED
//    }

//    #region Member Methods
//    void CallJSFunction(string functionCall)
//    {
//        CallJSFunction(functionCall, null);
//    }

//    void CallJSFunction(string functionCall, string callback)
//    {
//        if(_jsAPILoaded)
//        {
//            if(string.IsNullOrEmpty(callback))
//            {
//                Application.ExternalEval(functionCall);
//            }
//            else
//            {
//                Application.ExternalEval(string.Format(@"var value = {0};SendUnityMessage('{1}', String(value));", functionCall, callback));
//            }
//        }
//    }
//    void OnLoaded(string error)
//    {
//        if (string.IsNullOrEmpty(error))
//        {
//            _jsAPILoaded = true;
//            Debug.Log("Connected");
//        }
//        else
//            Debug.LogError(error);
//    }

//    void SetUserId(object returnValue)
//    {
//        _userId = returnValue.ToString();
//    }

//    void SetUserItems(object returnValue)
//    {
//        _items = returnValue.ToString();
//    }

//    void OnKongregateAPILoaded(string userInfo)
//    {
//        Debug.Log("API Loaded");
//        _jsAPILoaded = true;

//        string[] userParams = userInfo.Split('|');
//        int userId = int.Parse(userParams[0]);
//        string userName = userParams[1];
//        string gameAuthToken = userParams[2];

//        if(userId == 0)
//        {
//            SetIsGuest(true);
//        }
//        else
//        {
//            SetUserId(userId);
//            SetUsername(userName);
//            SetGameAuthToken(gameAuthToken ?? "poop");
//            SetIsGuest(false);
//        }
//    }
//    #endregion Member Methods

//    #region Unity Methods
//    void Awake()
//    {
//        //Only allow one instance of the API bridge
//        if(_instance)
//        {
//            Destroy(gameObject);
//        }

//        _instance = this;
//    }

//    void Start()
//    {
//        Application.ExternalEval(@"
//            function isGuest()
//            {
//                return kongregate.services.isGuest();
//            }
//            
//            function getUserName()
//            {
//                var name = kongegrate.services.getUsername();
//                return name;
//            }
//
//            function getGameAuthToken()
//            {
//                return kongregate.services.getGameAuthToken();
//            }
//
//            function getUserId()
//            {
//                return kongregate.services.getUserId();
//            }
//
//            function showSignInBox()
//            {
//                if(kongregate.services.isGuest())
//                    kongregate.services.showSignInBox();
//            }
//
//            function submitStat(statName, value)
//            {
//                kongregate.stats.submit(statName, value);
//            }
//
//            function getUserItems()
//            {
//                kongregate.mtx.requstUserItemList(null, onUserItems);
//            }
//
//            function onUserItems(result)
//            {
//                if(result.success)
//                {
//                    var items = '';
//                    for(var i = 0; i < result.data.length; i++)
//                    {
//                        items += result.data[i].identifier;
//                        if(i < result.data.length - 1)
//                            items += ',';
//                    }
//                    SendUnityMessage('SetUserItems', items);
//                }
//            }
//
//            function purchaseItem(item)
//            {
//                kongregate.mtx.purchaseItems([item], onPurchaseResult);
//                SendUnityMessage('LogMessage', 'purchase sent...');
//            }
//
//            function onPurchaseResult(result)
//            {
//                if(result.success)
//                {
//                    SendUnityMessage('LogMessage', 'purchase complete...');
//                    getUserItems();
//                }
//            }
//
//            //Utility functions to send data back to Unity
//            function SendUnityMessage(functionName, message)
//            {
//                Log('Calling to: ' + functionName);
//                var unity = kongregateUnitySupport.getUnityObject();
//                unity.SendMessage('KongregateAPI', functionName, message);
//            }
//
//            function Log(message)
//            {
//                var unity = kongregateUnitySupport.getUnityObject();
//                unity.SendMessage('KongregateAPI', 'LogMessage', message);
//            }
//
//            if(typeof(kongregateUnitySupport) != 'undefined')
//            {
//                kongregateUnitySupport.initAPI('KongregateAPI', 'OnKongregateAPILoaded');
//            };
//        ");

//        DontDestroyOnLoad(gameObject);
//    }

//    void OnGUI()
//    {
//        //Display if connected or if the connection failed
//        if (IsConnected)
//            GUI.Box(new Rect(50, 50, 300, 300), "CONNECTED \n userID: " + _userId + "\n username: " + _username + "\n gameAuthToken: " + _gameAuthToken);
//        else
//            GUI.Box(new Rect(50, 50, 300, 300), "CONNECTION FAILED \n userID: " + _userId + "\n username: " + _username + "\n gameAuthToken: " + _gameAuthToken);
//    }

//    #endregion Unity Methods
//}
