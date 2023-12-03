using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.StringLoading;
using VRC.Udon.Common.Interfaces;

public class jsonObject : UdonSharpBehaviour
{
    public VRCUrl urlContent;
    public GameObject[] ObjectsToEnable;
    public GameObject[] ObjectsToDisable;

    public float Delay = 30; //How often the script will grab the latest information. [Do Not Set this lower than 15]
    private string Page;
    private string[] PageSplit;

    private bool isUser;
    private string localDisplayname;
    void Start()
    {
        //On Start we grab the Local Players display name, and send function call to "_Download" to prepare the "urlContent"
        localDisplayname = Networking.LocalPlayer.displayName;
        _Download();
    }
    public void _Download()
    {
        //Loading "urlContent" via Downloader, then re-calling itself using the Delay.
        VRCStringDownloader.LoadUrl(urlContent, (IUdonEventReceiver)this);
        SendCustomEventDelayedSeconds(nameof(_Download), Delay);
    }

    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        Page = result.Result;
        PageSplit = Page.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        isUser = false;
        foreach (string name in PageSplit) //If Username appears in List, and is the Local User, sets isUser to TRUE.
        {
            if (name == Networking.LocalPlayer.displayName)
            {
                isUser = true;
                break;
            }
        }
        //Enable & Disable of Objects based on isUser.
        foreach (GameObject obj in ObjectsToEnable)
        {
            obj.SetActive(isUser);
        }
        foreach (GameObject obj in ObjectsToDisable)
        {
            obj.SetActive(!isUser);
        }
    }
    public override void OnStringLoadError(IVRCStringDownload result)
    {
        Debug.Log(result.Error);
    }
}