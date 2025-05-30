/*******************************************************************************
Copyright © 2015-2022 PICO Technology Co., Ltd.All rights reserved.  

NOTICE：All information contained herein is, and remains the property of 
PICO Technology Co., Ltd. The intellectual and technical concepts 
contained herein are proprietary to PICO Technology Co., Ltd. and may be 
covered by patents, patents in process, and are protected by trade secret or 
copyright law. Dissemination of this information or reproduction of this 
material is strictly forbidden unless prior written permission is obtained from
PICO Technology Co., Ltd. 
*******************************************************************************/
using UnityEditor;
using UnityEngine;

namespace Unity.XR.PXR.Editor
{
    public class PXR_EditorStyles
    {
        public Color colorLine = new Color32(0xD9, 0xD9, 0xD9, 255);
        public Color colorSelected = new Color32(0x7B, 0x7B, 0x7B, 255);
        public Color colorDocumentationUrlNormal = new Color32(0x0F, 0x6F, 0xD5, 255);
        public Color colorDocumentationUrlHover = new Color32(0x0F, 0x6F, 0xD5, 205);

        private GUIStyle _dialogIconStyle;
        private GUIStyle _headerText;
        private GUIStyle _versionText;
        private GUIStyle _contentArea;
        private GUIStyle _contentText;
        private GUIStyle _buttonStyle;
        private GUIStyle _buttonSelectedStyle;
        private GUIStyle _buttonToOpenStyle;
        private GUIStyle _backgroundColorStyle;
        private GUIStyle _bigWhiteTitleStyle;
        private GUIStyle _smallBlueLinkStyle;


        public GUIStyle HeaderText => _headerText ??= new GUIStyle(EditorStyles.largeLabel)
        {
            fontStyle = FontStyle.Normal,
            fontSize = 48,
            alignment = TextAnchor.MiddleCenter,
            fixedHeight = 69,
            fixedWidth = 600,
            normal = new GUIStyleState()
            {
                textColor = Color.white
            }
        };

        public GUIStyle BigWhiteTitleStyle => _bigWhiteTitleStyle ??= new GUIStyle(EditorStyles.largeLabel)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 20,
            fixedHeight = 25,
            alignment = TextAnchor.MiddleLeft,
            normal = new GUIStyleState()
            {
                textColor = Color.white
            }
        };

        public GUIStyle VersionText => _versionText ??= new GUIStyle(EditorStyles.miniLabel)
        {
            fontStyle = FontStyle.Normal,
            fontSize = 18,
            alignment = TextAnchor.LowerCenter,
            fixedHeight = 69,
            fixedWidth = 150,
            padding = new RectOffset(8, 0, 0, 8),
            normal = new GUIStyleState()
            {
                textColor = Color.white
            }
        };

        public GUIStyle ContentText => _contentText ??= new GUIStyle(EditorStyles.wordWrappedLabel)
        {
            richText = true,
            stretchHeight = true,
            fontSize = 16,
            normal = new GUIStyleState()
            {
                textColor = Color.white
            }
        };

        public GUIStyle SmallBlueLinkStyle => _smallBlueLinkStyle ??= new GUIStyle(EditorStyles.linkLabel)
        {
            fontStyle = FontStyle.Normal,
            fontSize = 16,
            fixedHeight = 25,
            alignment = TextAnchor.MiddleCenter,
            normal = new GUIStyleState()
            {
                textColor = colorDocumentationUrlNormal
            },
            hover = new GUIStyleState()
            {
                textColor = colorDocumentationUrlHover
            }
        };

        public GUIStyle IconStyle => _dialogIconStyle ??= new GUIStyle()
        {
            fixedHeight = 80,
            fixedWidth = 250,
            padding = new RectOffset(0, 10, 0, 0),
            alignment = TextAnchor.UpperRight,
        };

        public Texture2D MakeTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();
            return texture;
        }

        public GUIStyle ContentArea => _contentArea ??= new GUIStyle(EditorStyles.textArea)
        {
            stretchHeight = true,
            padding = new RectOffset(4, 4, 4, 4),
        };

        public GUIStyle Button => _buttonStyle ??= new GUIStyle(EditorStyles.miniButton)
        {
            stretchWidth = true,
            fixedWidth = 150,
            fixedHeight = 36,
            fontStyle = FontStyle.Bold,
            richText = true,
            padding = new RectOffset(4, 4, 4, 4),
            normal = new GUIStyleState() { background = MakeTexture(2, 2, colorLine) }
        };

        public GUIStyle ButtonSelected => _buttonSelectedStyle ??= new GUIStyle(EditorStyles.miniButton)
        {
            stretchWidth = true,
            fixedWidth = 150,
            fixedHeight = 36,
            fontStyle = FontStyle.Bold,
            richText = true,
            padding = new RectOffset(4, 4, 4, 4),
            normal = new GUIStyleState() { background = MakeTexture(2, 2, colorSelected) }
        };

        public GUIStyle ButtonToOpen => _buttonToOpenStyle ??= new GUIStyle(EditorStyles.miniButton)
        {
            stretchWidth = true,
            fixedWidth = 250,
            fixedHeight = 25,
            fontSize = 16,
            richText = true,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(4, 4, 4, 4),
            normal = new GUIStyleState()
            {
                textColor = Color.white,
                background = MakeTexture(2, 2, colorSelected)
            }
        };

        public GUIStyle BackgroundColor => _backgroundColorStyle ??= new GUIStyle(EditorStyles.wordWrappedLabel)
        {
        };

    }
}