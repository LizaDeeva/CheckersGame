using CheckersGame.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CheckersGame
{
    public partial class TopPlayerForm : Form
    {
        public TopPlayerForm()
        {
            SetupForm();
            LoadTopPlayer();
        }

        private void SetupForm()
        {
            this.Size = new Size(400, 250);
            this.Text = "Лучший игрок";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Заголовок
            Label titleLabel = new Label();
            titleLabel.Text = "Лучший игрок по проценту побед";
            titleLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            titleLabel.Size = new Size(350, 30);
            titleLabel.Location = new Point(25, 20);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Информация о лучшем игроке
            Label playerInfoLabel = new Label();
            playerInfoLabel.Name = "PlayerInfoLabel";
            playerInfoLabel.Text = "Загрузка...";
            playerInfoLabel.Size = new Size(350, 100);
            playerInfoLabel.Location = new Point(25, 70);
            playerInfoLabel.Font = new Font("Arial", 12);
            playerInfoLabel.TextAlign = ContentAlignment.TopCenter;

            // Кнопка закрытия
            Button closeButton = new Button();
            closeButton.Text = "Закрыть";
            closeButton.Size = new Size(100, 30);
            closeButton.Location = new Point(150, 180);
            closeButton.Click += (s, e) => this.Close();

            // Добавляем контролы
            this.Controls.AddRange(new Control[] {
                titleLabel, playerInfoLabel, closeButton
            });
        }

        private void LoadTopPlayer()
        {
            UserStats topPlayer = DatabaseHelper.GetTopPlayer();
            if (topPlayer != null)
            {
                string info = $"Игрок: {topPlayer.Username}\n" +
                             $"Всего игр: {topPlayer.TotalGames}\n" +
                             $"Выиграно игр: {topPlayer.WonGames}\n" +
                             $"Процент побед: {topPlayer.WinRate:F1}%";
                ((Label)this.Controls["PlayerInfoLabel"]).Text = info;
            }
            else
            {
                ((Label)this.Controls["PlayerInfoLabel"]).Text = "Нет данных о играх";
            }
        }
    }
}