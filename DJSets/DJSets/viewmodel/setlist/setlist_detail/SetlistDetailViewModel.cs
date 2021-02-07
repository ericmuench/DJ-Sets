using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using DJSets.clerks.audio;
using DJSets.clerks.dataservices;
using DJSets.clerks.dataservices.entityframework;
using DJSets.clerks.ef_util;
using DJSets.clerks.timeformat;
using DJSets.model.entityframework;
using DJSets.model.model_observe;
using DJSets.resources;
using DJSets.util.async;
using DJSets.util.Extensions;
using DJSets.util.mvvm;
using DJSets.viewmodel.basics;
using DJSets.viewmodel.setlistposition;
using GongSolutions.Wpf.DragDrop;

namespace DJSets.viewmodel.setlist.setlist_detail
{
    /// <summary>
    /// This class is the viewmodel for a SetlistDetailView and provides all necessary data
    /// </summary>
    public class SetlistDetailViewModel : BaseViewModel, IElementContainer<Setlist>, IDropTarget
    {
        #region Constructors

        public SetlistDetailViewModel(IElementContainer<Setlist> setlistContainer)
        {
            //init data services
            var dataService = new EfSqliteSetlistPositionDataService(setlistContainer.GetElement());
            _setlistPositionDataService = dataService;
            _setlistPositionMover = dataService;

            //assign setlist
            _setlist = setlistContainer.GetElement();
            ApplyData();

            //config commands
            ConfigureCommands();

            //register observers
            ModelNotificationCenter.SetlistNotifier.RegisterObserver(ApplyData);
            ModelNotificationCenter.SongNotifier.RegisterObserver(ApplyData);
        }
        #endregion

        #region Finalizers
        ~SetlistDetailViewModel()
        {
            ModelNotificationCenter.SetlistNotifier.UnRegisterObserver(ApplyData);
            ModelNotificationCenter.SongNotifier.UnRegisterObserver(ApplyData);
        }
        #endregion

        #region Commands
        /// <summary>
        /// This Command deletes a song
        /// </summary>
        public DelegateCommand<object, bool> DeleteCommand { get; set; }

        #endregion

        #region Fields

        #region Clerks
        /// <summary>
        /// This class handles all time format-actions
        /// </summary>
        private readonly TimeFormatConverter _timeFormatConverter = new TimeFormatConverter();

        /// <summary>
        /// This clerk allows DB-Interactions with Setlists
        /// </summary>
        private readonly IDataService<Setlist> _setlistDataService = new EfSqliteSetlistDataService();

        /// <summary>
        /// This clerk allows DB-Interactions with SetlistPositions
        /// </summary>
        private readonly IDataService<SetlistPosition> _setlistPositionDataService;

        /// <summary>
        /// This clerk allows to move SetlistPositions in the current Setlist
        /// </summary>
        private readonly ISetlistPositionMover _setlistPositionMover;

        /// <summary>
        /// This clerk allows playing audio
        /// </summary>
        private readonly AudioPlayer _audioPlayer = new AudioPlayer();

        #endregion

        #region _setlist
        /// <summary>
        /// This field stores the setlist for which its values are presented by this view
        /// </summary>
        private readonly Setlist _setlist;
        #endregion

        #region SetListTitle

        /// <summary>
        /// This field stores the SetlistTitle value
        /// </summary>
        private string _setListTitle = "SETLISTTITLE";

        ///<summary>
        /// This property provides the SetlistTitle-Value stored in <see cref="_setListTitle"/> for eventual Binding in xaml.
        /// </summary>
        public string SetListTitle
        {
            get => _setListTitle;
            set => Set(ref _setListTitle, value, nameof(SetListTitle));
        }

        #endregion

        #region SetListDescription

        /// <summary>
        /// This field stores the SetListDescription value
        /// </summary>
        private string _setListDescription = "SETLISTDESCRIPTION";

        ///<summary>
        /// This property provides the SetListDescription-Value stored in <see cref="_setListDescription"/> for eventual Binding in xaml.
        /// </summary>
        public string SetListDescription
        {
            get => _setListDescription;
            set => Set(ref _setListDescription, value, nameof(SetListDescription));
        }

