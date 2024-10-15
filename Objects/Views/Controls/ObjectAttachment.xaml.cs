using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Objects.Components;
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для ObjectAttachment.xaml
    /// </summary>
    public partial class ObjectAttachment : UserControl
    {
        public ObjectComment Comment;
        public ObjectAttachment(ObjectComment oc)
        {
            this.InitializeComponent();
            this.Comment = oc;
            this.VisibleName.Text = oc.Data;
            string ext = System.IO.Path.GetExtension(oc.Data);
            this.Extension.Content = ext;
            switch (ext)
            {
                case ".pdf":
                    this.ColorizeExtensionRectangle(255, 0, 21);
                    break;
                case ".docx":
                    this.ColorizeExtensionRectangle(0, 93, 230);
                    break;
                case ".xlsx":
                    this.ColorizeExtensionRectangle(0, 230, 93);
                    break;
                case ".png":
                    this.ColorizeExtensionRectangle(79, 165, 21);
                    break;
                case ".mp3":
                case ".wav":
                    this.ColorizeExtensionRectangle(253, 0, 155);
                    break;
            }
        }
        private void ColorizeExtensionRectangle(byte r, byte b, byte g)
        {
            this.ExtensionRectangle.Fill = new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        private void OpenClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                WebPreviewWindow wp = new(
                this.Comment.Data,
                ObjectProcessor.GetPathToAttachmentsFolder(this.Comment.Class, this.Comment.TargetObject) + this.Comment.Data, false);
                wp.Show();
            }
            catch (System.Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }

        private void RemoveClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                ObjectProcessor.RemoveObjectComment(new(this.Comment.Class), this.Comment);
                File.Delete(ObjectProcessor.GetPathToAttachmentsFolder(this.Comment.Class, this.Comment.TargetObject) + this.Comment.Data);
                this.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }
    }
}
