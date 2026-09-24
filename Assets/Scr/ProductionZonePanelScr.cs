using UnityEngine;
using UnityEngine.UI;

public class ProductionZonePanelScr : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public Transform recipeContainer;
    public GameObject recipeButtonPrefab;

    private ZoneGroup currentGroup;

    public void Open(ZoneGroup group)
    {
        if (group == null)
            return;

        if (!(group.building is Workshop))
            return;

        currentGroup = group;

        panel.SetActive(true);

        RefreshRecipes();
    }

    public void Close()
    {
        currentGroup = null;
        panel.SetActive(false);
    }

    private void RefreshRecipes()
    {
        if (recipeContainer == null)
        {
            Debug.LogError("ProductionZonePanel: recipeContainer не встановлений.");
            return;
        }

        if (recipeButtonPrefab == null)
        {
            Debug.LogError("ProductionZonePanel: recipeButtonPrefab не встановлений.");
            return;
        }

        // Очистити старі кнопки
        for (int i = recipeContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(recipeContainer.GetChild(i).gameObject);
        }

        if (GameManagerScr.Instance == null)
            return;

        for (int i = 0; i < GameManagerScr.Instance.schemes.Count; i++)
        {
            int index = i;

            Scheme scheme = GameManagerScr.Instance.schemes[index];

            GameObject buttonObject =
                Instantiate(recipeButtonPrefab, recipeContainer);

            Button button =
                buttonObject.GetComponent<Button>();

            if (button == null)
            {
                Debug.LogWarning(
                    "RecipeButtonPrefab не має компонента Button."
                );

                continue;
            }

            Text text =
                buttonObject.GetComponentInChildren<Text>();

            if (text != null)
            {
                text.text = GetRecipeName(scheme);
            }

            button.onClick.AddListener(
                () => SelectRecipe(index)
            );
        }
    }

    private string GetRecipeName(Scheme scheme)
    {
        if (scheme == null)
            return "Unknown recipe";

        if (scheme.outputs != null &&
            scheme.outputs.Length > 0 &&
            scheme.outputs[0] != null)
        {
            return scheme.outputs[0].name;
        }

        return "Unnamed recipe";
    }

    private void SelectRecipe(int index)
    {
        if (currentGroup == null)
            return;

        if (!(currentGroup.building is Workshop workshop))
            return;

        workshop.SetScheme(index);

        Close();
    }
}