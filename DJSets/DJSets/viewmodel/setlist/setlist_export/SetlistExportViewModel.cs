using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using DJSets.clerks.export;
using DJSets.model.export;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.util.mvvm;
using DJSets.util.mvvm.validation;
using DJSets.viewmodel.basics;
using DJSets.viewmodel.setlist.setlist_detail;

namespace DJSets.viewmodel.setlist.setlist_export
{
    /// <summary>
    /// This class defines a viewmodel for the SetlistExportView
    /// </summary>
    public class SetlistExportViewModel : ValidatableViewModel
    {

        #region Constructors
        public SetlistExportViewModel(SetlistDetailViewModel setlistDetailVm)
        {
            _setlistVm = setlistDetailVm;
            ConfigCommands();
            ApplyData();
        }
        #endregion

        #region Commands
        /// <summary>
        /// This command defines Cancel-Logic
        /// </summary>
        public DelegateCommand<object,bool> CancelCommand { get; set; }

        /// <summary>
        /// This command defines Export-Logic
        /// </summary>
        public DelegateCommand<object, bool> ExportCommand { get; set; }
        #endregion

        #region Fields

        #region Clerks
        /// <summary>
        /// This clerk should validate the fields of this viewmodel
        /// </summary>
        private readonly SetlistExportViewModelValidator _validator = new SetlistExportViewModelValidator();
        #endregion

        #region _setlist
        /// <summary>
        /// This field allows access to the Setlist to be exported
        /// </summary>
        private readonly SetlistDetailViewModel _setlistVm;
        #endregion

        #region DirectoryPath
        /// <summary>
        /// This field stores the DirectoryPath value
        /// </summary>
        private string _directoryPath = string.Empty;
        ///<summary>
        /// This property provides the DirectoryPath-Value stored in <see cref="_directoryPath"/> for eventual Binding in xaml.
        /// </summary>
        public string DirectoryPath
        {
            get => _directoryPath;
            set => Set((val,propName) =>
            {
                var errMsg = Application.Current.GetResource<string>(StringResourceKeys.StrDirectoryPathCanOnlyBeExistingFile);
                return SetError(() => _validator.IsValidDirectoryPath(val), propName, errMsg);
            },ref _directoryPath, value, nameof(DirectoryPath), _ => ExportCommand.NotifyCanExecuteChanged());
        }
        #endregion

        #region FileName
        /// <summary>
        /// This field stores the FileName value
        /// </summary>
        private string _fileName = string.Empty;
        ///<summary>
        /// This property provides the FileName-Value stored in <see cref="_fileName"/> for eventual Binding in xaml.
        /// </summary>
        public string FileName
        {
            get => _fileName;
            set => Set((val, propName) =>
            {
                var errMsg = Application.Current.GetResource<string>(StringResourceKeys.StrFileNameCannotBeEmptyOrBlank);
                return SetError(() => _validator.IsValidFileName(val), propName, errMsg);
            },ref _fileName, value, nameof(FileName), _ => ExportCommand.NotifyCanExecuteChanged());
        }
        #endregion

        #region AllSetlistExportTypeFileExtensionNames
        /// <summary>
        /// This field provides titles for all possible SetlistExportTypes
        /// </summary>
        public IEnumerable<string> AllSetlistExportTypeFileExtensionNames =>
            SetlistExportTypeUtils
                .AllValues()
                .Select(it => it.FileExtensionName());
        #endregion

        #region SelectedExportType
        /// <summary>
        /// This field stores the SelectedExportType value
        /// </summary>
        private SetlistExportType _selectedExportType = SetlistExportTypeUtils.SetlistExportTypeDefault();
        ///<summary>
        /// This property provides the SelectedExportType-Value stored in <see cref="_selectedExportType"/> for eventual Binding in xaml.
        /// </summary>
        public SetlistExportType SelectedExportType
        {
            get => _selectedExportType;
            set
            {
                Set(ref _selectedExportType, value, nameof(SelectedExportType));
                OnPropertyChanged(nameof(ExportNote));
            }
        }
        #endregion

        #region ExportNote
        /// <summary>
        /// This field provides a Note for <see cref="SelectedExportType"/>
        /// </summary>
        public string ExportNote => SelectedExportType.ExportNote();

        #endregion

        #endregion

        #region Functions
        /// <summary>
        /// This function creates the full, absolute FilePath out of the the given fields
        /// </summary>
        /// <returns>The full, absolute filepath where the Setlist-Data should be exported:</returns>
        public string GetAbsoluteFilePath() =>
            Path.Combine(DirectoryPath, $"{FileName}.{SelectedExportType.FileExtensionName()}");

        #endregion

        #region Help functions

        /// <summary>
        /// This function applies Data of <see cref="_setlistVm"/> to the VM-Fields
        /// </summary>
        private void ApplyData() => FileName = _setlistVm.SetListTitle;

        /// <summary>
        /// This function configures the commands of this ViewModel
        /// </summary>
        private void ConfigCommands()
        {
            //CancelCommand
            CancelCommand = new DelegateCommand<object, bool>(_ => true);

            //ExportCommand
            ExportCommand = new DelegateCommand<object, bool>(_ =>
            {
                IExporter<SetlistDetailViewModel> exporter;
                switch (SelectedExportType)
                {
                    case SetlistExportType.Txt:
                        exporter = new SetlistDetailViewModelTxtExporter(GetAbsoluteFilePath());
                        break;
                    case SetlistExportType.M3U:
                        exporter = new SetlistDetailViewModelM3UExporter(GetAbsoluteFilePath());
                        break;
                    default:
                        return false; 
                }

                return exporter.Export(_setlistVm);

            }, _ => _validator.IsValid(this));
        }
        #endregion
    }
}
