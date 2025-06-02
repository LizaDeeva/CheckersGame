using System;
using System.Drawing;
using System.Windows.Forms;

namespace CheckersGame
{
    public partial class RegisterForm : Form
    {
        private TextBox usernameTextBox;
        private TextBox passwordTextBox;
        private TextBox emailTextBox;

        public RegisterForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Size = new Size(350, 300);
            this.Text = "Регистрация";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Очищаем форму
            this.Controls.Clear();

            // Имя пользователя
            Label usernameLabel = new Label();
            usernameLabel.Text = "Имя пользователя:";
            usernameLabel.Location = new Point(20, 30);
            usernameLabel.Size = new Size(120, 20);

            usernameTextBox = new TextBox();
            usernameTextBox.Location = new Point(20, 55);
            usernameTextBox.Size = new Size(280, 25);

            // Email
            Label emailLabel = new Label();
            emailLabel.Text = "Email:";
            emailLabel.Location = new Point(20, 90);
            emailLabel.Size = new Size(120, 20);

            emailTextBox = new TextBox();
            emailTextBox.Location = new Point(20, 115);
            emailTextBox.Size = new Size(280, 25);

            // Пароль
            Label passwordLabel = new Label();
            passwordLabel.Text = "Пароль:";
            passwordLabel.Location = new Point(20, 150);
            passwordLabel.Size = new Size(120, 20);

            passwordTextBox = new TextBox();
            passwordTextBox.Location = new Point(20, 175);
            passwordTextBox.Size = new Size(280, 25);
            passwordTextBox.PasswordChar = '*';

            // Кнопки
            Button registerButton = new Button();
            registerButton.Text = "Зарегистрироваться";
            registerButton.Location = new Point(20, 220);
            registerButton.Size = new Size(130, 30);
            registerButton.Click += RegisterButton_Click;

            Button cancelButton = new Button();
            cancelButton.Text = "Отмена";
            cancelButton.Location = new Point(200, 220);
            cancelButton.Size = new Size(100, 30);
            cancelButton.Click += (s, ev) => this.DialogResult = DialogResult.Cancel;

            // Добавляем контролы
            this.Controls.AddRange(new Control[] {
                usernameLabel, usernameTextBox,
                emailLabel, emailTextBox,
                passwordLabel, passwordTextBox,
                registerButton, cancelButton
            });
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameTextBox.Text) ||
                string.IsNullOrWhiteSpace(passwordTextBox.Text) ||
                string.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            // Здесь будет регистрация в базе данных
            if (RegisterUser(usernameTextBox.Text, passwordTextBox.Text, emailTextBox.Text))
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Ошибка регистрации! Возможно, такой пользователь уже существует.");
            }
        }

        private bool RegisterUser(string username, string password, string email)
        {
            return DatabaseHelper.RegisterUser(username, password, email);
        }
    }
}