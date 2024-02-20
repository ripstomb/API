using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpH : MonoBehaviour
{
    public RawImage[] images;
    public TextMeshProUGUI uidText;
    public TextMeshProUGUI[] CardText;
    public TextMeshProUGUI userTexts;

    private int ImageIndex;
    private string fakeApiUrl = "https://my-json-server.typicode.com/ripstomb/API";
    private string RickyMortyApiurl = "https://rickandmortyapi.com/api";
    private int currentUid = 1;

    private void Start()
    {
        UpdateUidText(); 
    }
    public void SendRequest()
    {
        StartCoroutine(GetUserData(currentUid));
    }

    public void IncrementUid()
    {
        if (currentUid <3)
        { 
            currentUid++;
            SendRequest();
            UpdateUidText();
        }
        else
        {
            Debug.Log("Ya no hay mas usuarios, debes regresar");
        }
    }

    public void DecrementUid()
    {
        if (currentUid > 1) 
        {
            currentUid--;
            SendRequest();
            UpdateUidText();
        }
        else
        {
            Debug.Log("No hay nada mas hacia atrás");
        }
    }

    private void UpdateUidText()
    {
        if (uidText != null)
        {

            uidText.text = "Current UID: " + currentUid;
        }
    }


    IEnumerator GetUserData(int uid)
    {
        UnityWebRequest request = UnityWebRequest.Get(fakeApiUrl + "/users/" + uid);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                UserData user = JsonUtility.FromJson<UserData>(request.downloadHandler.text);

                Debug.Log(user.username);

                if (userTexts != null)
                {
                    userTexts.text = "Current User: " + user.username;
                }

                int maxItems = Mathf.Min(user.deck.Length, Mathf.Min(images.Length, CardText.Length));

                for (int i = 0; i < maxItems; i++)
                {
                    StartCoroutine(GetCharacter(user.deck[i], i));
                }
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
    }

    IEnumerator GetCharacter(int id, int itemIndex)
    {
        UnityWebRequest request = UnityWebRequest.Get(RickyMortyApiurl + "/character/" + id);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                CharacterData character = JsonUtility.FromJson<CharacterData>(request.downloadHandler.text);

                Debug.Log(character.name + " is a " + character.species);

                if (itemIndex < images.Length)
                {
                    StartCoroutine(DownloadImage(character.image, itemIndex));
                }

                if (itemIndex < CardText.Length)
                {
                    CardText[itemIndex].text = character.name;
                }
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
    }

    IEnumerator DownloadImage(string url, int imageIndex)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            images[imageIndex].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    [System.Serializable]
    public class JsonData
    {
        public InfoData info;
        public CharacterData[] results;
        public UserData[] users;
    }

    [System.Serializable]
    public class UserData
    {
        public int id;
        public string username;
        public int[] deck;
    }
    [System.Serializable]
    public class CharacterData
    {
        public int id;
        public string name;
        public string species;
        public string image;
    }
    [System.Serializable]
    public class InfoData
    {
        public int count;
        public int pages;
        public string next;
        public string prev;
    }
}

