using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MiniJSON;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class InAppPurchasing : MonoBehaviour
{
    public GameObject inAppPackPrefab;
    public GameObject inAppPackGrid;

    private void Start()
    {
        YGameLogin yGameLogin = new YGameLogin();
        StartCoroutine(yGameLogin.Login(LOGIN_TYPES.LOGIN_BY_QUICK));
        StartCoroutine(getListPayment());
    }

    private void Update()
    {

    }

    public IEnumerator getListPayment()
    {
        WWWForm form = new WWWForm();
        form.AddField("appId", "cb5e0e1690d3c6c53916617bbea6dc19");
        form.AddField("accessToken", PlayerPrefs.GetString("accessToken", ""));
        form.AddField("server_id", 0);
        form.AddField("version", 100);

        using (UnityWebRequest www = UnityWebRequest.Post("https://dev.ygame.vn/v1/user/sdk_info", form))
        {
            Debug.Log(System.Text.Encoding.UTF8.GetString(www.uploadHandler.data));

            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log("Response: \n" + www.downloadHandler.text);
                getInAppPackButtons(www.downloadHandler.text);
            }
        }
    }

    private ArrayList inAppPacks = new ArrayList();
    public void getInAppPackButtons(string response)
    {
        var sdkInfo = JsonUtility.FromJson<YGame.SDKInfo>(response); // sdkInfo is C# object created from api json response
        var productCatalog = ProductCatalog.LoadDefaultCatalog(); // Load default catalog from default catalog file

        for (int i = 0; i < sdkInfo.inAppConf.Count; i++)
        {
            inAppPacks.Add(Instantiate(inAppPackPrefab));

            // Save important info such as payment_id, etc. in InAppPacks for later API call usages
            var currentButton = ((GameObject)inAppPacks[i]);

            currentButton.GetComponent<InAppPackInfo>().paymentId = sdkInfo.inAppConf[i].paymentId;
            currentButton.transform.SetParent(inAppPackGrid.transform, false);

            currentButton.GetComponent<IAPButton>().onPurchaseComplete.AddListener(callGoogleInAppAPI);
            currentButton.GetComponent<IAPButton>().productId = sdkInfo.inAppConf[i].paymentId;

            // Get in app pack button textures dynamically from response icon urls
            StartCoroutine(getButtonTexture(sdkInfo.inAppConf[i].icon, i));

            productCatalog.Add(new ProductCatalogItem
            {
                id = sdkInfo.inAppConf[i].paymentId,
                type = ProductType.Consumable
            });
        }

        // Overwrite exisiting default catalog file 
        if (File.Exists("Assets/Resources/IAPProductCatalog.json"))
            File.WriteAllText("Assets/Resources/IAPProductCatalog.json", ProductCatalog.Serialize(productCatalog));
        else Debug.LogError("IAPProductCatalog.json does not exists!");

        for (int i = 0; i < inAppPacks.Count; i++)
            ((GameObject)inAppPacks[i]).SetActive(true);
    }

    IEnumerator getButtonTexture(string url, int indexInList)
    {

        WWW www = new WWW(url);
        yield return www;

        ((GameObject)inAppPacks[indexInList]).GetComponent<RawImage>().texture = www.texture;
    }

    private class ProductReceipt
    {
        public string packageName;
        public int productId;
        public string purchaseToken;
    }

    public void callGoogleInAppAPI(Product product)
    {
        apiResponseText.GetComponent<Text>().text = product.receipt;
        StartCoroutine(googleInAppAPI(product, "com.ygame.thienhangutuyet.0.99"));
    }

    public GameObject apiResponseText;
    private static string APP_ID = "cb5e0e1690d3c6c53916617bbea6dc19";
    IEnumerator googleInAppAPI(Product product, string paymentId)
    {
        var receiptDict = Json.Deserialize(product.receipt) as Dictionary<string, object>;
        var payload = receiptDict["Payload"] as string;
        var payloadDict = Json.Deserialize(payload) as Dictionary<string, object>;
        var payloadJson = payloadDict["json"] as string;
        var payloadJsonDict = Json.Deserialize(payloadJson) as Dictionary<string, object>;

        WWWForm form = new WWWForm();
        form.AddField("appId", APP_ID);
        form.AddField("accessToken", PlayerPrefs.GetString("accessToken", ""));
        form.AddField("server_id", 0);
        form.AddField("char_id", "0");
        form.AddField("payment_id", paymentId);
        form.AddField("packageName", (string)payloadJsonDict["packageName"]);
        form.AddField("productId", (string)payloadJsonDict["productId"]);
        form.AddField("purchaseToken", (string)payloadJsonDict["purchaseToken"]);

        // Generate signature from API params
        Dictionary<string, object> paramsDict = new Dictionary<string, object>();
        paramsDict.Add("appId", APP_ID);
        paramsDict.Add("accessToken", PlayerPrefs.GetString("accessToken", ""));
        paramsDict.Add("server_id", 0);
        paramsDict.Add("char_id", "0");
        paramsDict.Add("payment_id", paymentId);
        paramsDict.Add("packageName", (string)payloadJsonDict["packageName"]);
        paramsDict.Add("productId", (string)payloadJsonDict["productId"]);
        paramsDict.Add("purchaseToken", (string)payloadJsonDict["purchaseToken"]);
        form.AddField("sign", getSignature(paramsDict));
        //

        using (UnityWebRequest www = UnityWebRequest.Post("https://dev.ygame.vn/v1/user/payment_3_ewallet", form))
        {
            Debug.Log(System.Text.Encoding.UTF8.GetString(www.uploadHandler.data));

            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log("Response: \n" + www.downloadHandler.text);
                apiResponseText.GetComponent<Text>().text = www.downloadHandler.text;
            }
        }
    }

    public class InAppPurchasingData
    {
        public string appId;
        public string accessToken;
        public int server_id;
        public string char_id;
        public string payment_id;
        public string packageName;
        public string purchaseToken;
        public string productId;
    }

    private string encodeMD5(string input)
    {
        StringBuilder hash = new StringBuilder();
        MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
        byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

        for (int i = 0; i < bytes.Length; i++)
        {
            hash.Append(bytes[i].ToString("x2"));
        }
        return hash.ToString();
    }

    private static string APP_SECRET = "f7116909e769a248cecff1968904cafb";
    private string getSignature(Dictionary<string, object> dict)
    {
        string message = "";
        if (dict.Count > 0)
        {
            var dic = dict.OrderBy(s => s.Key);
            foreach (KeyValuePair<string, object> item in dic)
                message += item.Value.ToString();
        }
        if (!string.IsNullOrEmpty(message))
            message = encodeMD5(message + APP_SECRET);

        return message;
    }
}