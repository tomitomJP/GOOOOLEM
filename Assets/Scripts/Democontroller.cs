using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class Democontroller : MonoBehaviour
{
    [SerializeField] string titleSceneName = "Title";



    [SerializeField] string[] targetTags;

    List<GameObject> disabled = new List<GameObject>();

    [SerializeField] VideoPlayer videoPlayer;

    void Awake()
    {
        if (videoPlayer != null)
        {
            videoPlayer.isLooping = false;
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }



    void Update()
    {
        if (HasAnyInputThisFrame())
        {
            LoadSceneManager.FadeLoadScene(titleSceneName);
        }
    }

    bool HasAnyInputThisFrame()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) return true;

        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame) return true;
            if (Mouse.current.rightButton.wasPressedThisFrame) return true;
            if (Mouse.current.middleButton.wasPressedThisFrame) return true;
            if (Mouse.current.scroll.ReadValue() != Vector2.zero) return true;
        }

        if (Gamepad.current != null)
        {
            var gp = Gamepad.current;
            if (gp.leftStick.ReadValue() != Vector2.zero) return true;
            if (gp.rightStick.ReadValue() != Vector2.zero) return true;
            if (gp.leftTrigger.ReadValue() > 0f) return true;
            if (gp.rightTrigger.ReadValue() > 0f) return true;
            if (gp.buttonSouth.wasPressedThisFrame) return true;
            if (gp.startButton.wasPressedThisFrame) return true;
            if (gp.dpad.up.wasPressedThisFrame) return true;
            if (gp.dpad.down.wasPressedThisFrame) return true;
            if (gp.dpad.left.wasPressedThisFrame) return true;
            if (gp.dpad.right.wasPressedThisFrame) return true;
        }
        if (Input.anyKeyDown) return true;

        return false;
    }

    void OnEnable()
    {
        foreach (var tag in targetTags)
        {
            foreach (var go in GameObject.FindGameObjectsWithTag(tag))
            {
                if (go.activeSelf)
                {
                    go.SetActive(false);
                    disabled.Add(go);
                }
            }
        }
    }

    void OnDisable()
    {
        // シーンを抜ける時に復活
        foreach (var go in disabled)
        {
            if (go != null) go.SetActive(true);
        }
        disabled.Clear();
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        LoadSceneManager.FadeLoadScene(titleSceneName);
    }
}
