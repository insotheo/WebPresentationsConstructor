﻿using MessageBoxesWindows;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPC_Editor
{
    /// <summary>
    /// Логика взаимодействия для ColorpickerWindow.xaml
    /// </summary>
    public partial class ColorpickerWindow : Window
    {
        private int R, G, B;
        private string HEX;
        private Random rnd;

        public ColorpickerWindow()
        {
            InitializeComponent();
            rnd = new Random();
            R = rnd.Next(0, 255);
            G = rnd.Next(0, 255);
            B = rnd.Next(0, 255);
            HEX = RGBtoHEX();
            rTB.Text = R.ToString();
            gTB.Text = G.ToString();
            bTB.Text = B.ToString();
            hexTB.TextChanged += HexTB_TextChanged;
            hexTB.Text = HEX;
            setColor();
        }

        private void HexTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            hexTB.ToolTip = $"R: {((Color)((SolidColorBrush)previewBox.Fill).Color).R.ToString()}\n" +
                $"G: {((Color)((SolidColorBrush)previewBox.Fill).Color).G.ToString()}\n" +
                $"B: {((Color)((SolidColorBrush)previewBox.Fill).Color).B.ToString()}";
        }

        private string RGBtoHEX()
        {
            return "#" + R.ToString("X2") + G.ToString("X2") + B.ToString("X2");
        }

        private void setColor()
        {
            previewBox.Fill = new SolidColorBrush((Color)(ColorConverter.ConvertFromString(HEX)));
        }

        private void copyHEXtoClipboardBTN_Click(object sender, RoutedEventArgs e)
        {
            HEX = hexTB.Text.ToUpper().Trim();
            if (HEX != null)
            {
                if (HEX != String.Empty)
                {
                    Clipboard.SetText(HEX);
                    MessBox.showInfo("Скопировано в буфер обмена");
                }
            }
        }

        #region text changed events
        private void colorRGBtb_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                R = int.Parse(rTB.Text);
                G = int.Parse(gTB.Text);
                B = int.Parse(bTB.Text);
                if(R <= 255 && G <= 255 && B <= 255 && R >= 0 && G >= 0 && B >= 0)
                {
                    HEX = RGBtoHEX();
                    hexTB.Text = HEX;
                    setColor();
                }
            }
            catch { }
        }

        private void hexTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (hexTB.Text.StartsWith("#"))
                {
                    HEX = hexTB.Text.ToUpper().Trim();
                    setColor();
                }
            }
            catch { }
        }
        #endregion
    }
}
