namespace ReactivePlayer.UI.Services
{
    public class DialogResult<TResult>
    {
        public DialogResult(bool? isConfirmed, TResult content)
        {
            this.IsConfirmed = isConfirmed;
            this.Content = content;
        }

        public bool? IsConfirmed { get; }

        public TResult Content { get; }
    }
}