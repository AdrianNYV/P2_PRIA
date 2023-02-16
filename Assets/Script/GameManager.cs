using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public RawImage image;

    void Start() {
        // A correct website page.
        StartCoroutine(GetRequest("https://servizos.meteogalicia.gal/mgrss/observacion/jsonCamaras.action"));
    }

    IEnumerator GetRequest(string uri) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    ClaseMeteoResp claseMeteoResp = JsonUtility.FromJson<ClaseMeteoResp>(webRequest.downloadHandler.text);
                    print(claseMeteoResp.listaCamaras[0].imaxeCamara);
                    StartCoroutine(DescargaImagen(claseMeteoResp.listaCamaras[0].imaxeCamara));
                    break;
            }
        }
    }

    IEnumerator DescargaImagen(string uri){
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.downloadHandler = new DownloadHandlerTexture();
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (UnityWebRequest.Result.Success == webRequest.result)
            {
                image.texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
            }else{
                Debug.Log("No se descargó la imagen");
            }
        }
    }
}