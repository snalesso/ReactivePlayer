using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.Services
{
    public interface IDialogService
    {
        Task<DialogResult<IReadOnlyList<string>>> OpenFileDialogAsync(
            string title,
            string initialDirectoryPath,
            bool isMultiselectAllowed,
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> filters);

        Task ShowWindowAsync(object dataContext);

        Task<bool?> ShowDialogAsync(object dataContext);
    }
}