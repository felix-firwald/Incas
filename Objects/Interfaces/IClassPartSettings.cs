using Incas.Core.ViewModels;
using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Incas.Objects.Interfaces
{
    public interface IClassPartSettings
    {
        public delegate void OpenAdditionalSettings(IClassDetailsSettings settings);
        public event OpenAdditionalSettings OnAdditionalSettingsOpenRequested;

        /// <summary>
        /// Visible name for tab item
        /// </summary>
        public string ItemName { get; }

        /// <summary>
        /// Incas calls this method when setting the UI Part of class by type
        /// <para>Please set View-Model here!</para>
        /// </summary>
        /// <param name="obj"></param>
        public IClassPartSettings SetUp(IMembersContainerViewModel classViewModel);

        /// <summary>
        /// Incas calls this method when class window is going to save all class data and close itself
        /// </summary>
        public void Validate();

        /// <summary>
        /// Incas calls this method when class window is going to save all class data and close itself
        /// </summary>
        public void Save();
    }
}
