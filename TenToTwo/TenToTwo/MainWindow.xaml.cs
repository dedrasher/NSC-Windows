using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using NumericSystemConverter.Properties;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.ComponentModel;
using System.Linq;
namespace NumericSystemConverterApp
{
    public partial class MainWindow : Window
    {
        private enum CalculateMode
        {
            None, DecToBin, BinToDec, Custom
        }
        private CalculateMode calculateMode;
        private string  HistoryItem,OldAnswer;
        private AppLocalize.AppLanguage appLanguage;
        private Control[] UIElmentsToTranslate;
                private KeyConverter keyConverter = new KeyConverter();
        private Settings currentSettings;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void DeselectListBoxItem(object sender, RoutedEventArgs e)
        {
           var item = sender as ListBoxItem;
            if (item.IsSelected && History.SelectedItems.Count == 1)
                item.Dispatcher.InvokeAsync(() => item.IsSelected = false);
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           var OldAnswer = this.OldAnswer.Replace(AppLocalize.GetLocalizedString(AppLocalize.TranslateText.Answer, appLanguage), "");
            int FromInt = 0, ToInt = 0;
            try
            {
                if (calculateMode == CalculateMode.DecToBin)
                {
                    FromInt = 10;
                    ToInt = 2;
                }
                else if (calculateMode == CalculateMode.BinToDec)
                {
                    FromInt = 2;
                    ToInt = 10;
                }
                else if (calculateMode == CalculateMode.Custom)
                {
                    FromInt = int.Parse(From.Text);
                    ToInt = int.Parse(To.Text);
                }
             var Answer = NumericSystemConverter.Convert(FromInt, ToInt, Input.Text);
                AnswerLabel.Text = AppLocalize.GetLocalizedString(AppLocalize.TranslateText.Answer, appLanguage) + Answer;
                if (History.Items.Count == 0 || Answer != OldAnswer)
                {
                    if (ClearButton.Visibility == Visibility.Hidden)
                    {
                        ClearButton.Visibility = Visibility.Visible;
                    }
                    History.Items.Add(Input.Text + AppTools.GetSmallNumber(FromInt) + " = " + Answer + AppTools.GetSmallNumber(ToInt));
                    History.ScrollIntoView(History.Items[History.Items.Count - 1]);
                }
                this.OldAnswer = AnswerLabel.Text;
            }
            catch (Exception er)
            {
                if(er is OverflowException)
                MessageBox.Show(AppLocalize.GetLocalizedString(AppLocalize.TranslateText.VeryBigNumber, appLanguage), AppLocalize.GetLocalizedString(AppLocalize.TranslateText.Error, appLanguage), MessageBoxButton.OK, MessageBoxImage.Error);
                else if (er is ArgumentException)
                    MessageBox.Show(AppLocalize.GetLocalizedString(AppLocalize.TranslateText.InputIsNotCorrect, appLanguage), AppLocalize.GetLocalizedString(AppLocalize.TranslateText.Error, appLanguage), MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }



        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            var CharKey = keyConverter.ConvertToString(e.Key)[0];
            if (e.Key == Key.OemMinus && Input.Text.Length > 0 && Input.CaretIndex > 0)
            {
                e.Handled = true;
            }
            if (calculateMode == CalculateMode.DecToBin)
            {
                if (!char.IsDigit(CharKey) && e.Key != Key.OemMinus)
                {
                    e.Handled = true;
                }
            }
            else if (calculateMode == CalculateMode.BinToDec)
            {
                if (CharKey != '0' && CharKey != '1' && e.Key != Key.OemMinus)
                    e.Handled = true;
            }

        }


        private void BinaryToDecimal_Checked(object sender, RoutedEventArgs e)
        {
            Hint.Visibility = Visibility.Visible;
            Input.Text = null;
            AnswerLabel.Text = null;
            calculateMode = CalculateMode.BinToDec;
        }
     private void MoveCustomRadioButtonContent(float x)
        {
            foreach(var i in new Control[] { FromText, From, To, ToLabel, NsLabel })
            {
                i.Margin = new Thickness(i.Margin.Left + x, i.Margin.Top, i.Margin.Right - x, i.Margin.Bottom);
            }
        }
        
        private void ChangeButtonLocalize()
        {
            var ClearButtonContentControl = (ContentControl)ClearButton.Template.FindName("ContentControl", ClearButton);
            ClearButtonContentControl.Content = ClearButtonContentControl.Content == null ? AppLocalize.GetLocalizedString(AppLocalize.TranslateText.ClearHistory, appLanguage) :
                AppLocalize.GetAntotherLocalizeByString(ClearButtonContentControl.Content as string);
            var CountButtonContentControl = (ContentControl)CountButton.Template.FindName("ContentControl", CountButton);
            CountButtonContentControl.Content = AppLocalize.GetLocalizedString(AppLocalize.TranslateText.Count, appLanguage);
            if (appLanguage == AppLocalize.AppLanguage.Russian)
            {
                ClearButtonContentControl.FontSize = 11.5;
                CountButtonContentControl.FontSize = 26;
            } else
            {
                ClearButtonContentControl.FontSize = 18;
                CountButtonContentControl.FontSize = 35;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UIElmentsToTranslate = new Control[]{Hint, DecimalToBinary, BinaryToDecimal,FromText,ToLabel, NsLabel, HistoryBlock };
            AppLocalize.IconsInit();
            currentSettings = Settings.Default;
            var Language = currentSettings.Language;
                appLanguage = Language == "Russian" ? AppLocalize.AppLanguage.Russian : AppLocalize.AppLanguage.English;
            LanguageSwitcher.Source = AppLocalize.GetLanguageIcon(appLanguage == AppLocalize.AppLanguage.English ? AppLocalize.AppLanguage.Russian : AppLocalize.AppLanguage.English);
            AppLocalize.LocalizeAll(appLanguage, UIElmentsToTranslate.Length, UIElmentsToTranslate);
            ChangeButtonLocalize();
            var dp = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
            dp.AddValueChanged(AnswerLabel, (s, args) => {
                if (AnswerLabel.Text.Length > 32) { AnswerLabel.FontSize = 12; } 
                else if (AnswerLabel.Text.Length > 17) { AnswerLabel.FontSize = 20; } else { AnswerLabel.FontSize = 26; }
            });
            calculateMode = (CalculateMode)currentSettings.CalculateMode;
            BinaryToDecimal.IsChecked = calculateMode == CalculateMode.BinToDec;
            DecimalToBinary.IsChecked = calculateMode == CalculateMode.DecToBin;
            FromRadioButton.IsChecked = calculateMode == CalculateMode.Custom;
            To.Text = currentSettings.To;
            From.Text = currentSettings.From;
            Input.Text = currentSettings.Input;
            MoveCustomRadioButtonContent(appLanguage == AppLocalize.AppLanguage.Russian ? -39f : 0f);
            EventManager.RegisterClassHandler(typeof(ListBoxItem),
   MouseRightButtonDownEvent,
    new RoutedEventHandler(History_MouseRightButtonDown));
            EventManager.RegisterClassHandler(typeof(ListBoxItem),
   MouseLeftButtonDownEvent,
    new RoutedEventHandler(DeselectListBoxItem));
            if (Input.Text.Length != 0)
            {
                Hint.Visibility = Visibility.Hidden;
            }
            OldAnswer = currentSettings.OldAnswer;
            AnswerLabel.Text = currentSettings.Answer;
            var temp = currentSettings.History;
            if (temp != "null")
            {
                foreach (var i in temp.Split(',').Select(s => s).ToArray())
                {
                    History.Items.Add(i);
                }
            } else
            {
                ClearButton.Visibility = Visibility.Hidden;
            }
            var animation = new DoubleAnimation
            {
                From = 20.0,
                To = CountButton.ActualWidth,
                Duration = TimeSpan.FromSeconds(0.8)
            };
            CountButton.BeginAnimation(WidthProperty, animation);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            currentSettings.OldAnswer = OldAnswer;
            currentSettings.Answer = AnswerLabel.Text;
            currentSettings.CalculateMode = (int)calculateMode;
            currentSettings.History = History.Items.Count > 0 ? string.Join(",", History.Items.Cast<string>().ToArray().Select(i => i).ToArray()) : "null";
            currentSettings.Language = appLanguage == AppLocalize.AppLanguage.Russian ? "Russian" : "English";
            currentSettings.From = From.Text;
            currentSettings.To = To.Text;
            currentSettings.Input = Input.Text;
            currentSettings.Save();
        }
        private void DecimalToBinary_Checked(object sender, RoutedEventArgs e)
        {
            AnswerLabel.Text = null;
            if (Input.Text.Where(s => char.IsLetter(s)).Count() > 0)
            {
                Input.Text = null;
                Hint.Visibility = Visibility.Visible;
            }
            calculateMode = CalculateMode.DecToBin;
        }

        private void FromRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Input.Text.Length > 0)
            {
                Hint.Visibility = Visibility.Hidden;
            }
            AnswerLabel.Text = null;
            calculateMode = CalculateMode.Custom;
        }

