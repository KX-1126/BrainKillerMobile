using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class imageQuickSwitch : MonoBehaviour
{
    public TMP_Dropdown imageSelectorDropDown;
    // Start is called before the first frame update

    public void backToLastImage()
    {
        int currentIndex = imageSelectorDropDown.value;
        if (currentIndex > 0)
        {
            imageSelectorDropDown.value = currentIndex - 1;
            // Dropdown.value 赋值会自动触发 onValueChanged
        }
        // 如果已经是第一个，不做任何事
    }

    public void nextImage()
    {
        int currentIndex = imageSelectorDropDown.value;
        if (currentIndex < imageSelectorDropDown.options.Count - 1)
        {
            imageSelectorDropDown.value = currentIndex + 1;
            // Dropdown.value 赋值会自动触发 onValueChanged
        }
        // 如果已经是最后一个，不做任何事
    }

}
