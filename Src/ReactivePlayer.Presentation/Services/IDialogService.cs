using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Presentation.Services
{
    public interface IDialogService
    {
        //Task<TResult> ShowMessageBox<TResult>(string caption, string message);
        void ShowNotification();
        // TODO: does it make sense to wrap it in a task? is the dialog hosted in a separate thread that makes it already independent from the main UI thread?
        Task<DialogResult<IReadOnlyList<string>>> OpenFileDialog(
            string initialDirectoryPath,
            bool isMultiselectAllowed,
            IReadOnlyList<Tuple<IReadOnlyList<string>, string>> allowedExtensionsWithLabels,
            string title);
    }

    public class DialogResult<TResult>
    {
        public DialogResult(bool? code, TResult content)
        {
            this.Code = code;
            this.Content = content;
        }

        public bool? Code { get; }

        public TResult Content { get; }
    }
}