        #endregion

        #region Duration
        /// <summary>
        /// This field stores the Duration value
        /// </summary>
        private long _duration;
        ///<summary>
        /// This property provides the Duration-Value stored in <see cref="_duration"/> for eventual Binding in xaml.
        /// </summary>
        public long Duration
        {
            get => _duration;
            set
            {
                Set(ref _duration, value, nameof(Duration));
                OnPropertyChanged(nameof(SetListDurationString));
            }
        }
        #endregion

        #region SetListDurationString

        ///<summary>
        /// This property provides the SetListDurationString-Value for eventual Binding in xaml.
        /// </summary>
        public string SetListDurationString
        {
            get
            {
                var templateStr =
                    Application.Current.GetResource<string>(StringResourceKeys.StrTemplateSetlistDuration);
                var formatDuration = _timeFormatConverter.FormatToTimeString(Duration);
                return templateStr.Replace("#", formatDuration);
            }
        }
        #endregion

        #region SetlistPositionVMs
        /// <summary>
        /// This field stores the SetlistPositionVMs value
        /// </summary>
        private ObservableCollection<DetailedSetlistPositionItemViewModel> _setlistPositionVMs = 
            new ObservableCollection<DetailedSetlistPositionItemViewModel>();
        ///<summary>
        /// This property provides the SetlistPositionVMs-Value stored in <see cref="_setlistPositionVMs"/> for eventual Binding in xaml.
        /// </summary>
        public ObservableCollection<DetailedSetlistPositionItemViewModel> SetlistPositionVMs
        {
            get => _setlistPositionVMs;
            set
            {
                Set(ref _setlistPositionVMs, value, nameof(SetlistPositionVMs));
                SetlistPositionsView = new ListCollectionView(SetlistPositionVMs);

                SetlistPositionsView.CurrentChanged += (sender, args) 
                    => OnPropertyChanged(nameof(SelectedSetlistPositionVm));
            }
        }
        #endregion

        #region SelectedSetlistPositionVm
        public DetailedSetlistPositionItemViewModel SelectedSetlistPositionVm 
            => SetlistPositionsView.CurrentItem as DetailedSetlistPositionItemViewModel;

        #endregion

        #region SetlistPositionsView
        /// <summary>
        /// This field provides stores the backing field value of <see cref="SetlistPositionsView"/>
        /// </summary>
        private ListCollectionView _setlistPositionsView = 
            new ListCollectionView(new List<DetailedSetlistPositionItemViewModel>());
        /// <summary>
        /// This field provides a <see cref="ListCollectionView"/> for the UI and further registers current-changed-Events
        /// </summary>
        public ListCollectionView SetlistPositionsView
        {
            get => _setlistPositionsView;
            private set => Set(ref _setlistPositionsView, value, nameof(SetlistPositionsView));
        }

        #endregion

        #region SetlistPositionsAreLoading
        /// <summary>
        /// This field stores the SetlistPositionsAreLoading value
        /// </summary>
        /// <remarks>This value indicates whether the view is currently loading the SetlistPositions</remarks>
        private bool _setlistPositionsAreLoading;
        ///<summary>
        /// This property provides the SetlistPositionsAreLoading-Value stored in <see cref="_setlistPositionsAreLoading"/> for eventual Binding in xaml.
        /// </summary>
        public bool SetlistPositionsAreLoading
        {
            get => _setlistPositionsAreLoading;
            set => Set(ref _setlistPositionsAreLoading, value, nameof(SetlistPositionsAreLoading));
        }
        #endregion

        #endregion

        #region Interface Functions for IElementContainer
        /// <see cref="IElementContainer{T}.GetElement()"/>
        public Setlist GetElement() => _setlist;
        #endregion

        #region Interface Functions for IDropTarget
        /// <see cref="IDropTarget.DragOver"/>
        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }

