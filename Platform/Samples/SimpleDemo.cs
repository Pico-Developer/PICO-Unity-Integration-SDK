using System;
using System.Collections;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Pico.Platform.Samples
{
    public class SimpleDemo : MonoBehaviour
    {
        public bool useAsyncInit = true;

        public RawImage headImage;
        public Text nameText;
        public Text logText;

        // Start is called before the first frame update
        void Start()
        {
            Log($"UseAsyncInit={useAsyncInit}");
            if (useAsyncInit)
            {
                try
                {
                    CoreService.AsyncInitialize().OnComplete(m =>
                    {
                        if (m.IsError)
                        {
                            Log($"Async initialize failed: code={m.GetError().Code} message={m.GetError().Message}");
                            return;
                        }

                        if (m.Data != PlatformInitializeResult.Success && m.Data != PlatformInitializeResult.AlreadyInitialized)
                        {
                            Log($"Async initialize failed: result={m.Data}");
                            return;
                        }

                        Log("AsyncInitialize Successfully");
                        EnterDemo();
                    });
                }
                catch (Exception e)
                {
                    Log($"Async Initialize Failed:{e}");
                    return;
                }
            }
            else
            {
                try
                {
                    CoreService.Initialize();
                }
                catch (UnityException e)
                {
                    Log($"Init Platform SDK error:{e}");
                    throw;
                }

                EnterDemo();
            }
        }

        void EnterDemo()
        {
            UserService.RequestUserPermissions(new[] {Permissions.UserInfo, Permissions.FriendRelation}).OnComplete(m =>
            {
                if (m.IsError)
                {
                    Log($"Permission failed code={m.Error.Code} message={m.Error.Message}");
                    return;
                }

                Log($"RequestUserPermissions successfully:{String.Join(",", m.Data.AuthorizedPermissions)}");
                getUser();
            });
        }

        void getUser()
        {
            UserService.GetLoggedInUser().OnComplete(m =>
            {
                if (m.IsError)
                {
                    Debug.Log($"GetLoggedInUser failed:code={m.Error.Code} message={m.Error.Message}");
                    return;
                }

                StartCoroutine(DownloadImage(m.Data.ImageUrl, headImage));
                nameText.text = m.Data.DisplayName;
                Log($"DisplayName={m.Data.DisplayName} UserId={m.Data.ID}");
            });
        }

        IEnumerator DownloadImage(string mediaUrl, RawImage rawImage)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
            yield return request.SendWebRequest();
            if (request.responseCode != 200)
            {
                Log("Load image failed");
            }
            else
            {
                rawImage.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                rawImage.GetComponent<Renderer>().material.mainTexture = ((DownloadHandlerTexture) request.downloadHandler).texture;
            }
        }

        void Log(string s)
        {
            logText.text = s;
            Debug.Log(s);
        }
    }
}