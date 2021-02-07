
using System.Windows;
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

namespace DJSets.viewmodel.setlist.setlist_item
{
    /// <summary>
    /// This class defines ViewModel-Logic for an item in the Setlist-Menu
    /// </summary>
    public class OverviewSetlistItemViewModel : BaseViewModel,IElementContainer<Setlist>
    {
        #region Constructors
        public OverviewSetlistItemViewModel(Setlist setlist)
        {
            _setlist = setlist;
            _setlistPositionDataService = new EfSqliteSetlistPositionDataService(_setlist);
            LoadSetlistDuration();

            //register observers
            ModelNotificationCenter.SongNotifier.RegisterObserver(LoadSetlistDuration);
        }
        #endregion

        #region Finalizers

        ~OverviewSetlistItemViewModel()
        {
            ModelNotificationCenter.SongNotifier.UnRegisterObserver(LoadSetlistDuration);
        }

        #endregion
        #region Fields

        #region Clerks
        /// <summary>
        /// This clerk should format the DurationString of <see cref="_setlist"/> to be correctly displayed as a String
        /// </summary>
        private readonly TimeFormatConverter _timeFormatConverter = new TimeFormatConverter();

        /// <summary>
        /// This clerk is necessary to determine the length of the setlist
        /// </summary>
        private readonly IDataService<SetlistPosition> _setlistPositionDataService;

        #endregion

        /// <summary>
        /// This field contains the Setlist for which this ViewModel provides Information 
        /// </summary>
        private readonly Setlist _setlist;

        /// <summary>
        /// This field provides a title for a Setlist
        /// </summary>
        public string Title
        {
            get => _setlist.Title;
        }


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
                OnPropertyChanged(nameof(DurationString));
            }
        }
        #endregion

        /// <summary>
        /// This field provides the Setlists DurationString as a formatted String
        /// </summary>
        public string DurationString
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

        #region Interface Functions
        /// <see cref="IElementContainer{T}.GetElement()"/>
        public Setlist GetElement() => _setlist;
        #endregion

        #region Help Functions
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

        #endregion
    }
}
