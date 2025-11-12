using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.Presentation.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class SavedFlashcardsViewModel : ViewModelBase
    {
        public ObservableCollection<SavedFlashcardDto> SavedFlashcards { get; }

        private SavedFlashcardDto _selectedFlashcard;
        public SavedFlashcardDto SelectedFlashcard
        {
            get => _selectedFlashcard;
            set => SetProperty(ref _selectedFlashcard, value);
        }

        public ICommand MarkAsLearnedCommand { get; }
        public ICommand RemoveFromSavedCommand { get; }

        public SavedFlashcardsViewModel(/* BLL service here */)
        {
            SavedFlashcards = new ObservableCollection<SavedFlashcardDto>();
            LoadFakeData();

            if (SavedFlashcards.Count > 0)
            {
                SelectedFlashcard = SavedFlashcards[0];
            }

            MarkAsLearnedCommand = new RelayCommand(OnMarkAsLearned);
            RemoveFromSavedCommand = new RelayCommand(OnRemoveFromSaved);
        }

        private void OnMarkAsLearned()
        {

        }

        private void OnRemoveFromSaved()
        {

        }

        private void LoadFakeData()
        {
            SavedFlashcards.Add(new SavedFlashcardDto
            {
                Word = "Innermost",
                PartOfSpeech = "adjective",
                Difficulty = "B2",
                Transcription = "[/ˌɪnərˈmoʊst/]",
                Meaning = "most secret and hidden",
                Example = "This was the diary in which Gina recorded her innermost thoughts and secrets."
            });
            SavedFlashcards.Add(new SavedFlashcardDto
            {
                Word = "Fluid",
                PartOfSpeech = "adjective",
                Difficulty = "B1",
                Transcription = "[/ˈfluː.ɪd/]",
                Meaning = "smooth and continuous",
                Example = "The dancer's movements were fluid and graceful."
            });
            SavedFlashcards.Add(new SavedFlashcardDto { Word = "Grace", PartOfSpeech = "noun", Difficulty = "B2", Transcription = "[/ɡreɪs/]", Meaning = "a quality of moving in a smooth, relaxed, and attractive way", Example = "She moved with the grace of a cat." });
            SavedFlashcards.Add(new SavedFlashcardDto { Word = "Quintessential", PartOfSpeech = "adjective", Difficulty = "C1", Transcription = "[/ˌkwɪn.tɪˈsen.ʃəl/]", Meaning = "being the most typical example or most important part of something", Example = "He was the quintessential tough guy." });
            SavedFlashcards.Add(new SavedFlashcardDto { Word = "Ambivalence", PartOfSpeech = "noun", Difficulty = "C1", Transcription = "[/æmˈbɪv.ə.ləns/]", Meaning = "the state of having two opposing feelings at the same time", Example = "He felt a certain ambivalence towards his new job." });
        }
    }
}