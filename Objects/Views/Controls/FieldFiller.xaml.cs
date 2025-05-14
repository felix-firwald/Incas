using Incas.Core.Classes;
using Incas.Core.Extensions;
using Incas.Core.Views.Controls;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Components;
using Incas.Objects.Interfaces;
using IncasEngine.Core;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.FieldComponents;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.Scripting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.ApplicationModel.Background;
using Windows.Media.Protection.PlayReady;
using Xceed.Wpf.Toolkit;
using static Incas.Objects.Interfaces.IFillerBase;
using static IncasEngine.ObjectiveEngine.Models.State;

namespace Incas.Objects.Views.Controls
{
    public static class CommandIcons
    {
        public static Geometry ConvertFromString(string input)
        {
            return (Geometry)TypeDescriptor.GetConverter(typeof(Geometry)).ConvertFrom(input);
        }
    }
    /// <summary>
    /// Логика взаимодействия для FieldFiller.xaml
    /// </summary>
    public partial class FieldFiller : UserControl, ISimpleFiller
    {
        public Field Field { get; set; }
        public event RoutedEventHandler OnCustomButtonClicked;
        private bool unique = true;
        private bool isRequired;
        private Control control;
        public delegate void CommandScript(string script);
        public event StringAction OnInsert;  
        public event CommandScript OnScriptRequested;
        public event AutoRunMethod OnFillerUpdate;
        public event FillerUpdate OnDatabaseObjectCopyRequested;
        public FieldFiller(Field f)
        {
            this.InitializeComponent();
            this.Field = f;
            this.MainLabel.Text = string.IsNullOrWhiteSpace(this.Field.VisibleName) ? this.Field.Name + ":" : this.Field.VisibleName + ":";
            try
            {
                this.GenerateUIControl(this.Field.Type);
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }
        public void MarkAsNotValidated()
        {
            this.Grid.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 40, 45));
        }
        public void MarkAsValidated()
        {
            this.Grid.Background = null;
        }
        private FieldType GetFillerType()
        {
            return this.Field.Type;
        }
        private void GenerateUIControl(FieldType type)
        {
            string value = this.Field.Value;
            string description = this.Field.Description;
            switch (type)
            {
                case FieldType.String:                  
                    if (this.Field.Mask is null)
                    {
                        System.Windows.Controls.TextBox textBox = new()
                        {
                            Text = value,
                            Tag = description
                        };
                        textBox.TextChanged += this.Textbox_TextChanged;
                        textBox.Style = ResourceStyleManager.FindStyle(ResourceStyleManager.TextBoxStyle);
                        textBox.MaxLength = this.Field.GetMaxLength();
                        this.PlaceUIControl(textBox);
                    }
                    else
                    {
                        MaskedTextBox masked = new()
                        {
                            Text = value,
                            Mask = this.Field.Mask,
                            MaxLength = this.Field.GetMaxLength(),
                            Watermark = description
                        };
                        masked.TextChanged += this.Textbox_TextChanged;
                        this.PlaceUIControl(masked);
                    }
                    
                    break;
                case FieldType.Text:
                    System.Windows.Controls.TextBox textBox2 = new()
                    {
                        Text = value,
                        Tag = description
                    };
                    textBox2.TextChanged += this.Textbox_TextChanged;
                    textBox2.Style = ResourceStyleManager.FindStyle(ResourceStyleManager.TextBoxBigStyle);
                    textBox2.MaxLength = this.Field.GetMaxLength();
                    this.PlaceUIControl(textBox2);
                    break;
                case FieldType.LocalEnumeration:
                case FieldType.GlobalEnumeration:
                    value ??= "";
                    ComboBox comboBox = new()
                    {
                        ItemsSource = type == FieldType.LocalEnumeration ? this.Field.GetLocalEnumeration() : ProgramState.GetEnumeration(this.Field.GetGlobalEnumeration().TargetId),
                        SelectedIndex = 0,
                        Style = ResourceStyleManager.FindStyle(ResourceStyleManager.ComboboxStyle)
                    };
                    comboBox.SelectionChanged += this.Combobox_SelectionChanged;
                    this.PlaceUIControl(comboBox);
                    break;
                case FieldType.Integer:
                    IntegerUpDown numeric = new();                  
                    numeric.Style = ResourceStyleManager.FindStyle(ResourceStyleManager.IntegerUpDownStyle);
                    numeric.ToolTip = description;
                    numeric.ValueChanged += this.Numeric_ValueChanged;
                    NumberFieldData intData = this.Field.GetNumberFieldData();
                    if (intData is null)
                    {
                        this.PlaceUIControl(this.GenerateErrorLabel());
                        return;
                    }
                    numeric.Minimum = intData.MinValue;
                    numeric.Value = intData.DefaultValue;
                    numeric.Maximum = intData.GetMaxValue();                 
                    this.PlaceUIControl(numeric);
                    break;
                case FieldType.Float:
                    DoubleUpDown numericFloat = new();
                    numericFloat.Style = ResourceStyleManager.FindStyle(ResourceStyleManager.IntegerUpDownStyle);
                    numericFloat.ToolTip = description;
                    numericFloat.ValueChanged += this.Numeric_ValueChanged;
                    NumberFieldData floatData = this.Field.GetNumberFieldData();
                    if (floatData is null)
                    {
                        this.PlaceUIControl(this.GenerateErrorLabel());
                        return;
                    }
                    numericFloat.Minimum = floatData.MinValue;
                    numericFloat.Value = floatData.DefaultValue;
                    numericFloat.Maximum = floatData.GetMaxValue();
                    numericFloat.Increment = 0.100;
                    numericFloat.FormatString = floatData.GetFormat();
                    this.PlaceUIControl(numericFloat);
                    break;
                case FieldType.Object:
                    SelectionBox selectionBox = new(this.Field.GetBindingData());
                    selectionBox.OnValueChanged += this.SelectionBox_OnValueChanged;
                    this.PlaceUIControl(selectionBox);
                    break;
                case FieldType.Date:
                    DatePicker picker = new()
                    {
                        ToolTip = description,
                        Style = ResourceStyleManager.FindStyle(ResourceStyleManager.DatePickerStyle)
                    };
                    DateFieldData df = this.Field.GetDateFieldData();
                    picker.DisplayDateStart = df.StartDate;
                    picker.DisplayDateEnd = df.EndDate;
                    picker.SelectedDateChanged += this.DatePicker_SelectedDateChanged;
                    this.PlaceUIControl(picker);
                    break;
                case FieldType.Time:
                    TimePicker tPicker = new()
                    {
                        ToolTip = description,
                        Margin = new Thickness(5)
                    };
                    this.PlaceUIControl(tPicker);
                    break;
                case FieldType.Boolean:
                    CheckBox checkBox = new() {
                        ToolTip = description,
                        Content = this.Field.VisibleName,
                        IsThreeState = false,
                        Style = ResourceStyleManager.FindStyle(ResourceStyleManager.CheckboxStyle)
                    };
                    this.PlaceUIControl(checkBox, true);
                    break;
            }
        }
        private Label GenerateErrorLabel()
        {
            Label l = new();
            l.Content = "<поле класса не настроено>";
            l.FontFamily = ResourceStyleManager.FindFontFamily(ResourceStyleManager.FontRubik);
            l.Foreground = (new IncasEngine.Core.Color() { R = 255, G = 120, B = 120 }).AsBrush();
            return l;
        }
        private void PlaceUIControl(Control control, bool withoutLabel = false)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.control = control;
                this.control.ToolTip = this.Field.Description;
                this.Grid.Children.Add(control);
                if (withoutLabel)
                {
                    this.HideLabel();
                }
                else
                {
                    Grid.SetRow(control, 0);
                    Grid.SetColumn(control, 1);
                }                
            });            
        }
        public void HideLabel()
        {
            Grid.SetRow(control, 0);
            Grid.SetColumn(control, 0);
            Grid.SetRowSpan(control, 1);
            this.MainLabel.Visibility = Visibility.Collapsed;
        }

        private void MakeButton()
        {
            //this.command = this.Field.GetCommand();
            //if (this.command.ScriptType != ScriptType.Button)
            //{
            //    return;
            //}
            //this.CommandButtonIcon.Data = this.FindResource(this.command.Icon.ToString()) as PathGeometry;
            //this.CommandButton.Visibility = Visibility.Visible;
            //this.CommandButtonText.Content = this.command.Name;
        }

        public void SetValue(string value)
        {
            switch (this.GetFillerType())
            {
                case FieldType.String:
                case FieldType.Text:
                    ((System.Windows.Controls.TextBox)this.control).Text = value;
                    break;
                case FieldType.Integer:
                    int digitValue;
                    if (int.TryParse(value, out digitValue))
                    {
                        ((IntegerUpDown)this.control).Value = digitValue;
                    }
                    break;
                case FieldType.Float:
                    double doubleValue;
                    if (double.TryParse(value, out doubleValue))
                    {
                        ((DoubleUpDown)this.control).Value = doubleValue;
                    }
                    break;
                case FieldType.Object:
                    ((SelectionBox)this.control).SetObject(value);
                    break;
                case FieldType.LocalEnumeration:
                case FieldType.GlobalEnumeration:
                    ((ComboBox)this.control).SelectedValue = value;
                    break;
                case FieldType.Date:
                    this.SetDateTimeValue(value);
                    break;
                case FieldType.Time:
                    ((TimePicker)this.control).Text = value;
                    break;
#if E_BUSINESS
                case FieldType.Structure:
                        
                    break;
#endif
                case FieldType.Boolean:
                    bool resultBool = false;
                    if (bool.TryParse(value, out resultBool))
                    {
                        ((CheckBox)this.control).IsChecked = resultBool;
                    }                   
                    break;
            }     
        }

        private async void SetDateTimeValue(string value)
        {
            List<string> formats =
            [
                "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss", "dd.MM.yy", "«dd» MMMM yyyy"
            ];
            bool tryApply(string format)
            {
                DateTime parsedDate;
                bool success = DateTime.TryParseExact(value, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsedDate);
                if (success)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ((DatePicker)this.control).SelectedDate = parsedDate;
                    });                    
                }
                return success;
            }
            await Task.Run(() =>
            {
                foreach (string format in formats)
                {
                    if (tryApply(format))
                    {
                        return;
                    }
                }
            });         
        }

        public string GetTagName()
        {
            return this.Field.Name;
        }

        private string GetDateInFormat()
        {
            DatePicker picker = ((DatePicker)this.control);
            if (picker.SelectedDate is null)
            {
                if (this.isRequired == true)
                {
                    this.ThrowNotNullFailed();
                }
                return "";
            }
            DateFieldData df = this.Field.GetDateFieldData();
            string format = "dd.MM.yyyy";
            switch (df.Format)
            {
                case DateFormats.Usual:
                default:
                    break;
                case DateFormats.Full:
                    format = "dd MMMM yyyy";
                    break;
                case DateFormats.FullWithQuotes:
                    format = "«dd» MMMM yyyy";
                    break;
            }

            return picker.SelectedDate.HasValue ? ((DateTime)picker.SelectedDate).ToString(format) : "";
        }
        public string GetValue()
        {
            switch (this.GetFillerType())
            {
                case FieldType.String:
                case FieldType.Text:
                    string value = ((System.Windows.Controls.TextBox)this.control).Text;
                    if (this.isRequired == true && string.IsNullOrEmpty(value))
                    {
                        this.ThrowNotNullFailed();
                    }
                    if (this.Field.IsUnique && this.unique == false)
                    {
                        this.ThrowUniqueFailed();
                    }
                    return value;
                case FieldType.Integer:
                    return ((IntegerUpDown)this.control).Value.ToString();
                case FieldType.Float:
                    return ((DoubleUpDown)this.control).Value.ToString();
                case FieldType.Object:
                    return ((SelectionBox)this.control).Value;
                case FieldType.LocalEnumeration:
                case FieldType.GlobalEnumeration:
                    ComboBox cb = (ComboBox)this.control;
                    if (cb.SelectedIndex != -1)
                    {
                        return cb.Items.GetItemAt(cb.SelectedIndex).ToString();
                    }
                    if (this.isRequired == true)
                    {
                        this.ThrowNotNullFailed();
                    }
                    return "";
                case FieldType.Date:
                    return this.GetDateInFormat();
                case FieldType.Time:
                    TimePicker tp = this.control as TimePicker;
                    return tp.Text;
                case FieldType.Boolean:
                    return ((CheckBox)this.control).IsChecked.ToString();
                default:
                    return "<Критическая ошибка INCAS>";
                    //case FieldType.Generator:
                    //    return ((GeneratorFiller)this.control).GetText();
            }
        }
        /// <summary>
        /// Get an internal value of field
        /// </summary>
        /// <returns></returns>
        public string GetData()
        {
            switch (this.GetFillerType())
            {
                //case FieldType.Generator:
                //    return ((GeneratorFiller)this.control).GetData();
                case FieldType.Date:
                    if (((DatePicker)this.control).SelectedDate.HasValue)
                    {
                        return ((DateTime)((DatePicker)this.control).SelectedDate).ToString("dd.MM.yyyy");
                    }
                    else if (this.isRequired == true)
                    {
                        this.ThrowNotNullFailed();
                    }
                    return "";
                case FieldType.Object:
                    IObject obj = ((SelectionBox)this.control).SelectedObject;
                    if (this.isRequired == true && obj is null)
                    {
                        this.ThrowNotNullFailed();
                    }
                    return obj?.Id.ToString();
                default: return this.GetValue();
            }
        }
        public object GetDataForScript()
        {
            switch (this.GetFillerType())
            {
                case FieldType.Date:
                    if (((DatePicker)this.control).SelectedDate.HasValue)
                    {
                        return (DateTime)((DatePicker)this.control).SelectedDate;
                    }
                    return DateTime.MinValue;
                case FieldType.Time:
                    TimePicker tp = this.control as TimePicker;                    
                    return tp.Text;
                case FieldType.Object:
                    IObject obj = ((SelectionBox)this.control).SelectedObject;
                    if (obj is null)
                    {
                        return null;
                    }
                    return IncasPythonCommonTools.GetDynamic(obj);              
                case FieldType.Integer:
                    return ((IntegerUpDown)this.control).Value;
                case FieldType.Float:
                    return ((DoubleUpDown)this.control).Value;
                case FieldType.Boolean:
                    return (bool)(((CheckBox)this.control).IsChecked);
                case FieldType.LocalEnumeration:
                case FieldType.GlobalEnumeration:
                    ComboBox cb = (ComboBox)this.control;
                    if (cb.SelectedIndex != -1)
                    {
                        return cb.Items.GetItemAt(cb.SelectedIndex).ToString();
                    }
                    return "";
                case FieldType.String:
                default:
                    return ((System.Windows.Controls.TextBox)this.control).Text;
            }
        }
        public Dictionary<string, string> GetDataFromObjectRelation()
        {
            Dictionary<string, string> result = new();
            IObject obj = ((SelectionBox)this.control).SelectedObject;
            if (obj is null)
            {
                return result;
            }
            foreach (FieldData data in obj.Fields)
            {
                if (data.ClassField.Type == FieldType.Object)
                {
                    BindingData bd = data.ClassField.GetBindingData();
                    IObject recurObj = Processor.GetObject(EngineGlobals.GetClass(bd.BindingClass), Guid.Parse(data.Value.ToString()));
                    foreach (FieldData recdata in recurObj.Fields)
                    {
                        if (recdata.ClassField.Id == bd.BindingField)
                        {
                            result.Add($"{this.Field.Name}.{data.ClassField.Name}", recdata.Value.ToString());
                        }
                        result.Add($"{this.Field.Name}.{data.ClassField.Name}.{recdata.ClassField.Name}", recdata.Value.ToString());
                    }
                }
                else
                {
                    result.Add($"{this.Field.Name}.{data.ClassField.Name}", data.Value.ToString());
                }              
            }
            return result;
        }
        public void ThrowNotNullFailed()
        {
            this.MarkAsNotValidated();
            throw new NotNullFailed($"Поле \"{this.Field.VisibleName}\" является обязательным, однако не заполнено.");
        }
        public void ThrowUniqueFailed()
        {
            this.MarkAsNotValidated();
            throw new NotNullFailed($"Поле \"{this.Field.VisibleName}\" объявлено уникальным, однако такое значение уже встречается в базе данных.");
        }

        public Guid GetId()
        {
            return this.Field.Id;
        }

        private void CopyAllClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.GetValue());
        }

        private void InsertToOther(object sender, RoutedEventArgs e)
        {
            try
            {
                OnInsert?.Invoke(this.Field.Id, this.GetData());
            }
            catch (NotNullFailed)
            {
                DialogsManager.ShowExclamationDialog("Поле является обязательным, необходимо сначала присвоить ему значение.", "Переназначение прервано");
            }
        }

        private void TextRequest(object sender, RoutedEventArgs e)
        {

        }
        private void CommandClick(object sender, RoutedEventArgs e)
        {
            //this.PlayScript();
        }
        private void CheckForScriptOnUpdate()
        {
            //this.PlayScript(this.Field.ChangedEvent);
        }
        private void RunUpdateEvent()
        {
            this.MarkAsValidated();
            if (this.Field.OnUpdated != Guid.Empty)
            {
                
                this.OnFillerUpdate?.Invoke(this.Field.OnUpdated);
            }            
        }

        private void Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.RunUpdateEvent();
            this.CheckForScriptOnUpdate();
        }

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.RunUpdateEvent();
            this.CheckForScriptOnUpdate();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.RunUpdateEvent();
            this.CheckForScriptOnUpdate();
        }

        private void SelectionBox_OnValueChanged(object sender, TextChangedEventArgs e)
        {
            this.RunUpdateEvent();
            this.CheckForScriptOnUpdate();
        }

        private void Numeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.RunUpdateEvent();
            this.CheckForScriptOnUpdate();
        }

        private void ObjectCopyRequestClick(object sender, RoutedEventArgs e)
        {
            this.OnDatabaseObjectCopyRequested?.Invoke(this);
        }

        public void ApplyState(State state)
        {
            MemberState source = state.Settings[this.Field.Id];
            if (source.EditorVisibility)
            {
                this.Visibility = Visibility.Visible;
                this.control.IsEnabled = source.IsEnabled;
                this.isRequired = source.IsRequired;
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
            }
        }
        public void AddButton(Button btn)
        {
            btn.Click += (sender, e) =>
            {
                this.OnCustomButtonClicked?.Invoke(sender, e);
            };
            this.CustomButtons.Children.Add(btn);
        }
    }
}
