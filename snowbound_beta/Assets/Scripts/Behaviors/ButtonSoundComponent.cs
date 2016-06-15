﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class ButtonSoundComponent : MonoBehaviour {

    private const string DefaultHoverSoundTag = null;
    private const string DefaultClickSoundTag = "click";

    public string ClickSoundTag = "";
    public string HoverSoundTag = "";

    private SoundPlayer player;

    public void Awake() {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => {
            HandleButtonEvent(ClickSoundTag.Length > 0 ? ClickSoundTag : DefaultClickSoundTag);
        });
        //button.on.AddListener(() => {
        //    HandleButtonEvent(ClickSoundTag.Length > 0 ? ClickSoundTag : DefaultClickSoundTag);
        //});

        player = FindObjectOfType<SoundPlayer>();
    }

    private void HandleButtonEvent(string associatedTag) {
        player.PlaySound(associatedTag);
    }
}
