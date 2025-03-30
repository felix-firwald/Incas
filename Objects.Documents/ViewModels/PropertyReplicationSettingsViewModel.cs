using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents.PropertyReplicator;

namespace Incas.Objects.Documents.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class PropertyReplicationSettingsViewModel : BaseViewModel
    {
        /// <summary>
        /// Model
        /// </summary>
        public TemplateProperty Source { get; set; }

        public PropertyReplicationSettingsViewModel(TemplateProperty source)
        {
            this.Source = source;
            if (this.Source.Replicator is null)
            {
                this.Source.Replicator = new();
            }
        }
        public string TitleName
        {
            get
            {
                return $"Настройка свойства [{this.Source.Name}]";
            }
        }
        public static List<ReplicationSourceType> AvailableSourceTypes
        {
            get
            {
                return
                [
                    ReplicationSourceType.Field,
                    ReplicationSourceType.Property
                ];
            }
        }
        public static List<ReplicationTargetType> AvailableRenderTypes
        {
            get
            {
                return
                [
                    ReplicationTargetType.Text, 
                    ReplicationTargetType.FormattedText, 
                    ReplicationTargetType.QRCode, 
                    ReplicationTargetType.Image
                ];
            }
        }
        public ReplicationSourceType SourceType
        {
            get
            {
                return this.Source.Replicator.SourceType;
            }
            set
            {
                this.Source.Replicator.SourceType = value;
                this.OnPropertyChanged(nameof(this.SourceType));
            }
        }
        public ReplicationTargetType RenderType
        {
            get
            {
                return this.Source.Replicator.RenderType;
            }
            set
            {
                this.Source.Replicator.RenderType = value;
                this.OnPropertyChanged(nameof(this.RenderType));
            }
        }

        /// <summary>
        /// Сохраняет изменения в Model
        /// </summary>
        public bool Save()
        {
            return true;
        }
    }
}
