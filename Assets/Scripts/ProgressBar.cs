using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {
    private RectTransform CanvasRect;
    private Image progressBarImage; //gas progress bar
    private Vector3 inWorldPosition = Vector3.zero;
    private System.Action onComplete;
    // Use this for initialization
    public void Init (Vector3 inWorldPosition, System.Action onComplete) {
        //set ui object to car position
        this.onComplete = onComplete;
        CanvasRect = GameManager.Instance.ui.GetComponent<RectTransform>();
        this.inWorldPosition = inWorldPosition;

        //creating unique material to avoiding sharing material properties with another progress bars
        Material mat = Instantiate(Resources.Load("gas_progress_material")) as Material;
        progressBarImage = GetComponent<Image>();
        progressBarImage.material = mat;
        progressBarImage.material.SetFloat("_Progress", 0);
        StartCoroutine(GasProgress(GameManager.Instance.InGasStationTime));
    }
    /// <summary>
    /// Progress bar filling
    /// </summary>
    /// <param name="time"> Time to fill</param>
    /// <returns></returns>
    IEnumerator GasProgress(float time)
    {
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * 1 / time;
            progressBarImage.material.SetFloat("_Progress", i);
            yield return 0;
        }
        onComplete.Invoke();
        Destroy(gameObject);
        yield return 1;
    }

    // Update is called once per frame
    void Update () {
        if (inWorldPosition != Vector3.zero)
        {
            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(inWorldPosition);
            ViewportPosition.x = Mathf.Clamp01(ViewportPosition.x);
            ViewportPosition.y = Mathf.Clamp01(ViewportPosition.y);

            Vector2 WorldObject_ScreenPosition = new Vector2((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f), (ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f));

            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(WorldObject_ScreenPosition.x, WorldObject_ScreenPosition.y + 10);
        }        
    }
}
