using Microsoft.WindowsAPICodePack.Shell;
using System;

namespace ReactivePlayer.Core.Application.Services.FileSystem
{
    public static class ShellObjectHelper
    {
        public static TimeSpan? GetMediaFileDuration(string mediafilePath)
        {
            TimeSpan? duration = null;

            // Media file duration calculation using Shell done on Mark Heath's tip: http://markheath.net/post/how-to-get-media-file-duration-in-c

            // TODO: benchmark shell duration calculation vs TagLib.Tag.Duration
            using (var shell = ShellObject.FromParsingName(mediafilePath))
            {
                //IShellProperty idsp = shell.Properties.System.Media.Duration;
                var dsp = shell.Properties.System.Media.Duration?.Value;
                if (dsp != null && dsp.HasValue)
                {
                    //var id = (ulong)idsp.ValueAsObject;
                    //duration = TimeSpan.FromTicks((long)id);

                    var d = (ulong)dsp.Value;
                    // TODO: review implicit vs explicit cast: https://stackoverflow.com/questions/15394032/difference-between-casting-and-using-the-convert-to-method
                    duration = TimeSpan.FromTicks(Convert.ToInt64(d));
                    //duration = TimeSpan.FromTicks((long)d);
                }
            }

            return duration;
        }
    }
}