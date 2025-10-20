using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace Hanging_Man_Game
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region UI Properties
        public string Spotlight
        {
            get => spotlight;
            set { spotlight = value; OnPropertyChanged(); }
        }

        public List<char> Letters
        {
            get => letters;
            set { letters = value; OnPropertyChanged(); }
        }

        public string Message
        {
            get => message;
            set { message = value; OnPropertyChanged(); }
        }

        public string GameStatus
        {
            get => gameStatus;
            set { gameStatus = value; OnPropertyChanged(); }
        }

        public string CurrentImage
        {
            get => currentImage;
            set { currentImage = value; OnPropertyChanged(); }
        }
        #endregion

        #region Fields
        private readonly List<string> words = new()
        {
            "python","javascript","maui","mongodb","sql","xaml","powerpoint","code"
        };

        private string answer = "";
        private string spotlight;
        private readonly List<char> guessed = new();
        private List<char> letters = new();
        private string message;
        private string gameStatus;
        private int mistakes = 0;

        private readonly int maxWrong = 5;
        private string currentImage = "head.png";
        #endregion

        public MainPage()
        {
            InitializeComponent();
            Letters.AddRange("abcdefghijklmnopqrstuvwxyz");
            BindingContext = this;

            PickWord();
            CalculateWord(answer, guessed);
            UpdateStatus();
            CurrentImage = "head.png"; 
        }


        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PickWord()
        {
            answer = words[new Random().Next(words.Count)];
        }

        private void CalculateWord(string answer, List<char> guessed)
        {
            var temp = answer.Select(x => guessed.Contains(x) ? x : '_').ToArray();
            Spotlight = string.Join(' ', temp);
        }

        private void HandleGuess(char letter)
        {
            if (guessed.Contains(letter)) return;

            guessed.Add(letter);

            if (answer.Contains(letter))
            {
                CalculateWord(answer, guessed);
                CheckIfGameWon();
            }
            else
            {
                mistakes++;
                UpdateStatus();
                UpdateImage();
                CheckIfGameLost();
            }
        }

        private void UpdateImage()
        {
           
            switch (mistakes)
            {
                case 1: CurrentImage = "head.png"; break;
                case 2: CurrentImage = "stomach.png"; break;
                case 3: CurrentImage = "lefthand.png"; break;
                case 4: CurrentImage = "righthand.png"; break;
                case 5: CurrentImage = "feet.png"; break;
                default: CurrentImage = "head.png"; break;
            }
        }

        private void UpdateStatus()
        {
            GameStatus = $"Errors: {mistakes} of {maxWrong}";
        }

        private void CheckIfGameWon()
        {
            if (Spotlight.Replace(" ", "") == answer)
            {
                Message = "🎉 You Win!";
                DisableLetters();
            }
        }

        private void CheckIfGameLost()
        {
            if (mistakes >= maxWrong)
            {
                Message = $"💀 You lost! The word was: {answer}";
                DisableLetters();
            }
        }

        private void DisableLetters()
        {
            foreach (var child in LettersContainer.Children)
                if (child is Button btn)
                    btn.IsEnabled = false;
        }

        private void EnableLetters()
        {
            foreach (var child in LettersContainer.Children)
                if (child is Button btn)
                    btn.IsEnabled = true;
        }
        private void Reset_Clicked(object sender, EventArgs e)
        {
            mistakes = 0;
            guessed.Clear();
            CurrentImage = "head.png";
            PickWord();
            CalculateWord(answer, guessed);
            Message = "";
            UpdateStatus();
            EnableLetters();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                btn.IsEnabled = false;
                HandleGuess(btn.Text[0]);
            }
        }
    }

}
