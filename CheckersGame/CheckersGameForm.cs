using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CheckersGame
{
    public partial class CheckersGameForm : Form
    {
        private const int BOARD_SIZE = 8;
        private const int CELL_SIZE = 80;
        private Button[,] boardButtons;
        private CheckersPiece[,] board;
        private bool isPlayerTurn = true;
        private Point selectedPiece = new Point(-1, -1);
        private int difficulty; // 1 = легкий, 2 = сложный
        private Random random = new Random();
        private bool mustCapture = false;
        private List<Point> capturingPieces = new List<Point>();
        private bool isMultiCapture = false; // Флаг для множественного взятия

        public CheckersGameForm(int gameDifficulty)
        {
            InitializeComponent();
            difficulty = gameDifficulty;
            SetupForm();
            InitializeBoard();
        }

        private void SetupForm()
        {
            this.Size = new Size(BOARD_SIZE * CELL_SIZE + 200, BOARD_SIZE * CELL_SIZE + 100);
            this.Text = $"Шашки - {(difficulty == 1 ? "Простой" : "Сложный")} уровень";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Создаем доску из кнопок
            boardButtons = new Button[BOARD_SIZE, BOARD_SIZE];
            board = new CheckersPiece[BOARD_SIZE, BOARD_SIZE];

            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(CELL_SIZE, CELL_SIZE);
                    btn.Location = new Point(col * CELL_SIZE + 10, row * CELL_SIZE + 10);
                    btn.Tag = new Point(row, col);
                    btn.Click += CellButton_Click;
                    btn.Font = new Font("Arial", 36, FontStyle.Bold);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 1;

                    // Цвет клеток доски
                    if ((row + col) % 2 == 0)
                        btn.BackColor = Color.FromArgb(240, 217, 181); // Светлые клетки
                    else
                        btn.BackColor = Color.FromArgb(181, 136, 99); // Темные клетки

                    boardButtons[row, col] = btn;
                    this.Controls.Add(btn);
                }
            }

            // Кнопка "Сдаться"
            Button surrenderButton = new Button();
            surrenderButton.Text = "Сдаться";
            surrenderButton.Size = new Size(100, 30);
            surrenderButton.Location = new Point(BOARD_SIZE * CELL_SIZE + 20, 50);
            surrenderButton.Click += (s, e) => EndGame("Loss");
            this.Controls.Add(surrenderButton);

            // Метка статуса
            Label statusLabel = new Label();
            statusLabel.Text = "Ваш ход";
            statusLabel.Size = new Size(150, 50);
            statusLabel.Location = new Point(BOARD_SIZE * CELL_SIZE + 20, 20);
            statusLabel.Name = "StatusLabel";
            this.Controls.Add(statusLabel);

            // Метка уровня сложности
            Label difficultyLabel = new Label();
            difficultyLabel.Text = $"Уровень: {(difficulty == 1 ? "Простой" : "Сложный")}";
            difficultyLabel.Size = new Size(150, 30);
            difficultyLabel.Location = new Point(BOARD_SIZE * CELL_SIZE + 20, 90);
            this.Controls.Add(difficultyLabel);
        }

        private void InitializeBoard()
        {
            // Расставляем шашки в начальную позицию
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    if ((row + col) % 2 == 1) // Только на темных клетках
                    {
                        board[row, col] = CheckersPiece.Computer;
                        boardButtons[row, col].Text = "●";
                        boardButtons[row, col].ForeColor = Color.DarkRed;
                    }
                }
            }

            for (int row = 5; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    if ((row + col) % 2 == 1) // Только на темных клетках
                    {
                        board[row, col] = CheckersPiece.Player;
                        boardButtons[row, col].Text = "●";
                        boardButtons[row, col].ForeColor = Color.Black;
                    }
                }
            }
        }

        private void CellButton_Click(object sender, EventArgs e)
        {
            if (!isPlayerTurn) return;

            Button clickedButton = (Button)sender;
            Point position = (Point)clickedButton.Tag;

            // Если идет множественное взятие, можно ходить только выбранной шашкой
            if (isMultiCapture)
            {
                if (selectedPiece.X != -1 && IsValidMove(selectedPiece, position, true))
                {
                    bool wasCapture = IsCapturingMove(selectedPiece, position);
                    if (wasCapture)
                    {
                        MakeMove(selectedPiece, position);
                        CheckPromotion(position, true);
                        ClearSelection();

                        // Проверяем возможность продолжения взятия
                        if (CanCapture(position, true))
                        {
                            selectedPiece = position;
                            boardButtons[position.X, position.Y].BackColor = Color.Yellow;
                            UpdateStatus("Продолжите взятие");
                            return;
                        }
                        else
                        {
                            isMultiCapture = false;
                            EndPlayerTurn();
                        }
                    }
                }
                else
                {
                    // Неверный ход во время множественного взятия
                    return;
                }
                return;
            }

            // Проверяем обязательное взятие
            List<Point> playerCaptures = GetCapturingPieces(true);
            if (playerCaptures.Count > 0)
            {
                mustCapture = true;
                capturingPieces = playerCaptures;
            }
            else
            {
                mustCapture = false;
                capturingPieces.Clear();
            }

            if (selectedPiece.X == -1) // Выбираем шашку
            {
                if (IsPlayerPiece(board[position.X, position.Y]))
                {
                    // Если есть обязательное взятие, можно выбрать только шашки, которые могут брать
                    if (mustCapture && !capturingPieces.Contains(position))
                    {
                        UpdateStatus("Вы должны съесть шашку!");
                        return;
                    }

                    selectedPiece = position;
                    clickedButton.BackColor = Color.Yellow; // Подсвечиваем выбранную шашку
                }
            }
            else // Делаем ход
            {
                if (IsValidMove(selectedPiece, position, true))
                {
                    bool wasCapture = IsCapturingMove(selectedPiece, position);
                    MakeMove(selectedPiece, position);
                    CheckPromotion(position, true);
                    ClearSelection();

                    // Проверяем множественное взятие
                    if (wasCapture && CanCapture(position, true))
                    {
                        isMultiCapture = true;
                        selectedPiece = position;
                        boardButtons[position.X, position.Y].BackColor = Color.Yellow;
                        UpdateStatus("Продолжите взятие");
                        return;
                    }

                    EndPlayerTurn();
                }
                else
                {
                    ClearSelection();
                }
            }
        }

        private void CheckPromotion(Point position, bool isPlayer)
        {
            if (isPlayer && board[position.X, position.Y] == CheckersPiece.Player && position.X == 0)
            {
                board[position.X, position.Y] = CheckersPiece.PlayerKing;
                boardButtons[position.X, position.Y].Text = "♛"; // Используем корону для дамки
                boardButtons[position.X, position.Y].ForeColor = Color.DarkBlue;
            }
            else if (!isPlayer && board[position.X, position.Y] == CheckersPiece.Computer && position.X == 7)
            {
                board[position.X, position.Y] = CheckersPiece.ComputerKing;
                boardButtons[position.X, position.Y].Text = "♛";
                boardButtons[position.X, position.Y].ForeColor = Color.DarkMagenta;
            }
        }

        private void EndPlayerTurn()
        {
            if (!CheckGameEnd())
            {
                isPlayerTurn = false;
                UpdateStatus("Ход компьютера...");

                // Компьютер делает ход через небольшую задержку
                Timer computerMoveTimer = new Timer();
                computerMoveTimer.Interval = difficulty == 1 ? 1500 : 1000;
                computerMoveTimer.Tick += (s, ev) =>
                {
                    computerMoveTimer.Stop();
                    MakeComputerMove();
                    if (!CheckGameEnd())
                    {
                        isPlayerTurn = true;
                        UpdateStatus("Ваш ход");
                    }
                };
                computerMoveTimer.Start();
            }
        }

        private bool IsValidMove(Point from, Point to, bool isPlayer)
        {
            if (to.X < 0 || to.X >= BOARD_SIZE || to.Y < 0 || to.Y >= BOARD_SIZE) return false;
            if (board[to.X, to.Y] != CheckersPiece.Empty) return false;
            if ((to.X + to.Y) % 2 == 0) return false; // Только темные клетки

            CheckersPiece piece = board[from.X, from.Y];
            bool isKing = (piece == CheckersPiece.PlayerKing || piece == CheckersPiece.ComputerKing);

            int rowDiff = to.X - from.X;
            int colDiff = to.Y - from.Y;

            // Проверяем обязательное взятие
            if (mustCapture && isPlayer && !isMultiCapture)
            {
                return IsValidCapture(from, to);
            }

            // Во время множественного взятия можно делать только взятия
            if (isMultiCapture && isPlayer)
            {
                return IsValidCapture(from, to);
            }

            // Для дамок - проверяем диагональное движение на любое расстояние
            if (isKing)
            {
                if (Math.Abs(rowDiff) != Math.Abs(colDiff)) return false; // Должно быть диагональное движение

                // Проверяем, что путь свободен
                int rowStep = rowDiff > 0 ? 1 : -1;
                int colStep = colDiff > 0 ? 1 : -1;

                int currentRow = from.X + rowStep;
                int currentCol = from.Y + colStep;
                List<Point> capturedPieces = new List<Point>();

                while (currentRow != to.X && currentCol != to.Y)
                {
                    if (board[currentRow, currentCol] != CheckersPiece.Empty)
                    {
                        // Если встретили фигуру
                        if (isPlayer && IsComputerPiece(board[currentRow, currentCol]))
                        {
                            capturedPieces.Add(new Point(currentRow, currentCol));
                        }
                        else if (!isPlayer && IsPlayerPiece(board[currentRow, currentCol]))
                        {
                            capturedPieces.Add(new Point(currentRow, currentCol));
                        }
                        else
                        {
                            return false; // Встретили свою фигуру или уже взятую
                        }
                    }
                    currentRow += rowStep;
                    currentCol += colStep;
                }

                // Если есть взятия, то должно быть обязательное взятие
                if (capturedPieces.Count > 0)
                {
                    return true; // Взятие дамкой
                }
                else
                {
                    return !mustCapture; // Обычный ход дамки возможен только если нет обязательных взятий
                }
            }

            // Обычный ход для простых шашек
            if (Math.Abs(rowDiff) == 1 && Math.Abs(colDiff) == 1)
            {
                if (isPlayer)
                    return rowDiff < 0; // Игрок ходит вверх
                else
                    return rowDiff > 0; // Компьютер ходит вниз
            }

            // Ход со взятием для простых шашек
            if (Math.Abs(rowDiff) == 2 && Math.Abs(colDiff) == 2)
            {
                return IsValidCapture(from, to);
            }

            return false;
        }

        private bool IsValidCapture(Point from, Point to)
        {
            CheckersPiece piece = board[from.X, from.Y];
            bool isKing = (piece == CheckersPiece.PlayerKing || piece == CheckersPiece.ComputerKing);
            bool isPlayer = IsPlayerPiece(piece);

            int rowDiff = to.X - from.X;
            int colDiff = to.Y - from.Y;

            // Для дамок - проверяем взятие на любом расстоянии
            if (isKing)
            {
                if (Math.Abs(rowDiff) != Math.Abs(colDiff)) return false;

                int rowStep = rowDiff > 0 ? 1 : -1;
                int colStep = colDiff > 0 ? 1 : -1;

                int currentRow = from.X + rowStep;
                int currentCol = from.Y + colStep;
                List<Point> enemiesToCapture = new List<Point>();

                while (currentRow != to.X && currentCol != to.Y)
                {
                    if (board[currentRow, currentCol] != CheckersPiece.Empty)
                    {
                        if (isPlayer && IsComputerPiece(board[currentRow, currentCol]))
                        {
                            enemiesToCapture.Add(new Point(currentRow, currentCol));
                        }
                        else if (!isPlayer && IsPlayerPiece(board[currentRow, currentCol]))
                        {
                            enemiesToCapture.Add(new Point(currentRow, currentCol));
                        }
                        else
                        {
                            return false; // Встретили свою фигуру
                        }
                    }
                    currentRow += rowStep;
                    currentCol += colStep;
                }

                return enemiesToCapture.Count > 0; // Должен быть хотя бы один враг для взятия
            }
            else
            {
                // Для обычных шашек - только взятие через одну клетку
                if (Math.Abs(rowDiff) != 2 || Math.Abs(colDiff) != 2) return false;

                int middleRow = (from.X + to.X) / 2;
                int middleCol = (from.Y + to.Y) / 2;
                CheckersPiece middlePiece = board[middleRow, middleCol];

                if (isPlayer)
                {
                    return IsComputerPiece(middlePiece);
                }
                else
                {
                    return IsPlayerPiece(middlePiece);
                }
            }
        }

        private bool IsCapturingMove(Point from, Point to)
        {
            CheckersPiece piece = board[from.X, from.Y];
            bool isKing = (piece == CheckersPiece.PlayerKing || piece == CheckersPiece.ComputerKing);

            if (isKing)
            {
                int rowDiff = to.X - from.X;
                int colDiff = to.Y - from.Y;
                int rowStep = rowDiff > 0 ? 1 : -1;
                int colStep = colDiff > 0 ? 1 : -1;

                int currentRow = from.X + rowStep;
                int currentCol = from.Y + colStep;

                while (currentRow != to.X && currentCol != to.Y)
                {
                    if (board[currentRow, currentCol] != CheckersPiece.Empty)
                    {
                        return true; // Есть фигура для взятия
                    }
                    currentRow += rowStep;
                    currentCol += colStep;
                }
                return false;
            }
            else
            {
                return Math.Abs(to.X - from.X) == 2;
            }
        }

        private void MakeMove(Point from, Point to)
        {
            CheckersPiece piece = board[from.X, from.Y];
            bool isKing = (piece == CheckersPiece.PlayerKing || piece == CheckersPiece.ComputerKing);

            // Перемещаем шашку
            board[to.X, to.Y] = piece;
            board[from.X, from.Y] = CheckersPiece.Empty;

            boardButtons[to.X, to.Y].Text = boardButtons[from.X, from.Y].Text;
            boardButtons[to.X, to.Y].ForeColor = boardButtons[from.X, from.Y].ForeColor;
            boardButtons[from.X, from.Y].Text = "";

            // Убираем взятые фигуры
            if (isKing)
            {
                // Для дамок убираем все фигуры на пути
                int rowDiff = to.X - from.X;
                int colDiff = to.Y - from.Y;
                int rowStep = rowDiff > 0 ? 1 : -1;
                int colStep = colDiff > 0 ? 1 : -1;

                int currentRow = from.X + rowStep;
                int currentCol = from.Y + colStep;

                while (currentRow != to.X && currentCol != to.Y)
                {
                    if (board[currentRow, currentCol] != CheckersPiece.Empty)
                    {
                        board[currentRow, currentCol] = CheckersPiece.Empty;
                        boardButtons[currentRow, currentCol].Text = "";
                    }
                    currentRow += rowStep;
                    currentCol += colStep;
                }
            }
            else
            {
                // Для обычных шашек убираем одну фигуру посередине
                if (Math.Abs(to.X - from.X) == 2)
                {
                    int middleRow = (from.X + to.X) / 2;
                    int middleCol = (from.Y + to.Y) / 2;
                    board[middleRow, middleCol] = CheckersPiece.Empty;
                    boardButtons[middleRow, middleCol].Text = "";
                }
            }
        }

        private void MakeComputerMove()
        {
            // Проверяем обязательные взятия
            List<Point> computerCaptures = GetCapturingPieces(false);

            if (computerCaptures.Count > 0)
            {
                // Делаем взятие с возможностью множественного взятия
                MakeComputerCaptureSequence(computerCaptures);
            }
            else
            {
                // Обычный ход
                MakeComputerRegularMove();
            }
        }

        private void MakeComputerCaptureSequence(List<Point> capturingPieces)
        {
            Point bestFrom = capturingPieces[0];

            // Для сложного уровня выбираем лучший ход
            if (difficulty == 2)
            {
                bestFrom = GetBestCaptureMove(capturingPieces);
            }
            else
            {
                bestFrom = capturingPieces[random.Next(capturingPieces.Count)];
            }

            // Выполняем последовательность взятий
            MakeComputerCaptureFromPosition(bestFrom);
        }

        private void MakeComputerCaptureFromPosition(Point from)
        {
            List<Point> possibleCaptures = GetPossibleCaptures(from, false);
            if (possibleCaptures.Count > 0)
            {
                Point to;
                if (difficulty == 2)
                {
                    // Выбираем ход с максимальным количеством взятий
                    to = GetBestCaptureTarget(from, possibleCaptures);
                }
                else
                {
                    to = possibleCaptures[random.Next(possibleCaptures.Count)];
                }

                MakeMove(from, to);
                CheckPromotion(to, false);

                // Проверяем возможность продолжения взятия
                if (CanCapture(to, false))
                {
                    MakeComputerCaptureFromPosition(to);
                }
            }
        }

        private Point GetBestCaptureTarget(Point from, List<Point> possibleCaptures)
        {
            Point bestTarget = possibleCaptures[0];
            int maxCapturedPieces = 0;

            foreach (Point target in possibleCaptures)
            {
                int capturedCount = CountCapturedPieces(from, target);
                if (capturedCount > maxCapturedPieces)
                {
                    maxCapturedPieces = capturedCount;
                    bestTarget = target;
                }
            }

            return bestTarget;
        }

        private int CountCapturedPieces(Point from, Point to)
        {
            CheckersPiece piece = board[from.X, from.Y];
            bool isKing = (piece == CheckersPiece.PlayerKing || piece == CheckersPiece.ComputerKing);

            if (isKing)
            {
                int count = 0;
                int rowDiff = to.X - from.X;
                int colDiff = to.Y - from.Y;
                int rowStep = rowDiff > 0 ? 1 : -1;
                int colStep = colDiff > 0 ? 1 : -1;

                int currentRow = from.X + rowStep;
                int currentCol = from.Y + colStep;

                while (currentRow != to.X && currentCol != to.Y)
                {
                    if (board[currentRow, currentCol] != CheckersPiece.Empty)
                    {
                        count++;
                    }
                    currentRow += rowStep;
                    currentCol += colStep;
                }
                return count;
            }
            else
            {
                return Math.Abs(to.X - from.X) == 2 ? 1 : 0;
            }
        }

        private Point GetBestCaptureMove(List<Point> capturingPieces)
        {
            Point bestMove = capturingPieces[0];
            int maxCaptures = 0;

            foreach (Point piece in capturingPieces)
            {
                int captureCount = CountPossibleCaptures(piece, false);
                if (captureCount > maxCaptures)
                {
                    maxCaptures = captureCount;
                    bestMove = piece;
                }
            }

            return bestMove;
        }

        private int CountPossibleCaptures(Point position, bool isPlayer)
        {
            List<Point> captures = GetPossibleCaptures(position, isPlayer);
            if (captures.Count == 0) return 0;

            int maxCount = 0;
            foreach (Point capture in captures)
            {
                // Создаем копию доски для тестирования
                CheckersPiece[,] tempBoard = new CheckersPiece[BOARD_SIZE, BOARD_SIZE];
                Array.Copy(board, tempBoard, board.Length);

                // Временно делаем ход
                CheckersPiece piece = board[position.X, position.Y];
                board[capture.X, capture.Y] = piece;
                board[position.X, position.Y] = CheckersPiece.Empty;

                // Убираем взятые фигуры
                RemoveCapturedPieces(position, capture);

                int capturedCount = CountCapturedPieces(position, capture);
                int totalCount = capturedCount + CountPossibleCaptures(capture, isPlayer);

                if (totalCount > maxCount) maxCount = totalCount;

                // Восстанавливаем доску
                board = tempBoard;
            }

            return maxCount;
        }

        private void RemoveCapturedPieces(Point from, Point to)
        {
            CheckersPiece piece = board[to.X, to.Y]; // Фигура уже перемещена
            bool isKing = (piece == CheckersPiece.PlayerKing || piece == CheckersPiece.ComputerKing);

            if (isKing)
            {
                int rowDiff = to.X - from.X;
                int colDiff = to.Y - from.Y;
                int rowStep = rowDiff > 0 ? 1 : -1;
                int colStep = colDiff > 0 ? 1 : -1;

                int currentRow = from.X + rowStep;
                int currentCol = from.Y + colStep;

                while (currentRow != to.X && currentCol != to.Y)
                {
                    if (board[currentRow, currentCol] != CheckersPiece.Empty)
                    {
                        board[currentRow, currentCol] = CheckersPiece.Empty;
                    }
                    currentRow += rowStep;
                    currentCol += colStep;
                }
            }
            else
            {
                if (Math.Abs(to.X - from.X) == 2)
                {
                    int middleRow = (from.X + to.X) / 2;
                    int middleCol = (from.Y + to.Y) / 2;
                    board[middleRow, middleCol] = CheckersPiece.Empty;
                }
            }
        }

        private void MakeComputerRegularMove()
        {
            List<Point> possibleMoves = new List<Point>();

            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    if (IsComputerPiece(board[row, col]))
                    {
                        List<Point> validMoves = GetValidMovesForPiece(new Point(row, col), false);
                        foreach (Point move in validMoves)
                        {
                            Point encodedMove = new Point(row * 8 + col, move.X * 8 + move.Y);

                            // Для сложного уровня добавляем вес ходам
                            if (difficulty == 2)
                            {
                                int weight = EvaluateMove(new Point(row, col), move);
                                for (int i = 0; i < weight; i++)
                                {
                                    possibleMoves.Add(encodedMove);
                                }
                            }
                            else
                            {
                                possibleMoves.Add(encodedMove);
                            }
                        }
                    }
                }
            }

            if (possibleMoves.Count > 0)
            {
                Point move = possibleMoves[random.Next(possibleMoves.Count)];
                Point from = new Point(move.X / 8, move.X % 8);
                Point to = new Point(move.Y / 8, move.Y % 8);

                MakeMove(from, to);
                CheckPromotion(to, false);
            }
        }

        private List<Point> GetValidMovesForPiece(Point position, bool isPlayer)
        {
            List<Point> moves = new List<Point>();
            CheckersPiece piece = board[position.X, position.Y];
            bool isKing = (piece == CheckersPiece.PlayerKing || piece == CheckersPiece.ComputerKing);

            if (isKing)
            {
                // Дамка может ходить в любом диагональном направлении на любое расстояние
                int[] directions = { -1, 1 };
                foreach (int rowDir in directions)
                {
                    foreach (int colDir in directions)
                    {
                        for (int distance = 1; distance < BOARD_SIZE; distance++)
                        {
                            int newRow = position.X + distance * rowDir;
                            int newCol = position.Y + distance * colDir;

                            if (newRow >= 0 && newRow < BOARD_SIZE &&
                                newCol >= 0 && newCol < BOARD_SIZE &&
                                (newRow + newCol) % 2 == 1)
                            {
                                if (IsValidMove(position, new Point(newRow, newCol), isPlayer))
                                {
                                    moves.Add(new Point(newRow, newCol));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Обычная шашка - ходит только на соседние клетки
                int[] directions = { -1, 1 };
                foreach (int rowDir in directions)
                {
                    foreach (int colDir in directions)
                    {
                        // Ограничиваем направление для обычных шашек
                        if (!isPlayer && rowDir < 0) continue; // Компьютер не ходит назад
                        if (isPlayer && rowDir > 0) continue; // Игрок не ходит назад

                        int newRow = position.X + rowDir;
                        int newCol = position.Y + colDir;

                        if (newRow >= 0 && newRow < BOARD_SIZE &&
                            newCol >= 0 && newCol < BOARD_SIZE &&
                            (newRow + newCol) % 2 == 1)
                        {
                            if (IsValidMove(position, new Point(newRow, newCol), isPlayer))
                            {
                                moves.Add(new Point(newRow, newCol));
                            }
                        }

                        // Также проверяем ходы со взятием
                        newRow = position.X + 2 * rowDir;
                        newCol = position.Y + 2 * colDir;

                        if (newRow >= 0 && newRow < BOARD_SIZE &&
                            newCol >= 0 && newCol < BOARD_SIZE &&
                            (newRow + newCol) % 2 == 1)
                        {
                            if (IsValidMove(position, new Point(newRow, newCol), isPlayer))
                            {
                                moves.Add(new Point(newRow, newCol));
                            }
                        }
                    }
                }
            }

            return moves;
        }

        private int EvaluateMove(Point from, Point to)
        {
            int weight = 1;

            // Предпочтение ходам к центру доски
            double centerDistance = Math.Abs(to.X - 3.5) + Math.Abs(to.Y - 3.5);
            if (centerDistance < 3) weight += 2;

            // Предпочтение продвижению вперед
            if (to.X > from.X) weight += 1;

            // Предпочтение превращению в дамку
            if (board[from.X, from.Y] == CheckersPiece.Computer && to.X == 7) weight += 5;

            // Предпочтение взятию
            if (IsCapturingMove(from, to)) weight += 3;

            return weight;
        }

        private List<Point> GetCapturingPieces(bool isPlayer)
        {
            List<Point> capturingPieces = new List<Point>();

            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    CheckersPiece piece = board[row, col];
                    if ((isPlayer && IsPlayerPiece(piece)) || (!isPlayer && IsComputerPiece(piece)))
                    {
                        if (CanCapture(new Point(row, col), isPlayer))
                        {
                            capturingPieces.Add(new Point(row, col));
                        }
                    }
                }
            }

            return capturingPieces;
        }

        private bool CanCapture(Point position, bool isPlayer)
        {
            return GetPossibleCaptures(position, isPlayer).Count > 0;
        }

        private List<Point> GetPossibleCaptures(Point position, bool isPlayer)
        {
            List<Point> captures = new List<Point>();
            CheckersPiece piece = board[position.X, position.Y];
            bool isKing = (piece == CheckersPiece.PlayerKing || piece == CheckersPiece.ComputerKing);

            if (isKing)
            {
                // Дамки могут брать в любом диагональном направлении на любое расстояние
                int[] directions = { -1, 1 };
                foreach (int rowDir in directions)
                {
                    foreach (int colDir in directions)
                    {
                        for (int distance = 2; distance < BOARD_SIZE; distance++)
                        {
                            int jumpRow = position.X + distance * rowDir;
                            int jumpCol = position.Y + distance * colDir;

                            if (jumpRow >= 0 && jumpRow < BOARD_SIZE &&
                                jumpCol >= 0 && jumpCol < BOARD_SIZE &&
                                board[jumpRow, jumpCol] == CheckersPiece.Empty &&
                                (jumpRow + jumpCol) % 2 == 1)
                            {
                                // Проверяем, есть ли враги на пути
                                bool hasEnemyOnPath = false;
                                bool hasAllyOnPath = false;

                                for (int step = 1; step < distance; step++)
                                {
                                    int checkRow = position.X + step * rowDir;
                                    int checkCol = position.Y + step * colDir;
                                    CheckersPiece pathPiece = board[checkRow, checkCol];

                                    if (pathPiece != CheckersPiece.Empty)
                                    {
                                        if ((isPlayer && IsComputerPiece(pathPiece)) ||
                                            (!isPlayer && IsPlayerPiece(pathPiece)))
                                        {
                                            hasEnemyOnPath = true;
                                        }
                                        else
                                        {
                                            hasAllyOnPath = true;
                                            break; // Путь заблокирован союзником
                                        }
                                    }
                                }

                                if (hasEnemyOnPath && !hasAllyOnPath)
                                {
                                    captures.Add(new Point(jumpRow, jumpCol));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Обычные шашки берут только через одну клетку
                int[] directions = { -1, 1 };
                foreach (int rowDir in directions)
                {
                    foreach (int colDir in directions)
                    {
                        int jumpRow = position.X + 2 * rowDir;
                        int jumpCol = position.Y + 2 * colDir;
                        int middleRow = position.X + rowDir;
                        int middleCol = position.Y + colDir;

                        if (jumpRow >= 0 && jumpRow < BOARD_SIZE &&
                            jumpCol >= 0 && jumpCol < BOARD_SIZE &&
                            board[jumpRow, jumpCol] == CheckersPiece.Empty &&
                            (jumpRow + jumpCol) % 2 == 1)
                        {
                            CheckersPiece middlePiece = board[middleRow, middleCol];
                            if ((isPlayer && IsComputerPiece(middlePiece)) ||
                                (!isPlayer && IsPlayerPiece(middlePiece)))
                            {
                                captures.Add(new Point(jumpRow, jumpCol));
                            }
                        }
                    }
                }
            }

            return captures;
        }

        private bool IsPlayerPiece(CheckersPiece piece)
        {
            return piece == CheckersPiece.Player || piece == CheckersPiece.PlayerKing;
        }

        private bool IsComputerPiece(CheckersPiece piece)
        {
            return piece == CheckersPiece.Computer || piece == CheckersPiece.ComputerKing;
        }

        private void ClearSelection()
        {
            if (selectedPiece.X != -1)
            {
                int row = selectedPiece.X;
                int col = selectedPiece.Y;

                // Восстанавливаем цвет клетки
                if ((row + col) % 2 == 0)
                    boardButtons[row, col].BackColor = Color.FromArgb(240, 217, 181);
                else
                    boardButtons[row, col].BackColor = Color.FromArgb(181, 136, 99);

                selectedPiece = new Point(-1, -1);
            }
        }

        private void UpdateStatus(string status)
        {
            Label statusLabel = (Label)this.Controls["StatusLabel"];
            if (statusLabel != null)
                statusLabel.Text = status;
        }

        private bool CheckGameEnd()
        {
            int playerPieces = 0;
            int computerPieces = 0;
            bool playerCanMove = false;
            bool computerCanMove = false;

            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    CheckersPiece piece = board[row, col];
                    if (IsPlayerPiece(piece))
                    {
                        playerPieces++;
                        if (!playerCanMove && HasValidMoves(new Point(row, col), true))
                            playerCanMove = true;
                    }
                    else if (IsComputerPiece(piece))
                    {
                        computerPieces++;
                        if (!computerCanMove && HasValidMoves(new Point(row, col), false))
                            computerCanMove = true;
                    }
                }
            }

            if (playerPieces == 0 || !playerCanMove)
            {
                EndGame("Loss");
                return true;
            }
            else if (computerPieces == 0 || !computerCanMove)
            {
                EndGame("Win");
                return true;
            }

            return false;
        }

        private bool HasValidMoves(Point position, bool isPlayer)
        {
            // Проверяем возможность взятия
            if (CanCapture(position, isPlayer)) return true;

            // Проверяем обычные ходы
            CheckersPiece piece = board[position.X, position.Y];
            bool isKing = (piece == CheckersPiece.PlayerKing || piece == CheckersPiece.ComputerKing);

            if (isKing)
            {
                // Дамка может ходить в любом диагональном направлении
                int[] directions = { -1, 1 };
                foreach (int rowDir in directions)
                {
                    foreach (int colDir in directions)
                    {
                        for (int distance = 1; distance < BOARD_SIZE; distance++)
                        {
                            int newRow = position.X + distance * rowDir;
                            int newCol = position.Y + distance * colDir;

                            if (newRow >= 0 && newRow < BOARD_SIZE &&
                                newCol >= 0 && newCol < BOARD_SIZE &&
                                (newRow + newCol) % 2 == 1)
                            {
                                if (board[newRow, newCol] == CheckersPiece.Empty)
                                {
                                    // Проверяем, что путь свободен
                                    bool pathClear = true;
                                    for (int step = 1; step < distance; step++)
                                    {
                                        int checkRow = position.X + step * rowDir;
                                        int checkCol = position.Y + step * colDir;
                                        if (board[checkRow, checkCol] != CheckersPiece.Empty)
                                        {
                                            pathClear = false;
                                            break;
                                        }
                                    }
                                    if (pathClear) return true;
                                }
                                else
                                {
                                    break; // Путь заблокирован
                                }
                            }
                            else
                            {
                                break; // Вышли за границы доски
                            }
                        }
                    }
                }
            }
            else
            {
                // Обычная шашка
                int[] directions = { -1, 1 };
                foreach (int rowDir in directions)
                {
                    foreach (int colDir in directions)
                    {
                        // Для обычных шашек ограничиваем направление
                        if (isPlayer && rowDir > 0) continue;
                        if (!isPlayer && rowDir < 0) continue;

                        int newRow = position.X + rowDir;
                        int newCol = position.Y + colDir;

                        if (newRow >= 0 && newRow < BOARD_SIZE &&
                            newCol >= 0 && newCol < BOARD_SIZE &&
                            board[newRow, newCol] == CheckersPiece.Empty &&
                            (newRow + newCol) % 2 == 1)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void EndGame(string result)
        {
            string message = result == "Win" ? "Вы выиграли!" : "Вы проиграли!";
            MessageBox.Show(message);

            // Обновляем статистику в БД
            DatabaseHelper.UpdateGameStats(CurrentUser.UserID, result);

            this.Close();
        }
    }

    public enum CheckersPiece
    {
        Empty,
        Player,
        Computer,
        PlayerKing,
        ComputerKing
    }
}