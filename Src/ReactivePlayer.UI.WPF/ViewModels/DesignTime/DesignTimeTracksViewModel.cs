using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ReactivePlayer.UI.WPF.ViewModels.DesignTime
{
    internal class DesignTimeTracksViewModel : TracksViewModel
    {
        public DesignTimeTracksViewModel()
            : base(
                  null
                  , null
                  //, null
                  //, null
                  //, null
                  //, null
                  //, null
                  )
        //new LocalLibraryService(new FakeTracksInMemoryRepository()),
        //new FakePlaybackService(),
        //t => new TrackViewModel(t, new FakePlaybackService()))
        {
            //this.ReloadTracks.Execute();
        }

        //protected override IReadOnlyList<Tuple<Expression<Func<TrackViewModel, object>>, Predicate<TrackViewModel>>> Filters => throw new NotImplementedException();
    }
}