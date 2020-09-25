using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReadTags.ViweModels;

namespace ReadTags.ViweModels {
    class AboundBoxWindowViewModel {

        public ReactiveProperty<FileVersionInfo> FileVersionInfo { set; get; }
            = new ReactiveProperty<FileVersionInfo>();

        public AboundBoxWindowViewModel() {
            string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            FileVersionInfo.Value = System.Diagnostics.FileVersionInfo.GetVersionInfo(location);
        }
    }
}
