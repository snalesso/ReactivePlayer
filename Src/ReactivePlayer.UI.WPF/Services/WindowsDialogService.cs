using Caliburn.Micro;
using Microsoft.Win32;
using ReactivePlayer.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.Services
{
    public sealed class WindowsDialogService : IDialogService
    {
        private readonly IWindowManager _windowManager;

        public WindowsDialogService(IWindowManager windowManager)
        {
            this._windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
        }

        public DialogResult<IReadOnlyList<string>> OpenFileDialog(
            string title,
            string initialDirectoryPath,
            bool isMultiselectAllowed,
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> filters)
        {
            var ofd = new OpenFileDialog()
            {
                Filter = string.Join("|", filters?.Select(kvp => kvp.Key + "|(" + string.Join(", ", kvp.Value.Select(ext => $"*.{ext}")) + ")")),
                Multiselect = isMultiselectAllowed,
                InitialDirectory = initialDirectoryPath,
                Title = title
            };

            var resultCode = ofd.ShowDialog();

            return new DialogResult<IReadOnlyList<string>>(resultCode, resultCode == true ? ofd.FileNames : null);
        }

        public void ShowDialog(object dataContext)
        {
            this._windowManager.ShowDialog(dataContext);
        }

        public void ShowWindow(object dataContext)
        {
            this._windowManager.ShowWindow(dataContext);
        }
    }
}