using System;
using System.Drawing;
using System.Windows.Forms;

namespace CheckersGame
{
    public partial class LoginForm : Form
    {
        private TextBox usernameTextBox;
        private TextBox passwordTextBox;

        public LoginForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Size = new Size(350, 250);
            this.Text = "Вход в аккаунт";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Очищаем форму
            this.Controls.Clear();

            // Логин
            Label usernameLabel = new Label();
            usernameLabel.Text = "Имя пользователя:";
            usernameLabel.Location = new Point(20, 30);
            usernameLabel.Size = new Size(120, 20);

            usernameTextBox = new TextBox();
            usernameTextBox.Location = new Point(20, 55);
            usernameTextBox.Size = new Size(280, 25);

            // Пароль
            Label passwordLabel = new Label();
            passwordLabel.Text = "Пароль:";
            passwordLabel.Location = new Point(20, 90);
            passwordLabel.Size = new Size(120, 20);

            passwordTextBox = new TextBox();
            passwordTextBox.Location = new Point(20, 115);
            passwordTextBox.Size = new Size(280, 25);
            passwordTextBox.PasswordChar = '*';

            // Кнопки
            Button loginButton = new Button();
            loginButton.Text = "Войти";
            loginButton.Location = new Point(20, 160);
            loginButton.Size = new Size(100, 30);
            loginButton.Click += LoginButton_Click;

            Button cancelButton = new Button();
            cancelButton.Text = "Отмена";
            cancelButton.Location = new Point(200, 160);
            cancelButton.Size = new Size(100, 30);
            cancelButton.Click += (s, ev) => this.DialogResult = DialogResult.Cancel;

            // Добавляем контролы
            this.Controls.AddRange(new Control[] {
                usernameLabel, usernameTextBox,
                passwordLabel, passwordTextBox,
                loginButton, cancelButton
            });
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameTextBox.Text) ||
                string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            // Здесь будет проверка в базе данных
            if (AuthenticateUser(usernameTextBox.Text, passwordTextBox.Text))
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль!");
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            int userID;
            string email;

            if (DatabaseHelper.AuthenticateUser(username, password, out userID, out email))
            {
                CurrentUser.UserID = userID;
                CurrentUser.Username = username;
                CurrentUser.Email = email;
                return true;
            }

            return false;
        }
    }
}