        private void From_KeyDown(object sender, KeyEventArgs e)
        {
            var CharKey = keyConverter.ConvertToString(e.Key)[0];
            if (!char.IsDigit(CharKey))
            {
                e.Handled = true;
                return;
            }


            if (CharKey == '0' && From.Text.Length == 0)
            {
                e.Handled = true;
                return;
            }


        }

        private void To_KeyDown(object sender, KeyEventArgs e)
        {
            var CharKey = keyConverter.ConvertToString(e.Key)[0];
            if (!char.IsDigit(CharKey))
            {
                e.Handled = true;
                return;
            }


            if (CharKey == '0' && To.Text.Length == 0)
            {
                e.Handled = true;
                return;
            }


        }

        private void From_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(From.Text, out int result))
            {
                if (result > 36)
                {
                    int cursor = From.CaretIndex;
                    From.Text = From.Text.Remove(From.Text.Length - 1, 1);
                    From.CaretIndex = cursor - 1;
                }
            }
        }

        private void To_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(To.Text, out int result))
            {
                if (result > 36)
                {
                    int cursor = To.CaretIndex;
                    To.Text = To.Text.Remove(To.Text.Length - 1, 1);
                    To.CaretIndex = cursor - 1;
                }
            }
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Input.Text.Length > 14) { Input.FontSize = 24; } else { Input.FontSize = 34; }
            if (!Regex.IsMatch(Input.Text, "^[A-Za-z0-9-]+$") && Input.Text.Length > 0) {
                int cursor = Input.CaretIndex;
                Input.Text = Input.Text.Remove(Input.Text.Length - 1);
                if (cursor > 0)
                    Input.CaretIndex = cursor - 1;
                else
                    Input.CaretIndex = cursor;
            }
        }

    

        private void Input_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Input.Text.Length == 0)
                Hint.Visibility = Visibility.Visible;
        }
        private void Hint_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Hint.Visibility = Visibility.Hidden;
            Input.Focus();
        }

        private void AnswerLabel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (AnswerLabel.Text.Length > 0)
                MessageBox.Show(AnswerLabel.Text, AppLocalize.GetLocalizedString(AppLocalize.TranslateText.AnswerTitle, appLanguage) , MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            if (History.SelectedItems.Count == 0 && MessageBox.Show(AppLocalize.GetLocalizedString(AppLocalize.TranslateText.ClearHistoryQuestion, appLanguage), AppLocalize.GetLocalizedString(AppLocalize.TranslateText.Warning, appLanguage), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                History.Items.Clear();
            else if (History.SelectedItems.Count > 0 && MessageBox.Show(AppLocalize.GetLocalizedString(AppLocalize.TranslateText.ClearSelectedQuestion, appLanguage), AppLocalize.GetLocalizedString(AppLocalize.TranslateText.Warning, appLanguage), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Array SelectedItems = new object[History.SelectedItems.Count];
                History.SelectedItems.CopyTo(SelectedItems,0);
                foreach (var i in SelectedItems)
                    History.Items.Remove(i);
            }
            if (History.Items.Count == 0)
                ClearButton.Visibility = Visibility.Hidden;
        }

        private void History_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           var ContentControl = (ContentControl)ClearButton.Template.FindName("ContentControl", ClearButton);
            if (History.SelectedItems.Count > 0)
            {               
                ContentControl.Content = AppLocalize.GetLocalizedString(AppLocalize.TranslateText.ClearSelected, appLanguage);
            }
            else
            {
                ContentControl.Content = AppLocalize.GetLocalizedString(AppLocalize.TranslateText.ClearHistory, appLanguage);
            }

        }
        private void SetHistoryItemToInterface(string value)
        {
            var ParsedExpression = AppTools.ParseExpression(value);
            if (calculateMode != CalculateMode.Custom)
            {
                BinaryToDecimal.IsChecked = false;
                DecimalToBinary.IsChecked = false;
                FromRadioButton.IsChecked = true;
                calculateMode = CalculateMode.Custom;
            }
            Input.Text = ParsedExpression.Input;
            From.Text = ParsedExpression.FromNC;
            To.Text = ParsedExpression.ToNC;
            AnswerLabel.Text = AppLocalize.GetLocalizedString(AppLocalize.TranslateText.Answer, appLanguage) + ParsedExpression.Answer;
            if (Hint.Visibility == Visibility.Visible)
                Hint.Visibility = Visibility.Hidden;
        }
        private void ApplyHIToInter_Click(object sender, RoutedEventArgs e)
        {
              SetHistoryItemToInterface(HistoryItem);
        }

        private void History_MouseRightButtonDown(object sender, RoutedEventArgs e) { 

                var Button = History.ContextMenu.Items[0] as MenuItem;
                Button.Header = AppLocalize.GetLocalizedString(AppLocalize.TranslateText.ApplyToInterface, appLanguage);
                HistoryItem = (sender as ListBoxItem).Content.ToString();
        }

        private void History_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (History.SelectedItems.Count != 1)
                e.Handled = true;
        }

        private void LanguageSwitcher_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var OldAnswerText = AppLocalize.GetLocalizedString(AppLocalize.TranslateText.Answer, appLanguage);
            LanguageSwitcher.Source = AppLocalize.GetLanguageIcon(appLanguage);
            appLanguage = appLanguage == AppLocalize.AppLanguage.English ? AppLocalize.AppLanguage.Russian : AppLocalize.AppLanguage.English;               
                AppLocalize.LocalizeAll(appLanguage, UIElmentsToTranslate.Length,UIElmentsToTranslate);
            MoveCustomRadioButtonContent(appLanguage == AppLocalize.AppLanguage.Russian ? -39f : 39f);
            ChangeButtonLocalize();
            if (AnswerLabel.Text.Contains(OldAnswerText))
            {
                AnswerLabel.Text = AnswerLabel.Text.Replace(OldAnswerText, AppLocalize.GetLocalizedString(AppLocalize.TranslateText.Answer, appLanguage));
            }
        }
    }
}
