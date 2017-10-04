using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Presentation.Services
{
    public sealed class WindowsDialogService : IDialogService
    {
        public async Task<DialogResult<IReadOnlyList<string>>> OpenFileDialog(
            string initialDirectoryPath,
            bool isMultiselectAllowed,
            IReadOnlyList<Tuple<IReadOnlyList<string>, string>> allowedExtensionsWithLabels,
            string title)
        {
            var ofd = new OpenFileDialog()
            {
                Filter = string.Join("|", allowedExtensionsWithLabels?.Select(tuple => tuple.Item2 + "|(" + string.Join(", ", tuple.Item1.Select(ext => "*." + ext)) + ")")),
                Multiselect = isMultiselectAllowed,
                InitialDirectory = initialDirectoryPath,
                Title = title
            };

            var resultCode = await Task.Run(() => ofd.ShowDialog());

            return new DialogResult<IReadOnlyList<string>>(resultCode, resultCode == true ? ofd.FileNames : null);
        }

        public void ShowNotification()
        {
            throw new NotImplementedException();
        }
    }
}