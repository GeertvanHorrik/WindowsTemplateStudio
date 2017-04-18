﻿using System.Windows.Media.Imaging;

using Microsoft.Templates.Core;
using Microsoft.Templates.Core.Mvvm;

namespace Microsoft.Templates.UI.ViewModels
{
    public class MetadataInfoViewModel : Observable
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private BitmapImage _icon;
        public BitmapImage Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value); }
        }

        public MetadataInfoViewModel(MetadataInfo metadataInfo)
        {
            if (metadataInfo == null)
            {
                return;
            }

            Name = metadataInfo.Name;
            DisplayName = metadataInfo.DisplayName;
            Description = metadataInfo.Description;
            Icon = Extensions.CreateIcon(metadataInfo.Icon);
        }
    }
}
