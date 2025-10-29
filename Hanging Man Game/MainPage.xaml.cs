using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hanging_Man_Game
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region UI Bindings
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
        #endregion

        #region Visibility Bindings
        private bool isHeadVisible;
        public bool IsHeadVisible { get => isHeadVisible; set { isHeadVisible = value; OnPropertyChanged(); } }

        private bool isBodyVisible;
        public bool IsBodyVisible { get => isBodyVisible; set { isBodyVisible = value; OnPropertyChanged(); } }

        private bool isLeftHandVisible;
        public bool IsLeftHandVisible { get => isLeftHandVisible; set { isLeftHandVisible = value; OnPropertyChanged(); } }

        private bool isRightHandVisible;
        public bool IsRightHandVisible { get => isRightHandVisible; set { isRightHandVisible = value; OnPropertyChanged(); } }

        private bool isFeetVisible;
        public bool IsFeetVisible { get => isFeetVisible; set { isFeetVisible = value; OnPropertyChanged(); } }
        #endregion

        #region Fields
        private readonly List<string> words = new()
        {
            "lion","bird","snake","monkey","tiger","parrot","raccoon","snail"
        };

        public string Hint
        {
            get => hint;
            set { hint = value; OnPropertyChanged(); }
        }
        private string hint;

        private string answer = "";
        private string spotlight;
        private readonly List<char> guessed = new();
        private List<char> letters = new();
        private string message;
        private string gameStatus;
        private int mistakes = 0;

        private readonly int maxWrong = 5;
        #endregion

        public MainPage()
        {
            InitializeComponent();
            Letters.AddRange("abcdefghijklmnopqrstuvwxyz");
            BindingContext = this;

            PickWord();
            CalculateWord(answer, guessed);
            UpdateStatus();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void PickWord()
        {
            var rand = new Random();
            answer = words[rand.Next(words.Count)];

            // Assign hints based on the chosen word
            switch (answer)
            {
                case "lion":
                    Hint = "Hint: This is the king of the jungle.";
                    break;
                case "bird":
                    Hint = "Hint: This animal can fly.";
                    break;
                case "snake":
                    Hint = "Hint: This animal slithers on the ground.";
                    break;
                case "monkey":
                    Hint = "Hint: This animal loves bananas.";
                    break;
                case "tiger":
                    Hint = "Hint: This is a striped wild cat.";
                    break;
                case "parrot":
                    Hint = "Hint: This bird can mimic human speech.";
                    break;
                case "raccoon":
                    Hint = "Hint: This animal is known for wearing a 'mask'.";
                    break;
                case "snail":
                    Hint = "Hint: This animal carries its home on its back.";
                    break;
            }
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
                case 1: IsHeadVisible = true; break;
                case 2: IsBodyVisible = true; break;
                case 3: IsLeftHandVisible = true; break;
                case 4: IsRightHandVisible = true; break;
                case 5: IsFeetVisible = true; break;
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

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                btn.IsEnabled = false;
                HandleGuess(btn.Text[0]);
            }
        }

        private void Home_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new LandingPage();
        }

        private void Reset_Clicked(object sender, EventArgs e)
        {
            mistakes = 0;
            guessed.Clear();
            IsHeadVisible = false;
            IsBodyVisible = false;
            IsLeftHandVisible = false;
            IsRightHandVisible = false;
            IsFeetVisible = false;

            PickWord();
            CalculateWord(answer, guessed);
            Message = "";
            UpdateStatus();
            EnableLetters();
        }
    }
}