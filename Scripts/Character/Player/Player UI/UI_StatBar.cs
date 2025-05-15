using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : MonoBehaviour
{
    private Slider slider;
    private RectTransform rectTransform;
    // Variable to scale bar size depending on stat (Higher the stat the longer the bar grows)
    [Header("Bar Options")]
    [SerializeField] protected bool scaleBarLengthWithStats = true;
    [SerializeField] protected float widthScaleMultiplier = 1;
    //Secondary bar behind my bar for polish effect (Yellow bar that shows how much an action/damage takes away from the currect stat)


    protected virtual void Awake()
    {
        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void SetStat(int newValue)
    {
        slider.value = newValue;
    }

    public virtual void SetMaxStat(int maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;

        if (scaleBarLengthWithStats )
        {
            //Scale the transform of this object
            rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);

            //Resets the position of the bars based on their layout group settings
            PlayerUIManager.instance.playerUIHudManager.RefreshHUD();
        }
    }
}
