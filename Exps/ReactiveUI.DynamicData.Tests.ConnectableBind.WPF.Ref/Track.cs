using System;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind.WPF
{
    public class Track
    {
        public uint Id { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public DateTime AddedToLibraryDateTime { get; set; }
    }
}