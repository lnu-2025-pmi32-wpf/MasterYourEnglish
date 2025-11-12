using MasterYourEnglish.Presentation.ViewModels.Commands;
using System.Collections.Generic;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    // DTO for a single card in the session
    // (This should live in your BLL project)
    public class FlashcardSessionDto
    {
        public string Word { get; set; }
        public string Transcription { get; set; }
        public string Definition { get; set; }
        public string Example { get; set; }
        public string PartOfSpeech { get; set; }
    }

    public class FlashcardSessionViewModel : ViewModelBase
    {
        // --- Private Fields ---
        private List<FlashcardSessionDto> _sessionCards;
        private int _currentIndex;

        // --- Public Properties for Binding ---
        private bool _isFlipped;
        public bool IsFlipped
        {
            get => _isFlipped;
            set => SetProperty(ref _isFlipped, value);
        }

        private FlashcardSessionDto _currentFlashcard;
        public FlashcardSessionDto CurrentFlashcard
        {
            get => _currentFlashcard;
            set => SetProperty(ref _currentFlashcard, value);
        }

        private string _progressText;
        public string ProgressText
        {
            get => _progressText;
            set => SetProperty(ref _progressText, value);
        }

        public string TimerText { get; set; } = "0:00"; // Timer still optional

        // --- Commands ---
        public ICommand FlipCardCommand { get; }
        public ICommand NextCardCommand { get; }
        public ICommand PreviousCardCommand { get; }
        public ICommand KnowCommand { get; }
        public ICommand DontKnowCommand { get; }
        public ICommand AddToSavedCommand { get; }

        public FlashcardSessionViewModel(/* BLL Service */)
        {
            _sessionCards = new List<FlashcardSessionDto>();

            // --- 
            // THIS IS THE FIX. We wrap the methods in lambdas (e.g., "p => ...")
            // to match the expected signature. The 'p' (parameter) is ignored.
            // --- 
            FlipCardCommand = new RelayCommand(p => IsFlipped = !IsFlipped);
            NextCardCommand = new RelayCommand(p => NextCard(), p => CanGoNext());
            PreviousCardCommand = new RelayCommand(p => PreviousCard(), p => CanGoPrevious());
            KnowCommand = new RelayCommand(p => NextCard(), p => CanGoNext());
            DontKnowCommand = new RelayCommand(p => NextCard(), p => CanGoNext());
            AddToSavedCommand = new RelayCommand(p => AddToSaved());
        }

        // This is the new method called by MainViewModel
        public void LoadSession(int bundleId)
        {
            // In a real app, you'd call:
            // _sessionCards = await _bllService.GetCardsForBundle(bundleId);

            // For now, we load fake data
            _sessionCards = new List<FlashcardSessionDto>
            {
                new FlashcardSessionDto { Word = "gooning", Transcription = "[/ˈguː.nɪŋ/]", Definition = "behaving in a foolish or silly way; acting like a goon", Example = "\"The kids were gooning around...\"", PartOfSpeech = "verb (informal)" },
                new FlashcardSessionDto { Word = "Ephemeral", Transcription = "[/əˈfem.ər.əl/]", Definition = "lasting for only a short time", Example = "\"Her success as a singer was ephemeral.\"", PartOfSpeech = "adjective" },
                new FlashcardSessionDto { Word = "Lethargic", Transcription = "[/ləˈθɑːr.dʒɪk/]", Definition = "having little energy; feeling unwilling and unable to do anything", Example = "\"I was feeling tired and lethargic.\"", PartOfSpeech = "adjective" },
                new FlashcardSessionDto { Word = "Sycophant", Transcription = "[/ˈsɪk.ə.fænt/]", Definition = "a person who praises important people in an obsequious way to get something from them", Example = "\"The prime minister was surrounded by sycophants.\"", PartOfSpeech = "noun" },
                new FlashcardSessionDto { Word = "Pulchritude", Transcription = "[/ˈpʌl.krə.tuːd/]", Definition = "beauty, especially physical beauty", Example = "\"The judges were charmed by her pulchritude.\"", PartOfSpeech = "noun (formal)" }
            };

            // Reset the session
            _currentIndex = 0;
            IsFlipped = false;
            UpdateCurrentCard();
        }

        private void UpdateCurrentCard()
        {
            CurrentFlashcard = _sessionCards[_currentIndex];
            ProgressText = $"{_currentIndex + 1}/{_sessionCards.Count}";
            // Re-evaluate if the buttons should be enabled
            CommandManager.InvalidateRequerySuggested();
        }

        // --- Command Logic (These methods are now parameter-less, which is fine) ---
        private void NextCard()
        {
            if (CanGoNext())
            {
                _currentIndex++;
                IsFlipped = false; // Flip back to the front
                UpdateCurrentCard();
            }
        }
        private bool CanGoNext() => _sessionCards.Count > 0 && _currentIndex < _sessionCards.Count - 1;

        private void PreviousCard()
        {
            if (CanGoPrevious())
            {
                _currentIndex--;
                IsFlipped = false; // Flip back to the front
                UpdateCurrentCard();
            }
        }
        private bool CanGoPrevious() => _currentIndex > 0;

        private void AddToSaved()
        {
            // In a real app:
            // await _bllService.AddToSaved(CurrentFlashcard.Id);
            // For now, it just pretends to do something.
        }
    }
}