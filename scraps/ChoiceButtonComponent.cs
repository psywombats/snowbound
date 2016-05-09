using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChoiceButtonComponent : UnityEngine.UI.Button {

    public Graphic selectedImage;
    public Graphic highlightedImage;

    private Graphic standardImage;

    protected override void Awake() {
        base.Awake();
        this.standardImage = this.targetGraphic;
    }
    
    public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData) {
        base.OnPointerDown(eventData);
        this.targetGraphic = this.selectedImage;
    }

    public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData) {
        base.OnPointerEnter(eventData);
        this.targetGraphic = this.highlightedImage;
    }

    public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData) {
        base.OnPointerExit(eventData);
        this.targetGraphic = this.standardImage;
    }

    public override void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData) {
        base.OnPointerUp(eventData);
        this.targetGraphic = this.standardImage;
    }
}
