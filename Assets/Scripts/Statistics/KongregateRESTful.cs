using UnityEngine;
using System.Collections;
using SimpleJSON;

public class KongregateRESTful : MonoBehaviour
{
    private const string AUTHENTICATION_URL = "http://www.kongregate.com/api/authenticate.json?";
    private const string API_KEY = "42745907-2198-4088-aa85-f884066e35a2";

    public string Error { get; set; }

    public string JSONResult { get; set; }

    public bool IsDone { get; set; }

	public void Authenticate(string userId, string gameAuthToken)
	{
        string fullUrl = AUTHENTICATION_URL + "user_id=" + userId + "&game_auth_token=" + gameAuthToken + "&api_key=" + API_KEY;
        

        StartCoroutine(WaitForRequest(fullUrl));
	}

    

    private IEnumerator WaitForRequest(string fullUrl)
    {
        WWW www = new WWW(fullUrl);
        yield return www;

        if(!string.IsNullOrEmpty(www.error))
        {
            Error = www.error;
        }
        else
        {
            JSONResult = www.text;
        }

        StartCoroutine(WaitForRequest(fullUrl));
    }
}
