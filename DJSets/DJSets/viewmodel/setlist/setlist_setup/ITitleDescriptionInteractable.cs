using DJSets.viewmodel.vm_util.titledescription;

namespace DJSets.viewmodel.setlist.setlist_setup
{
    /// <summary>
    /// This Interface defines the functionality that is necessary to interact with a TitleDescriptionView
    /// </summary>
    interface ITitleDescriptionInteractable
    {
        /// <summary>
        /// This Function defines a Setter for the TitleDescription-DataSource (TitleDescriptionViewModel) to get Data from it
        /// </summary>
        /// <param name="vm">new ViewModel-Instance to be set to get title and description from</param>
        public void SetTitleDescriptionSource(TitleDescriptionViewModel vm);
    }
}
