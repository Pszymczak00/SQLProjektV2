using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SQLProjektV2
{
    public static class Globals
    {

        public static List<string> OperatorsString { get; set; } = new List<string>() { "Równe (=)", "Różne (!=)", "Mniejsze (<)", "Mniejsze lub równe (<=)", "Większe (>)", "Większe lub równe (>=)", "Zawiera", "Nie zawiera" };

        public static List<string> OperatorsDateNumber { get; set; } = new List<string>() { "Równe (=)", "Różne (!=)", "Mniejsze (<)", "Mniejsze lub równe (<=)", "Większe (>)", "Większe lub równe (>=)" };


        public static List<string> OperatorsBoolean { get; set; } = new List<string>() { "Prawda", "Fałsz" };

        public static ComboBox ComboBox
        {
            get
            {
                return new ComboBox()
                {
                    Height = 30,
                    Width = 150,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5)
                };
            }
        }

        public static Button Button
        {
            get
            {
                Button temp =  new Button()
                {
                    Height = 30,
                    Width = 150,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5),
                    Content = "Usuń filtr",
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(2),
                    BorderBrush = Brushes.White,
                    Background = new SolidColorBrush(Color.FromArgb(0, 40, 40, 40))
                };

                temp.Click += DeleteFilter;
                return temp;
            }
        }

        private static void DeleteFilter(object sender, RoutedEventArgs e)
        {
            (((StackPanel)((Button)sender).Parent).Parent as StackPanel).Children.Remove(((StackPanel)((Button)sender).Parent));
        }

        public static ComboBox ComboBoxColumnChoose
        {
            get
            {
                ComboBox temp = ComboBox;
                temp.SelectionChanged += OnMyComboBoxChanged;
                temp.SelectedValuePath = "Value";
                temp.DisplayMemberPath = "Key";
                return temp;
            }
        }

        public static TextBox TextBox
        {
            get
            {
                return new TextBox()
                {
                    Background = new SolidColorBrush(Color.FromArgb(0, 22, 22, 22)),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.White,
                    Height = 30,
                    Width = 150,
                    Margin = new Thickness(5)
                };
            }
        }

        public static TextBox TextBoxOnlyNumbers
        {
            get
            {
                TextBox temp = new TextBox()
                {
                    Background = new SolidColorBrush(Color.FromArgb(0, 22, 22, 22)),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.White,
                    Height = 30,
                    Width = 150,
                    Margin = new Thickness(5)
                };
                temp.PreviewTextInput += NumberValidationTextBox;
                return temp;
            }
        }

        public static DatePicker DatePicker
        {
            get
            {
                return new DatePicker()
                {
                    Background = new SolidColorBrush(Color.FromArgb(0, 22, 22, 22)),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Height = 30,
                    Width = 150,
                    Margin = new Thickness(5)
                };
            }
        }

        public static DatePicker DatePickerMonth
        {
            get
            {
                DatePicker temp = Globals.DatePicker;
                SQLProjektV2.Views.DatePickerCalendar.SetIsMonthYear(temp, true);
                temp.Text = "MM-yyyy";
                SQLProjektV2.Views.DatePickerDateFormat.SetDateFormat(temp, "MM-yyyy");
                return temp;
            }
        }


        public static Xceed.Wpf.Toolkit.DateTimeUpDown DatePickerLong
        {
            get
            {
                return new Xceed.Wpf.Toolkit.DateTimeUpDown()
                {
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Height = 30,
                    Width = 200,
                    Margin = new Thickness(5),
                    DefaultValue = DateTime.Now,
                    Value = DateTime.Now
                };
            }
        }
         


        public static StackPanel StackPanel
        {
            get
            {
                return new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(5)
                };
            }
        }

        private static void OnMyComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            string temp = ((KeyValuePair<string, string>)(sender as ComboBox).SelectedItem).Value;
            

            if(temp == "String" || temp == "StringNumbers")
                ((ComboBox)((StackPanel)((ComboBox)sender).Parent).Children[1]).ItemsSource = Globals.OperatorsString;
            else if (temp == "Boolean")
                ((ComboBox)((StackPanel)((ComboBox)sender).Parent).Children[1]).ItemsSource = Globals.OperatorsBoolean;
            else
                ((ComboBox)((StackPanel)((ComboBox)sender).Parent).Children[1]).ItemsSource = Globals.OperatorsDateNumber;

            ((ComboBox)((StackPanel)((ComboBox)sender).Parent).Children[1]).SelectedIndex = 0;

            if(((StackPanel)((ComboBox)sender).Parent).Children.Count == 4)
                ((StackPanel)((ComboBox)sender).Parent).Children?.RemoveAt(2);
            if (temp == "DateShort")
                ((StackPanel)((ComboBox)sender).Parent).Children.Insert(2, Globals.DatePicker);
            else if (temp == "DateLong")
                ((StackPanel)((ComboBox)sender).Parent).Children.Insert(2, Globals.DatePickerLong);
            else if (temp == "DateMonth")
                ((StackPanel)((ComboBox)sender).Parent).Children.Insert(2, Globals.DatePickerMonth);
            else if (temp == "StringNumbers" || temp == "Number")
                ((StackPanel)((ComboBox)sender).Parent).Children.Insert(2, Globals.TextBoxOnlyNumbers);
            else if (temp == "String")
                ((StackPanel)((ComboBox)sender).Parent).Children.Insert(2, Globals.TextBox);
        }

        private static void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }

        public static string  GetFilter(StackPanel filterList)
        {
            string output = "";


            foreach (StackPanel el in filterList.Children)
            {
                string oper = ((ComboBox)el.Children[1]).SelectedItem.ToString();

                switch (oper)
                {
                    case "Równe (=)":
                        oper = "=";
                        break;
                    case "Różne (!=)":
                        oper = "<>";
                        break;
                    case "Mniejsze (<)":
                        oper = "<";
                        break;
                    case "Mniejsze lub równe (<=)":
                        oper = "<=";
                        break;
                    case "Większe (>)":
                        oper = ">";
                        break;
                    case "Większe lub równe (>=)":
                        oper = ">=";
                        break;
                    default:
                        break;
                }

                string columnName = ((KeyValuePair<string, string>)((ComboBox)el.Children[0]).SelectedItem).Key;
                string columnType = ((KeyValuePair<string, string>)((ComboBox)el.Children[0]).SelectedItem).Value;
                if (columnType == "String")
                {
                    string text = ((TextBox)el.Children[2]).Text;  
                    if (oper == "Zawiera")
                    {
                        oper = "LIKE";
                        text = "*" + text + "*";
                    } 
                    else if (oper == "Nie zawiera")
                    {
                        oper = "NOT LIKE";
                        text = "*" + text + "*";
                    }

                    output += $"Isnull([{columnName}], '')" + " " + oper + " '" + text + "'";
                }

                if (columnType == "StringNumbers")
                {
                    string text = ((TextBox)el.Children[2]).Text;
                    if (oper == "Zawiera")
                    {
                        oper = "LIKE";
                        text = "*" + text + "*";
                    }
                    else if (oper == "Nie zawiera")
                    {
                        oper = "NOT LIKE";
                        text = "*" + text + "*";
                    }

                    output += $"Isnull([{columnName}], '')" + " " + oper + " '" + text + "'";
                }

                if (columnType == "Number")
                {
                    string text = ((TextBox)el.Children[2]).Text;
                    if(text.Length > 0)
                        output += "[" + columnName + "]" + " " + oper + " " + text ;
                    else output += "[" + columnName + "] IS NULL";
                }

                if (columnType == "DateShort")
                {
                    if (((DatePicker)el.Children[2]).SelectedDate != null)
                    {
                        string text = ((DatePicker)el.Children[2]).SelectedDate.Value.ToString("MM-dd-yyyy");

                        output += $"Isnull([{columnName}], '')" + " " + oper + " #" + text + "#";
                    }
                    else output += $"Isnull([{columnName}], '') = ''";

                }

                if (columnType == "DateMonth")
                {
                    string year = ((DatePicker)el.Children[2]).SelectedDate.Value.Year.ToString();
                    string month = ((DatePicker)el.Children[2]).SelectedDate.Value.Month.ToString();
                    string text = $"{year}-{month}-01";

                    output += "[" + columnName + "]" + " " + oper + " #" + text + "#";
                }

                if (columnType == "DateLong")
                {
                    string text = ((Xceed.Wpf.Toolkit.DateTimeUpDown)el.Children[2]).Value.Value.ToString("yyyy-MM-dd HH:mm:ss");

                    output += "[" + columnName + "]" + " " + oper + " #" + text + "#";
                }

                if (columnType == "Boolean")
                {
                    if (oper == "Prawda")
                    {
                        output += "[" + columnName + "]" + " = 1";
                    }
                    else if (oper == "Fałsz")
                    {
                        output += "[" + columnName + "]" + " = 0";
                    }
                    
                }
                output += " AND ";
            }

            if (output.Length > 0)
            {
                return output.Substring(0, output.Length - 4);
            }
            else return "";

        }
    }
}
