using System.ComponentModel;
using System.Runtime.CompilerServices;
using Raven.NET.Core.Annotations;

namespace Raven.NET.Core.Subjects
{
    public class RavenSubject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}