        fullText = "";
        int accumulatedSize = 0;

        for (int i = 0; i < text.Length; i += 1) {
            fullText += text[i];
            CharacterInfo info;
            Canvas.ForceUpdateCanvases();
            textbox.font.GetCharacterInfo(text[i], out info);
            accumulatedSize += info.glyphWidth;

            if (accumulatedSize > textbox.rectTransform.rect.width) {
                // insert the newline at the last seen space then backtrack to that location
                int spaceindex = fullText.LastIndexOf(' ');
                if (spaceindex > 0) {
                    fullText = fullText.Substring(0, spaceindex) + '\n';
                } else {
                    // uuh apparently this word is bigger than the whole line
                    Assert.IsTrue(false, "line is too big: " + text);
                }
                accumulatedSize = 0;
                i = spaceindex + 1;
            }
        }