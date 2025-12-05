using UnityEngine;

public class InputListener : MonoBehaviour
{
    void Update()
    {
        if (GameManager.Instance == null) return;
        if (Input.GetKeyDown(KeyBindings.GetLazada())) GameManager.Instance.PlayerPressedKey(KeyBindings.GetLazada());
        if (Input.GetKeyDown(KeyBindings.GetShopee())) GameManager.Instance.PlayerPressedKey(KeyBindings.GetShopee());
    }
}
