﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlasModInstaller
{
    public class SkinRow
    {
        private readonly Skin skin;

        private readonly Panel outerPanel;
        private readonly Panel innerPanel;

        private readonly Label nameText;
        private readonly Label authorText;

        private readonly Button updateButton;
        private readonly Button previewIdleButton;
        private readonly Button previewChargedButton;
        private readonly Button installButton;

        private bool _downloading;

        // Main methods

        public async Task Install()
        {
            if (MainForm.BlasRootFolder == null) return;

            _downloading = true;
            using (WebClient client = new WebClient())
            {
                installButton.Text = "Downloading...";
                installButton.ForeColor = Colors.ORANGE;
                installButton.FlatAppearance.BorderColor = Colors.ORANGE;

                string downloadPath = $"{MainForm.DownloadsPath}{skin.id}";
                Directory.CreateDirectory(downloadPath);

                await client.DownloadFileTaskAsync(new Uri(skin.InfoURL), downloadPath + "\\info.txt");
                await client.DownloadFileTaskAsync(new Uri(skin.TextureURL), downloadPath + "\\texture.png");

                string installPath = skin.PathToSkinFolder;
                Directory.CreateDirectory(installPath);
                File.Copy(downloadPath + "\\info.txt", installPath + "\\info.txt");
                File.Copy(downloadPath + "\\texture.png", installPath + "\\texture.png");

                Directory.Delete(downloadPath, true);
            }
            _downloading = false;

            UpdateUI();
        }

        private void Uninstall()
        {
            if (MainForm.BlasRootFolder == null) return;

            if (Directory.Exists(skin.PathToSkinFolder))
                Directory.Delete(skin.PathToSkinFolder, true);

            UpdateUI();
        }

        // Click methods

        private void ClickedInstall(object sender, EventArgs e)
        {
            if (_downloading) return;

            if (skin.Installed)
            {
                if (MessageBox.Show("Are you sure you want to uninstall this skin?", nameText.Text, MessageBoxButtons.OKCancel) == DialogResult.OK)
                    Uninstall();
            }
            else
            {
                Install();
            }
        }

        private void ClickedPreviewIdle(object sender, EventArgs e)
        {
            try { Process.Start(skin.IdlePreviewURL); }
            catch (Exception) { MessageBox.Show("Link does not exist!", "Invalid Link"); }
        }

        private void ClickedPreviewCharged(object sender, EventArgs e)
        {
            try { Process.Start(skin.ChargedPreviewURL); }
            catch (Exception) { MessageBox.Show("Link does not exist!", "Invalid Link"); }
        }

        private void ClickedUpdate(object sender, EventArgs e)
        {
            Uninstall();
            Install();
        }

        // UI methods

        public void UpdateUI()
        {
            // Text
            nameText.Text = skin.name;
            nameText.Size = new Size(nameText.PreferredWidth, 30);
            authorText.Text = "by " + skin.author;
            authorText.Location = new Point(nameText.PreferredWidth + 15, authorText.Location.Y);

            // Install button
            bool installed = skin.Installed;
            installButton.Text = installed ? "Installed" : "Not installed";
            installButton.ForeColor = installed ? Colors.GREEN : Colors.RED;
            installButton.FlatAppearance.BorderColor = installed ? Colors.GREEN : Colors.RED;

            // Update button
            bool canUpdate = skin.UpdateAvailable;
            updateButton.Visible = canUpdate;
        }

        public SkinRow(Skin skin, Panel parentPanel, int skinIdx)
        {
            this.skin = skin;
            Color backgroundColor = skinIdx % 2 == 0 ? Colors.DARK_GRAY : Colors.LIGHT_GRAY;
            parentPanel.AutoScroll = false;

            // Panels

            outerPanel = new Panel
            {
                Name = skin.name,
                Parent = parentPanel,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Black,
                Location = new Point(0, (Sizes.SKIN_HEIGHT - 2) * skinIdx - 2),
                Size = new Size(parentPanel.Width, Sizes.SKIN_HEIGHT),
            };

            innerPanel = new Panel
            {
                Name = skin.name,
                Parent = outerPanel,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = backgroundColor,
                Location = new Point(0, 2),
                Size = new Size(parentPanel.Width, Sizes.SKIN_HEIGHT - 4),
            };

            // Left side

            nameText = new Label
            {
                Name = skin.name,
                Parent = innerPanel,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(10, 8),
                Size = new Size(100, 30),
                ForeColor = Color.LightGray,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = Fonts.SKIN_NAME,
            };

            authorText = new Label
            {
                Name = skin.name,
                Parent = innerPanel,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(200, 13),
                Size = new Size(200, 20),
                ForeColor = Color.LightGray,
                TextAlign = ContentAlignment.BottomLeft,
                Font = Fonts.SKIN_AUTHOR,
            };

            // Right side

            updateButton = new Button
            {
                Name = skin.name,
                Parent = innerPanel,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(parentPanel.Width - 550, 11),
                Size = new Size(130, 24),
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = Fonts.BUTTON,
                Text = "Download update",
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TabStop = false,
            };
            updateButton.FlatAppearance.BorderColor = Color.White;
            updateButton.Click += ClickedUpdate;
            updateButton.MouseUp += MainForm.Instance.RemoveButtonFocus;
            updateButton.MouseLeave += MainForm.Instance.RemoveButtonFocus;

            previewIdleButton = new Button
            {
                Name = skin.name,
                Parent = innerPanel,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(parentPanel.Width - 390, 11),
                Size = new Size(110, 24),
                BackColor = Colors.BLUE,
                Font = Fonts.BUTTON,
                Text = "Preview Idle",
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TabStop = false,
            };
            previewIdleButton.FlatAppearance.BorderColor = Color.Black;
            previewIdleButton.Click += ClickedPreviewIdle;
            previewIdleButton.MouseUp += MainForm.Instance.RemoveButtonFocus;
            previewIdleButton.MouseLeave += MainForm.Instance.RemoveButtonFocus;

            previewChargedButton = new Button
            {
                Name = skin.name,
                Parent = innerPanel,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(parentPanel.Width - 260, 11),
                Size = new Size(110, 24),
                BackColor = Colors.BLUE,
                Font = Fonts.BUTTON,
                Text = "Preview Charged",
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TabStop = false,
            };
            previewChargedButton.FlatAppearance.BorderColor = Color.Black;
            previewChargedButton.Click += ClickedPreviewCharged;
            previewChargedButton.MouseUp += MainForm.Instance.RemoveButtonFocus;
            previewChargedButton.MouseLeave += MainForm.Instance.RemoveButtonFocus;

            installButton = new Button
            {
                Name = skin.name,
                Parent = innerPanel,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(parentPanel.Width - 110, 11),
                Size = new Size(100, 24),
                BackColor = backgroundColor,
                Font = Fonts.BUTTON,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TabStop = false,
            };
            installButton.Click += ClickedInstall;
            installButton.MouseUp += MainForm.Instance.RemoveButtonFocus;
            installButton.MouseLeave += MainForm.Instance.RemoveButtonFocus;

            parentPanel.AutoScroll = true;
            MainForm.Instance.BlasSkinPage.AdjustPageWidth();
            UpdateUI();
        }
    }
}
