using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] float scrollSpeed = 1f;

    GameInput inputActions;
    Vector2 scrollInput;

    void Awake()
    {
        inputActions = new GameInput();

        inputActions.UI.Scroll.performed += ctx =>
        {
            scrollInput = ctx.ReadValue<Vector2>();
        };
        inputActions.UI.Scroll.canceled += ctx =>
        {
            scrollInput = Vector2.zero;
        };
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition += scrollInput.y * scrollSpeed * Time.unscaledDeltaTime;

            // Clamp（0~1の範囲に制限）
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
        }
    }
}
