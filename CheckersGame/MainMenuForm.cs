using System;
using System.Drawing;
using System.Windows.Forms;

namespace CheckersGame
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Size = new Size(400, 400);
            this.Text = "Шашки - Главное меню";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Очищаем форму
            this.Controls.Clear();

            // Заголовок
            Label titleLabel = new Label();
            titleLabel.Text = "Главное меню";
            titleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            titleLabel.Size = new Size(350, 30);
            titleLabel.Location = new Point(25, 20);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Кнопка "Играть на простом уровне"
            Button easyGameButton = new Button();
            easyGameButton.Text = "Играть на простом уровне";
            easyGameButton.Size = new Size(250, 40);
            easyGameButton.Location = new Point(75, 80);
            easyGameButton.Click += (s, ev) => StartGame(1);

            // Кнопка "Играть на сложном уровне"
            Button hardGameButton = new Button();
            hardGameButton.Text = "Играть на сложном уровне";
            hardGameButton.Size = new Size(250, 40);
            hardGameButton.Location = new Point(75, 140);
            hardGameButton.Click += (s, ev) => StartGame(2);

            // Кнопка "Моя статистика"
            Button statsButton = new Button();
            statsButton.Text = "Моя статистика";
            statsButton.Size = new Size(250, 40);
            statsButton.Location = new Point(75, 200);
            statsButton.Click += StatsButton_Click;

            // Кнопка "Лучший игрок"
            Button topPlayerButton = new Button();
            topPlayerButton.Text = "Лучший игрок";
            topPlayerButton.Size = new Size(250, 40);
            topPlayerButton.Location = new Point(75, 260);
            topPlayerButton.Click += TopPlayerButton_Click;

            // Кнопка "Выход"
            Button exitButton = new Button();
            exitButton.Text = "Выход";
            exitButton.Size = new Size(250, 40);
            exitButton.Location = new Point(75, 320);
            exitButton.Click += (s, ev) => this.Close();

            // Добавляем контролы
            this.Controls.AddRange(new Control[] {
                titleLabel, easyGameButton, hardGameButton,
                statsButton, topPlayerButton, exitButton
            });
        }

        private void StartGame(int difficulty)
        {
            CheckersGameForm gameForm = new CheckersGameForm(difficulty);
            gameForm.ShowDialog();
        }

        private void StatsButton_Click(object sender, EventArgs e)
        {
            StatsForm statsForm = new StatsForm();
            statsForm.ShowDialog();
        }

        private void TopPlayerButton_Click(object sender, EventArgs e)
        {
            TopPlayerForm topPlayerForm = new TopPlayerForm();
            topPlayerForm.ShowDialog();
        }
    }
}