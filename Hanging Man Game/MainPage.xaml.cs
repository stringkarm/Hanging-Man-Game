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

        #region Visibility Bindings.
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

        #region Fields (UPDATED)
        private readonly Dictionary<string, (List<string> words, List<string> hints)> Categories = new()
        {
            {
                "Mammals",
                (
                    new() { "orangutan", "rhinoceros", "capybara", "chimpanzee", "hippopotamus" },
                    new()
                    {
                        "Hint: A large, tree-dwelling great ape with long arms.",
                        "Hint: A large, herbivorous mammal with a distinctive horn on its nose.",
                        "Hint: The world's largest rodent, semi-aquatic and found in South America.",
                        "Hint: A highly intelligent great ape, closely related to humans.",
                        "Hint: A large, mostly herbivorous mammal, spending its days in the water."
                    }
                )
            },
            {
                "Reptiles",
                (
                    new() { "alligator", "anaconda", "crocodilian", "chameleon" },
                    new()
                    {
                        "Hint: A large semi-aquatic reptile with a broad snout and powerful jaws.",
                        "Hint: A massive, non-venomous snake found in tropical South America.",
                        "Hint: A large, predatory reptile that lives mostly in the water.",
                        "Hint: A lizard famous for its ability to change skin color."
                    }
                )
            },
            {
                "Plants",
                (
                    new() { "mahogany", "bromeliad", "cecropia", "stranglerfig", "fern" },
                    new()
                    {
                        "Hint: A tropical hardwood tree prized for its reddish-brown wood.",
                        "Hint: A family of tropical flowering plants often found growing on trees.",
                        "Hint: A fast-growing pioneer tree known for its hollow stems.",
                        "Hint: A type of tree that germinates in a host tree and slowly envelops it.",
                        "Hint: A non-flowering plant that reproduces by spores, common on the forest floor."
                    }
                )
            },
            {
                "Insects",
                (
                    new() { "tarantula", "millipede", "cockroach", "stickinsect", "leafcutterant" },
                    new()
                    {
                        "Hint: A very large, hairy spider (arachnid) found in tropical regions.",
                        "Hint: A long-bodied arthropod with many pairs of legs (not always a thousand!).",
                        "Hint: A nocturnal insect notorious for infesting homes, although many live in forests.",
                        "Hint: An insect highly specialized for camouflage, resembling a twig.",
                        "Hint: A tropical insect that cuts foliage to farm fungus in its nest."
                    }
                )
            },
            {
                "Phenomena",
                (
                    new() { "biodiversity", "canopy", "understory", "ecosystem" },
                    new()
                    {
                        "Hint: The variety of life in the world or in a particular habitat.",
                        "Hint: The uppermost layer of branches and leaves in a forest.",
                        "Hint: The layer of vegetation beneath the main branches of a forest.",
                        "Hint: A biological community of interacting organisms and their physical environment."
                    }
                )
            }
        };

        private string currentCategory = "Mammals"; // Default to a new category

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

        private async void Category_Clicked(object sender, EventArgs e)
        {
             
            string[] categoryNames = Categories.Keys.ToArray();

            string action = await DisplayActionSheet("Choose Category", "Cancel", null, categoryNames);

            //  new categories
            if (Categories.ContainsKey(action))
            {
                currentCategory = action;
                Reset_Clicked(sender, e);
            }
        }

        private void PickWord()
        {
            var categoryData = Categories[currentCategory];
            var wordList = categoryData.words;
            var hintList = categoryData.hints;

            var rand = new Random();
            int index = rand.Next(wordList.Count);

            answer = wordList[index];
            Hint = hintList[index];
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
            GameStatus = $"     Errors: {mistakes} of {maxWrong} \nCategory: {currentCategory}";
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