        /// <see cref="IDropTarget.Drop"/>
        public void Drop(IDropInfo dropInfo)
        {
            EnsureNoAudioPlaying();
            var source = dropInfo.Data as DetailedSetlistPositionItemViewModel;
            var target = dropInfo.TargetItem as DetailedSetlistPositionItemViewModel;

            new AsyncTask<bool>().OnExecute(() =>
            {
                return (source != null && target != null) && 
                       _setlistPositionMover.MovePosition(
                    source.GetElement(), 
                        target.GetDataPositionValue()
                    );
            }).OnDone(moveSuccess =>
            {
                if (moveSuccess)
                {
                    LoadSetlistPositions();
                }
            }).Start();
        }
        #endregion

        #region Functions

        /// <summary>
        /// This function enforces a reload of the data
        /// </summary>
        public void ApplyData()
        {
            AssignSetlistDataToViewModel();
            LoadSetlistDuration();
            LoadSetlistPositions();
        }

        /// <summary>
        /// This function exports <see cref="_setlist"/> into a Text-File at the given <see cref="filePath"/>
        /// </summary>
        /// <param name="filePath">The Path where the <see cref="_setlist"/> should be saved</param>
        /// <param name="onDone">A Callback to be executed when Export is done</param>
        [Obsolete]
        public void ExportSetlist(string filePath, Action<bool> onDone)
        {
            EnsureNoAudioPlaying();
            new AsyncTask<bool>().OnExecute(() =>
            {
                try
                {
                    var textualSongs = SetlistPositionVMs
                        .Select(vm => $"{vm.Position}) {vm.SongArtist} - {vm.SongTitle} ({vm.SongTempo},{vm.SongMusicKey})");
                    var setlistTextContent = $"{_setlist.Title}\n\n{_setlist.Description}\n\n" +
                                             $"{string.Join("\n",textualSongs)}\n\n{SetListDurationString}";
                    File.WriteAllText(filePath,setlistTextContent);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return false;
                }
            }).OnDone(onDone).Start();
        }

        /// <summary>
        /// This function ensures that no audio is playing by stopping the audio player
        /// </summary>
        public void EnsureNoAudioPlaying() => _audioPlayer.Stop();

        #endregion

        #region Help functions
        /// <summary>
        /// This function applies all setlist-specific data (without setlistpositions)
        /// data-values from <see cref="_setlist"/> to the VM-Fields
        /// </summary>
        private void AssignSetlistDataToViewModel() => _setlist.NotNull(setlistNn =>
        {
            SetListDescription = setlistNn.Description;
            SetListTitle = setlistNn.Title;
        });

        /// <summary>
        /// This function configures the commands of this VM
        /// </summary>
        private void ConfigureCommands()
        {
            DeleteCommand = new DelegateCommand<object, bool>(_ => _setlistDataService.Remove(_setlist));
        }

        /// <summary>
        /// This function loads the duration of a setlist and assigns it to <see cref="Duration"/>
        /// </summary>
        private void LoadSetlistDuration()
        {
            new AsyncTask<long>()
                .OnExecute(() => new SetlistDurationCalculator().CalculateSetlistDuration(_setlistPositionDataService))
                .OnDone(duration => Duration = duration)
                .Start();
        }

        /// <summary>
        /// This function loads all setlistPosition and applies them to the listcollectionview
        /// </summary>
        private void LoadSetlistPositions()
        {
            SetlistPositionsAreLoading = true;
            new AsyncTask<List<SetlistPosition>>()
                .OnExecute(() => _setlistPositionDataService.GetAll())
                .OnDone(result =>
                {
                    SetlistPositionsAreLoading = false;
                    SetlistPositionVMs = new ObservableCollection<DetailedSetlistPositionItemViewModel>(
                        result.Select(it =>
                        {
                            var vm = new DetailedSetlistPositionItemViewModel(it,_setlistPositionDataService,_audioPlayer);
                            vm.RemoveFromSetlistCommand.OnDone(removeIndex =>
                            {
                                vm.EnsureNoAudioPlaying();
                                if (removeIndex >= 0)
                                {
                                    LoadSetlistDuration();
                                    SetlistPositionVMs.RemoveAt(removeIndex);
                                }
                            });
                            return vm;
                        })
                    );
                    
                })
                .Start();
        }
        #endregion

    }

    
}
