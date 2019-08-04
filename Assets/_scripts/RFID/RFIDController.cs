using System;
using TMPro;
using UnityEngine;

namespace _scripts.RFID
{
    public class RFIDController: MonoBehaviour
    {

        private AndroidJavaObject mActivity;
        private AndroidJavaObject mIntent;
        private string sAction;
        private bool enable;
        public Action<string, RFIDErrorModel> returnId;

        public void WaitForNfc()
        {
            enable = true;
        }

        private void Update()
        {
            //if ( Application.platform != RuntimePlatform.Android ) return;
            if ( !enable ) return;
            try {
                // Create new NFC Android object
                mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"); // Activities open apps
                mIntent = mActivity.Call<AndroidJavaObject>("getIntent");
                sAction = mIntent.Call<string>("getAction"); // resulte are returned in the Intent object
                
                switch (sAction)
                {
                    case "android.nfc.action.NDEF_DISCOVERED":
                    {
                        var error = new RFIDErrorModel
                        {
                            type = RFIDErrorEnum.Unknow,
                            message = "Tag of type NDEF"
                        };
                        returnId.Invoke("", error);
                        enable = false;
                        return;
                    }
                    case "android.nfc.action.TECH_DISCOVERED":
                    {
                        Debug.Log("TAG DISCOVERED");
                        // Get ID of tag
                        AndroidJavaObject mNdefMessage = mIntent.Call<AndroidJavaObject>("getParcelableExtra", "android.nfc.extra.TAG");
                        if (mNdefMessage != null) {
                            byte[] payLoad = mNdefMessage.Call<byte[]>("getId");
                            Debug.Log($"string ? : {mNdefMessage.Call<string>("toString")}");
                            string text = System.Convert.ToBase64String(payLoad);
                            returnId.Invoke(text, null);
                            enable = false;
                        }
                        else
                        {
                            var error = new RFIDErrorModel
                            {
                                type = RFIDErrorEnum.IdNotFound,
                                message = "No Id Found"
                            };
                        
                            returnId.Invoke("", error);
                            enable = false;
                        }
                        mIntent.Call<AndroidJavaObject>("setAction", "");
                        return;
                    }
                    case "android.nfc.action.TAG_DISCOVERED":
                    {
                        var error = new RFIDErrorModel
                        {
                            type = RFIDErrorEnum.NotSupported,
                            message = "This type of tag is not supported !"
                        };
                        returnId.Invoke("", error);
                        enable = false;
                        break;
                    }
                }
            }
            catch (Exception ex) {
                string text = ex.Message;
                var error = new RFIDErrorModel
                {
                    type = RFIDErrorEnum.Unknow,
                    message = text
                };
                returnId.Invoke("", error);
            }
        }
    }
}
