using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    [SerializeField]
    private GameObject OptionsDialog;
    [SerializeField]
    private Text OptionsSpawnInterwalTime;
    [SerializeField]
    private Text OptionsInGasStationTime;
    [SerializeField]
    private Slider OptionsSpawnInterwalSlider;
    [SerializeField]
    private Slider OptionsInGasStationSlider;

    private float _spawnInterwal;
    public float SpawnInterwal {
        get { return _spawnInterwal; }
        set {
            OptionsSpawnInterwalSlider.value = value;
            OptionsSpawnInterwalTime.text = value.ToString();
            _spawnInterwal = value;
        }
    }

    private float _inGasStationTime;
    public float InGasStationTime
    {
        get { return _inGasStationTime; }
        set
        {
            OptionsInGasStationSlider.value = value;
            OptionsInGasStationTime.text = value.ToString();
            _inGasStationTime = value;
        }
    }

    public void OptionsShow()
    {
        OptionsDialog.SetActive(true);
        SpawnInterwal = PlayerPrefs.GetFloat("SpawnInterwal");
        InGasStationTime = PlayerPrefs.GetFloat("InGasStationTime");
    }

    public void OptionsSave()
    {
        OptionsDialog.SetActive(false);
        PlayerPrefs.SetFloat("SpawnInterwal", SpawnInterwal);
        PlayerPrefs.SetFloat("InGasStationTime", InGasStationTime);
    }

    public void GoToScene()
    {
        SceneManager.LoadScene("scene");
    }

    public void ExitBtn()
    {
        Application.Quit();
    }
}