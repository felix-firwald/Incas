using Incas.DialogSimpleForm.Components;
using IncasEngine.ObjectiveEngine.FieldComponents;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.AutoUI
{
    public class FloatFieldSettings : BaseFieldSettings
    {
        #region Data
        [Description("Минимальное значение")]
        public int MinValue { get; set; }

        [Description("Значение по-умолчанию")]
        public int DefaultValue { get; set; }

        [Description("Максимальное значение")]
        public int MaxValue { get; set; }

        [Description("Тип формата")]
        public Selector Format { get; set; }
        #endregion

        public FloatFieldSettings(Field field)
        {
            this.Source = field;
            this.GetBaseData();
            this.Format = new(new()
            {
                { NumberFieldData.FormatNumber.General, "Обычный"  },
                { NumberFieldData.FormatNumber.FixedPoint, "С фиксированной запятой"  },
                { NumberFieldData.FormatNumber.Currency, "Валюта"  },
                { NumberFieldData.FormatNumber.Number, "Номер"  },
                { NumberFieldData.FormatNumber.Percent, "Процент"  },
            });
            try
            {
                NumberFieldData nf = field.GetNumberFieldData();
                this.MinValue = nf.MinValue;
                this.DefaultValue = nf.DefaultValue;
                this.MaxValue = nf.MaxValue;
                this.Format.SetSelection(nf.Format);
            }
            catch
            {

            }
        }

        #region Functionality
        public override void Save()
        {
            this.SaveBaseData();
            NumberFieldData nf = new()
            {
                MinValue = this.MinValue,
                MaxValue = this.MaxValue,
                DefaultValue = this.DefaultValue,
                Format = (NumberFieldData.FormatNumber)this.Format.SelectedObject
            };
            this.Source.SetNumberFieldData(nf);
            //this.Source.Value = JsonConvert.SerializeObject(nf);
        }
        #endregion
    }
}
