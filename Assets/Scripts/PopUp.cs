using UnityEngine;

public class PopUp : MonoBehaviour
{

    [SerializeField] private GameObject popUp;

    public void ClickedOkayButton()
    {
        popUp.SetActive(false);
    }
}
