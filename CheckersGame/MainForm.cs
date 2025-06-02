using System;
using System.Drawing;
using System.Windows.Forms;

namespace CheckersGame
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            // Настройки формы
            this.Size = new Size(400, 300);
            this.Text = "Шашки - Добро пожаловать";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Очищаем форму
            this.Controls.Clear();

            // Заголовок
            Label titleLabel = new Label();
            titleLabel.Text = "Добро пожаловать в игру Шашки!";
            titleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            titleLabel.Size = new Size(350, 30);
            titleLabel.Location = new Point(25, 30);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Кнопка "Войти в аккаунт"
            Button loginButton = new Button();
            loginButton.Text = "Войти в аккаунт";
            loginButton.Size = new Size(200, 40);
            loginButton.Location = new Point(100, 100);
            loginButton.Click += LoginButton_Click;

            // Кнопка "Зарегистрироваться" 
            Button registerButton = new Button();
            registerButton.Text = "Зарегистрироваться";
            registerButton.Size = new Size(200, 40);
            registerButton.Location = new Point(100, 160);
            registerButton.Click += RegisterButton_Click;

            // Добавляем контролы
            this.Controls.Add(titleLabel);
            this.Controls.Add(loginButton);
            this.Controls.Add(registerButton);
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // Если вход успешен, открываем главное меню
                this.Hide();
                MainMenuForm mainMenu = new MainMenuForm();
                mainMenu.ShowDialog();
                this.Close();
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            if (registerForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Регистрация успешна! Теперь войдите в аккаунт.", "Успех");
            }
        }
    }
}