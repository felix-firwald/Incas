using Incas.Core.Classes;
using Incas.Core.Views.Controls;
using Incas.Objects.Components;
using Incas.Objects.Exceptions;
using Incas.Templates.Components;
using Incas.Templates.Views.Controls;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Incas.Objects.Views.Controls.IFiller;

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
    public partial class FieldFiller : UserControl, IFiller
    {
        //public static Dictionary<TagType, >
        public Objects.Models.Field Field { get; set; }
        private bool validated = true;
        private Control control;
        private CommandSettings command;
        public delegate void CommandScript(string script);
        public event StringAction OnInsert;
        public event CommandScript OnScriptRequested;
        public event FillerUpdate OnFillerUpdate;
        public event FillerUpdate OnDatabaseObjectCopyRequested;
        public FieldFiller(Objects.Models.Field f)
        {
            this.InitializeComponent();
            this.Field = f;
            this.MainLabel.Text = string.IsNullOrWhiteSpace(this.Field.VisibleName) ? this.Field.Name + ":" : this.Field.VisibleName + ":";
            try
            {
                this.GenerateUIControl(this.Field.Type);
                //this.MakeButton();
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }
        public void MarkAsNotValidated()
        {
            this.validated = false;
            this.Grid.Background = new SolidColorBrush(Color.FromRgb(68, 40, 45));
        }
        public void MarkAsValidated()
        {
            this.validated = true;
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
                case FieldType.Variable:
                case FieldType.Text:
                    System.Windows.Controls.TextBox textBox = new()
                    {
                        Text = value,
                        Tag = description
                    };
                    textBox.TextChanged += this.Textbox_TextChanged;
                    if (type == FieldType.Variable)
                    {
                        textBox.Style = this.FindResource("TextBoxMain") as Style;
                        textBox.MaxLength = 120;
                    }
                    else
                    {
                        textBox.Style = this.FindResource("TextBoxBig") as System.Windows.Style;
                        textBox.MaxLength = 1200;
                    }
                    this.PlaceUIControl(textBox);
                    break;
                case FieldType.LocalEnumeration:
                case FieldType.GlobalEnumeration:
                    value ??= "";
                    ComboBox comboBox = new()
                    {
                        ItemsSource = type == FieldType.LocalEnumeration ? JsonConvert.DeserializeObject<List<string>>(value) : ProgramState.GetEnumeration(Guid.Parse(value.ToString())),
                        SelectedIndex = 0,
                        Style = this.FindResource("ComboBoxMain") as Style
                    };
                    comboBox.SelectionChanged += this.Combobox_SelectionChanged;
                    this.PlaceUIControl(comboBox);
                    break;
                case FieldType.Number:
                    NumericBox numeric = new();
                    numeric.OnNumberChanged += this.NumericBox_OnNumberChanged;
                    numeric.ApplyMinAndMax(JsonConvert.DeserializeObject<Objects.Components.NumberFieldData>(value));
                    this.PlaceUIControl(numeric);
                    break;
                case FieldType.LocalConstant:
                case FieldType.HiddenField:
                    this.Visibility = Visibility.Collapsed;
                    break;
                case FieldType.GlobalConstant:
                    this.Visibility = Visibility.Collapsed;
                    Guid id;
                    Guid.TryParse(value.ToString(), out id);
                    this.Field.Value = ProgramState.GetConstant(id);
                    break;
                case FieldType.Relation:
                    SelectionBox selectionBox = new(JsonConvert.DeserializeObject<BindingData>(value));
                    selectionBox.OnValueChanged += this.SelectionBox_OnValueChanged;
                    this.PlaceUIControl(selectionBox);
                    break;
                case FieldType.Date:
                    DatePicker picker = new()
                    {
                        ToolTip = description,
                        Style = this.FindResource("DatePickerMain") as Style
                    };
                    picker.SelectedDateChanged += this.DatePicker_SelectedDateChanged;
                    this.PlaceUIControl(picker);
                    break;
                case FieldType.Generator:
                case FieldType.Macrogenerator:
                    Guid num = Guid.Empty;
                    Guid.TryParse(value.ToString(), out num);
                    Generator generator = new(type)
                    {
                        TemplateId = num
                    };
                    generator.OnValueChanged += this.Generator_OnValueChanged;
                    this.PlaceUIControl(generator);
                    break;
            }
        }

        private void PlaceUIControl(Control control)
        {
            this.control = control;
            this.control.ToolTip = this.Field.Description;
            this.Grid.Children.Add(control);
            Grid.SetRow(control, 0);
            Grid.SetColumn(control, 1);
        }

        private void MakeButton()
        {
            this.command = this.Field.GetCommand();
            if (this.command.ScriptType != ScriptType.Button)
            {
                return;
            }
            this.CommandButtonIcon.Data = this.FindResource(this.command.Icon.ToString()) as PathGeometry;
            this.CommandButton.Visibility = Visibility.Visible;
            this.CommandButtonText.Content = this.command.Name;
        }

        public void SetValue(string value)
        {
            switch (this.GetFillerType())
            {
                case FieldType.Variable:
                case FieldType.Text:
                default:
                    ((System.Windows.Controls.TextBox)this.control).Text = value;
                    break;
                case FieldType.Number:
                    int digitValue;
                    if (int.TryParse(value, out digitValue))
                    {
                        ((NumericBox)this.control).Value = digitValue;
                    }
                    break;
                case FieldType.Relation:
                    ((SelectionBox)this.control).SetObject(value);
                    break;
                case FieldType.LocalConstant:
                case FieldType.GlobalConstant:
                    return;
                case FieldType.HiddenField:
                    this.Field.Value = value;
                    break;
                case FieldType.LocalEnumeration:
                case FieldType.GlobalEnumeration:
                    ((ComboBox)this.control).SelectedValue = value;
                    break;
                case FieldType.Date:
                    this.SetDateTimeValue(value);
                    break;
                case FieldType.Generator:
                case FieldType.Macrogenerator:
                    ((Generator)this.control).SetData(value);
                    break;
            }
        }

        private void SetDateTimeValue(string value)
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
                    ((DatePicker)this.control).SelectedDate = parsedDate;
                }
                return success;
            }
            foreach (string format in formats)
            {
                if (tryApply(format))
                {
                    return;
                }
            }
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
                if (this.Field.NotNull == true)
                {
                    this.ThrowNotNullFailed();
                }
                return "";
            }
            DateFieldData df = JsonConvert.DeserializeObject<DateFieldData>(this.Field.Value);
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
                case FieldType.Variable:
                default:
                    string value = ((System.Windows.Controls.TextBox)this.control).Text;
                    if (this.Field.NotNull == true && string.IsNullOrEmpty(value))
                    {
                        this.ThrowNotNullFailed();
                    }
                    return value;
                case FieldType.Number:
                    return ((NumericBox)this.control).Value.ToString();
                case FieldType.LocalConstant:
                case FieldType.HiddenField:
                case FieldType.GlobalConstant:
                    return this.Field.Value?.ToString();
                case FieldType.Relation:
                    return ((SelectionBox)this.control).Value;
                case FieldType.LocalEnumeration:
                case FieldType.GlobalEnumeration:
                    ComboBox cb = (ComboBox)this.control;
                    if (cb.SelectedIndex != -1)
                    {
                        return cb.Items.GetItemAt(cb.SelectedIndex).ToString();
                    }
                    return "";
                case FieldType.Date:
                    return this.GetDateInFormat();
                case FieldType.Generator:
                case FieldType.Macrogenerator:
                    return ((Generator)this.control).GetText();
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
                case FieldType.Generator:
                case FieldType.Macrogenerator:
                    return ((Generator)this.control).GetData();
                case FieldType.Date:
                    if (((DatePicker)this.control).SelectedDate.HasValue)
                    {
                        return ((DateTime)((DatePicker)this.control).SelectedDate).ToString("dd.MM.yyyy");
                    }
                    else if (this.Field.NotNull == true)
                    {
                        this.ThrowNotNullFailed();
                    }
                    return "";
                case FieldType.Relation:
                    Objects.Components.Object obj = ((SelectionBox)this.control).SelectedObject;
                    if (this.Field.NotNull == true && obj is null)
                    {
                        this.ThrowNotNullFailed();
                    }
                    return obj?.Id.ToString();
                default: return this.GetValue();

            }
        }
        public Dictionary<string, string> GetDataFromObjectRelation()
        {
            Dictionary<string, string> result = new();
            Objects.Components.Object obj = ((SelectionBox)this.control).SelectedObject;
            if (obj is null)
            {
                return result;
            }
            foreach (FieldData data in obj.Fields)
            {
                if (data.ClassField.Type == FieldType.Relation)
                {
                    BindingData bd = data.ClassField.GetBindingData();
                    Objects.Components.Object recurObj = ObjectProcessor.GetObject(new(bd.Class), Guid.Parse(data.Value));
                    foreach (FieldData recdata in recurObj.Fields)
                    {
                        if (recdata.ClassField.Id == bd.Field)
                        {
                            result.Add($"{this.Field.Name}.{data.ClassField.Name}", recdata.Value);
                        }
                        result.Add($"{this.Field.Name}.{data.ClassField.Name}.{recdata.ClassField.Name}", recdata.Value);
                    }
                }
                else
                {
                    result.Add($"{this.Field.Name}.{data.ClassField.Name}", data.Value);
                }              
            }
            return result;
        }
        public void ThrowNotNullFailed()
        {
            this.MarkAsNotValidated();
            throw new NotNullFailed($"Поле \"{this.Field.VisibleName}\" является обязательным, однако не заполнено.");
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
        private void PlayScript()
        {
            try
            {
                if (this.command.Script.Contains("# [affects other]"))
                {
                    OnScriptRequested?.Invoke(this.command.Script);
                }
                else
                {
                    ScriptScope scope = ScriptManager.GetEngine().CreateScope();
                    scope.SetVariable("input_data", this.GetValue());
                    ScriptManager.Execute(this.command.Script, scope);

                    this.SetValue((string)scope.GetVariable("output"));
                }
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog("При обработке скрипта произошла ошибка:\n" + ex.Message);
            }
        }
        private void CommandClick(object sender, RoutedEventArgs e)
        {
            this.PlayScript();
        }
        private void CheckForScriptOnUpdate()
        {
            if (this.command.ScriptType == ScriptType.ValueChanged)
            {
                this.PlayScript();
            }
            if (!this.validated)
            {
                this.validated = true;
                this.MainLabel.Foreground = this.FindResource("GrayLight") as SolidColorBrush;
            }
        }
        private void RunUpdateEvent()
        {
            this.MarkAsValidated();
            this.OnFillerUpdate?.Invoke(this);
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

        private void Generator_OnValueChanged(object sender)
        {
            this.RunUpdateEvent();
            this.CheckForScriptOnUpdate();
        }

        private void NumericBox_OnNumberChanged(object sender)
        {
            this.RunUpdateEvent();
            this.CheckForScriptOnUpdate();
        }

        private void CopyFromIncasClipboard(object sender, RoutedEventArgs e)
        {
            string value = DialogsManager.ShowClipboardManager(true);
            if (!string.IsNullOrEmpty(value))
            {
                this.SetValue(value);
            }
        }

        private void ObjectCopyRequestClick(object sender, RoutedEventArgs e)
        {
            this.OnDatabaseObjectCopyRequested?.Invoke(this);
        }
    }
}
