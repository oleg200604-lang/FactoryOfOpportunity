using UnityEngine;

public class GameUIScr : MonoBehaviour
{
    public GameObject buildPanel;

    public void ConstructionButton()
    {
        buildPanel.SetActive(!buildPanel.activeSelf);
    }
}
