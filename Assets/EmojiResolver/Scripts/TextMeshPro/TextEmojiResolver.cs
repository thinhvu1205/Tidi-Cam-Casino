using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextEmojiResolver : MonoBehaviour
{

    [SerializeField]
    private EmojiResolver emojiResolver;
    private TMP_Text textComponent;
    private string currentText = "";

    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponent<TMP_Text>();

        emojiResolver.ChangeEmojiUnicodeToTag(textComponent);
        currentText = textComponent.text;
    }

    // Update is called once per frame
    void Update()
    {
        if (textComponent!=null&&currentText != textComponent.text)
        {
            emojiResolver.ChangeEmojiUnicodeToTag(textComponent);
            currentText = textComponent.text;
        }
    }
}
