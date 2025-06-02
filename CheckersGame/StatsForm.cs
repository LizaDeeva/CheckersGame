using System;
using CheckersGame.Models;
using System.Drawing;
using System.Windows.Forms;

namespace CheckersGame
{
    public partial class StatsForm : Form
    {
        public StatsForm()
        {
            InitializeComponent();
            SetupForm();
            LoadStats();
        }

        private void SetupForm()
        {
            this.Size = new Size(400, 300);
            this.Text = "Моя статистика";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Заголовок
            Label titleLabel = new Label();
            titleLabel.Text = $"Статистика игрока: {CurrentUser.Username}";
            titleLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            titleLabel.Size = new Size(350, 30);
            titleLabel.Location = new Point(25, 20);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Метки для статистики
            Label totalGamesLabel = new Label();
            totalGamesLabel.Name = "TotalGamesLabel";
            totalGamesLabel.Text = "Всего игр: 0";
            totalGamesLabel.Size = new Size(300, 25);
            totalGamesLabel.Location = new Point(50, 70);
            totalGamesLabel.Font = new Font("Arial", 12);

            Label wonGamesLabel = new Label();
            wonGamesLabel.Name = "WonGamesLabel";
            wonGamesLabel.Text = "Выиграно: 0";
            wonGamesLabel.Size = new Size(300, 25);
            wonGamesLabel.Location = new Point(50, 100);
            wonGamesLabel.Font = new Font("Arial", 12);

            Label lostGamesLabel = new Label();
            lostGamesLabel.Name = "LostGamesLabel";
            lostGamesLabel.Text = "Проиграно: 0";
            lostGamesLabel.Size = new Size(300, 25);
            lostGamesLabel.Location = new Point(50, 130);
            lostGamesLabel.Font = new Font("Arial", 12);

            Label winRateLabel = new Label();
            winRateLabel.Name = "WinRateLabel";
            winRateLabel.Text = "Процент побед: 0%";
            winRateLabel.Size = new Size(300, 25);
            winRateLabel.Location = new Point(50, 160);
            winRateLabel.Font = new Font("Arial", 12);

            // Кнопка закрытия
            Button closeButton = new Button();
            closeButton.Text = "Закрыть";
            closeButton.Size = new Size(100, 30);
            closeButton.Location = new Point(150, 220);
            closeButton.Click += (s, e) => this.Close();

            // Добавляем контролы
            this.Controls.AddRange(new Control[] {
                titleLabel, totalGamesLabel, wonGamesLabel,
                lostGamesLabel, winRateLabel, closeButton
            });
        }

        private void LoadStats()
        {
            UserStats stats = DatabaseHelper.GetUserStats(CurrentUser.UserID);

            if (stats != null)
            {
                ((Label)this.Controls["TotalGamesLabel"]).Text = $"Всего игр: {stats.TotalGames}";
                ((Label)this.Controls["WonGamesLabel"]).Text = $"Выиграно: {stats.WonGames}";
                ((Label)this.Controls["LostGamesLabel"]).Text = $"Проиграно: {stats.LostGames}";
                ((Label)this.Controls["WinRateLabel"]).Text = $"Процент побед: {stats.WinRate:F1}%";
            }
        }
    }
}