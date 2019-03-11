using CSCore.Codecs;
using CSCore.SoundOut;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace CSCore.SimpleControlsSync
{
    public class ShellViewModel : ReactiveObject, IDisposable
    {
        private CompositeDisposable _disposables = new CompositeDisposable();
        private ISoundOut _soundOut;
        //CSCorePlayer _csCorePlayer = new CSCorePlayer();

        public ShellViewModel()
        {
            this.DoNothing = ReactiveCommand.Create(() => { })
                .DisposeWith(this._disposables);
            this.Load = ReactiveCommand.Create(
                (Uri location) =>
                {
                    if (location == null)
                    {
                        location = new Uri(@"D:\Music\Boyce Avenue - Jumper (song by Third Eye Blind).mp3");
                    }

                    if (this._soundOut != null)
                    {
                        this._soundOut.Dispose();
                        this._soundOut = null;
                    }

                    this._soundOut = new WasapiOut().DisposeWith(this._disposables);
                    var codec = CodecFactory.Instance.GetCodec(location).DisposeWith(this._disposables);
                    this._soundOut.Initialize(codec);

                    this._soundOut.Volume = 0.5f;
                })
                .DisposeWith(this._disposables);
            this.PlayFile = ReactiveCommand.Create(
                (Uri location) =>
                {
                    //if (location == null)
                    //{
                    //    location = new Uri(@"D:\Music\Boyce Avenue - Jumper (song by Third Eye Blind).mp3");
                    //}

                    //this._csCorePlayer.Play(location.LocalPath);
                })
                .DisposeWith(this._disposables);

            this.Play = ReactiveCommand.Create(
                () =>
                {
                    if (this._soundOut != null)
                    {
                        this._soundOut.Play();
                    }
                })
                .DisposeWith(this._disposables);

            this.Pause = ReactiveCommand.Create(
                () =>
                {
                    if (this._soundOut != null)
                    {
                        this._soundOut.Pause();
                    }
                })
                .DisposeWith(this._disposables);

            this.Resume = ReactiveCommand.Create(
                () =>
                {
                    if (this._soundOut != null)
                    {
                        this._soundOut.Resume();
                    }
                })
                .DisposeWith(this._disposables);

            this.Stop = ReactiveCommand.Create(
                () =>
                {
                    if (this._soundOut != null)
                    {
                        this._soundOut.Stop();
                        this._soundOut.WaitForStopped();
                    }
                })
                .DisposeWith(this._disposables);
        }

        public ReactiveCommand<Unit, Unit> DoNothing { get; }
        public ReactiveCommand<Uri, Unit> Load { get; }
        public ReactiveCommand<Uri, Unit> PlayFile { get; }
        public ReactiveCommand<Unit, Unit> Play { get; }
        public ReactiveCommand<Unit, Unit> Pause { get; }
        public ReactiveCommand<Unit, Unit> Resume { get; }
        public ReactiveCommand<Unit, Unit> Stop { get; }

        public void Dispose()
        {
            this._disposables.Dispose();
        }
    }
}