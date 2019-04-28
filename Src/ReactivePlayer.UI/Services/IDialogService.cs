using System.Collections.Generic;

namespace ReactivePlayer.UI.Services
{
    public interface IDialogService
    {
        DialogResult<IReadOnlyList<string>> OpenFileDialog(
            string title,
            string initialDirectoryPath,
            bool isMultiselectAllowed,
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> filters);

        void ShowWindow(object dataContext);

        void ShowDialog(object dataContext);
    }
}