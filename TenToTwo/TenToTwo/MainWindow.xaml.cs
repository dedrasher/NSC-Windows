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
        private string HistoryItem;
        private string FromStr, ToStr;
        private AppLocalize.AppLanguage appLanguage;
        private Control[] UIElmentsToTranslate;
                private KeyConverter keyConverter = new KeyConverter();
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
        private string GetSmallNumber(int value)
        {
            return new string[] { "₂","₃", "₄","₅", "₆","₇", "₈", "₉", "₁₀", "₁₁", "₁₂", "₁₃", "₁₄", "₁₅",
            "₁₆","₁₇","₁₈","₁₉","₂₀","₂₁","₂₂","₂₃","₂₄","₂₅","₂₆","₂₇","₂₈","₂₉","₃₀","₃₁","₃₂","₃₃","₃₄","₃₅","₃₆"}[value - 2];
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string OldAnswer = AnswerLabel.Text.Replace(AppLocalize.GetLocalize(AppLocalize.TranslateText.Answer, appLanguage), "");
            int FromInt = 0, ToInt = 0;
            try
            {
                if ((bool)DecimalToBinary.IsChecked)
                {
                    FromInt = 10;
                    ToInt = 2;
                }
                else if ((bool)BinaryToDecimal.IsChecked)
                {
                    FromInt = 2;
                    ToInt = 10;
                }
                else if ((bool)FromRadioButton.IsChecked)
                {
                    FromInt = int.Parse(From.Text);
                    ToInt = int.Parse(To.Text);
                }
             var Answer = NumericSystemConverter.Convert(FromInt, ToInt, Input.Text);
                AnswerLabel.Text = AppLocalize.GetLocalize(AppLocalize.TranslateText.Answer, appLanguage) + Answer;
                if (History.Items.Count == 0 || Answer != OldAnswer || FromStr != From.Text || ToStr != To.Text)
                {
                    if (ClearButton.Visibility == Visibility.Hidden)
                    {
                        ClearButton.Visibility = Visibility.Visible;
                    }
                    History.Items.Add(Input.Text + GetSmallNumber(FromInt) + " = " + Answer + GetSmallNumber(ToInt));
                    History.ScrollIntoView(History.Items[History.Items.Count - 1]);
                }
                FromStr = From.Text;
                ToStr = To.Text;
            }
            catch (Exception er)
            {
                if(er is OverflowException)
                MessageBox.Show(AppLocalize.GetLocalize(AppLocalize.TranslateText.VeryBigNumber, appLanguage), AppLocalize.GetLocalize(AppLocalize.TranslateText.Error, appLanguage), MessageBoxButton.OK, MessageBoxImage.Error);
                else if (er is ArgumentException)
                    MessageBox.Show(AppLocalize.GetLocalize(AppLocalize.TranslateText.InputIsNotCorrect, appLanguage), AppLocalize.GetLocalize(AppLocalize.TranslateText.Error, appLanguage), MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }



        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            var CharKey = keyConverter.ConvertToString(e.Key)[0];
            if (e.Key == Key.OemMinus && Input.Text.Length > 0 && Input.CaretIndex > 0)
            {
                e.Handled = true;
            }
            if ((bool)DecimalToBinary.IsChecked)
            {
                if (!char.IsDigit(CharKey) && e.Key != Key.OemMinus)
                {
                    e.Handled = true;
                }
            }
            else if ((bool)BinaryToDecimal.IsChecked)
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
            ClearButtonContentControl.Content = ClearButtonContentControl.Content == null ? AppLocalize.GetLocalize(AppLocalize.TranslateText.ClearHistory, appLanguage) :
                AppLocalize.GetAntotherLocalizeByString(ClearButtonContentControl.Content as string);
            var CountButtonContentControl = (ContentControl)CountButton.Template.FindName("ContentControl", CountButton);
            CountButtonContentControl.Content = AppLocalize.GetLocalize(AppLocalize.TranslateText.Count, appLanguage);
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
            var Language = Settings.Default.Language;
                appLanguage = Language == "Russian" ? AppLocalize.AppLanguage.Russian : AppLocalize.AppLanguage.English;
            LanguageSwitcher.Source = AppLocalize.GetLanguageIcon(appLanguage == AppLocalize.AppLanguage.English ? AppLocalize.AppLanguage.Russian : AppLocalize.AppLanguage.English);
            AppLocalize.LocalizeAll(appLanguage, UIElmentsToTranslate.Length, UIElmentsToTranslate);
            ChangeButtonLocalize();
            var dp = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
            dp.AddValueChanged(AnswerLabel, (s, args) => {
                if (AnswerLabel.Text.Length > 32) { AnswerLabel.FontSize = 12; } 
                else if (AnswerLabel.Text.Length > 20) { AnswerLabel.FontSize = 20; } else { AnswerLabel.FontSize = 26; }
            });
            BinaryToDecimal.IsChecked = Settings.Default.BinIsCheck;
            DecimalToBinary.IsChecked = Settings.Default.DecIsCheck;
            FromRadioButton.IsChecked = Settings.Default.CustomIsCheck;
            To.Text = Settings.Default.To;
            From.Text = Settings.Default.From;
            FromStr = From.Text;
            ToStr = To.Text;
            Input.Text = Settings.Default.Input;
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
            AnswerLabel.Text = Settings.Default.Answer;
            var temp = Settings.Default.History;
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
            var animation = new DoubleAnimation();
            animation.From = 20.0;
            animation.To = CountButton.ActualWidth;
            animation.Duration = TimeSpan.FromSeconds(2);
            CountButton.BeginAnimation(WidthProperty, animation);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.Answer = AnswerLabel.Text;
            if (BinaryToDecimal.IsChecked.HasValue)
            {
                Settings.Default.BinIsCheck = (bool)BinaryToDecimal.IsChecked;
            }
            if (DecimalToBinary.IsChecked.HasValue)
            {
                Settings.Default.DecIsCheck = (bool)DecimalToBinary.IsChecked;
            }
            if (FromRadioButton.IsChecked.HasValue)
            {
                Settings.Default.CustomIsCheck = (bool)FromRadioButton.IsChecked;
            }
            if (History.Items.Count > 0)
            {
                Settings.Default.History = string.Join(",", History.Items.Cast<string>().ToArray().Select(i => i).ToArray());
            } else
            {
                Settings.Default.History = "null";
            }
            Settings.Default.Language = appLanguage == AppLocalize.AppLanguage.Russian ? "Russian" : "English";
            Settings.Default.From = From.Text;
            Settings.Default.To = To.Text;
            Settings.Default.Input = Input.Text;
            Settings.Default.Save();
        }



        private void DecimalToBinary_Checked(object sender, RoutedEventArgs e)
        {
            AnswerLabel.Text = null;
            if (Input.Text.Where(s => char.IsLetter(s)).Count() > 0)
            {
                Input.Text = null;
                Hint.Visibility = Visibility.Visible;
            }
        }

        private void FromRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Input.Text.Length > 0)
            {
                Hint.Visibility = Visibility.Hidden;
            }
            AnswerLabel.Text = null;
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
                MessageBox.Show(AnswerLabel.Text, AppLocalize.GetLocalize(AppLocalize.TranslateText.AnswerTitle, appLanguage) , MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            if (History.SelectedItems.Count == 0 && MessageBox.Show(AppLocalize.GetLocalize(AppLocalize.TranslateText.ClearHistoryQuestion, appLanguage), AppLocalize.GetLocalize(AppLocalize.TranslateText.Warning, appLanguage), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                History.Items.Clear();
            else if (History.SelectedItems.Count > 0 && MessageBox.Show(AppLocalize.GetLocalize(AppLocalize.TranslateText.ClearSelectedQuestion, appLanguage), AppLocalize.GetLocalize(AppLocalize.TranslateText.Warning, appLanguage), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
                ContentControl.Content = AppLocalize.GetLocalize(AppLocalize.TranslateText.ClearSelected, appLanguage);
            }
            else
            {
                ContentControl.Content = AppLocalize.GetLocalize(AppLocalize.TranslateText.ClearHistory, appLanguage);
            }

        }
        private void SetHistoryItemToInterface(string value)
        {
            var SmallNumbers = new string[] { "₁", "₂", "₃", "₄", "₅", "₆", "₇", "₈", "₉", "₀" };
            bool IsSmallRegisterNumber(string number)
            {
                return Array.IndexOf(SmallNumbers, number) > -1;
            }
            string UpperNumber(string SmallNumber)
            {
                var UpperNumbers = "1234567890";
                string result = null;
                for (int i = 0; i < SmallNumber.Length; i++)
                {
                    result += UpperNumbers[Array.IndexOf(SmallNumbers, SmallNumber[i].ToString())];
                }
                return result;
            }
            var item = value;
            var input = string.Join("", item.TakeWhile(s => !IsSmallRegisterNumber(s.ToString())).ToArray());
            var FromNC = UpperNumber(string.Join("", item.Remove(0, input.Length).TakeWhile(s => IsSmallRegisterNumber(s.ToString())).ToArray()));
            var Result = string.Join("", item.Remove(0, input.Length + FromNC.Length + 3).TakeWhile(s => !IsSmallRegisterNumber(s.ToString())).ToArray());
            var ToNC = UpperNumber(item.Remove(0, input.Length + FromNC.Length + 3 + Result.Length).ToUpper());
            BinaryToDecimal.IsChecked = false;
            DecimalToBinary.IsChecked = false;
            FromRadioButton.IsChecked = true;
            Input.Text = input;
            From.Text = FromNC;
            To.Text = ToNC;
            AnswerLabel.Text = AppLocalize.GetLocalize(AppLocalize.TranslateText.Answer, appLanguage) + Result;
            if (Hint.Visibility == Visibility.Visible)
                Hint.Visibility = Visibility.Hidden;
        }
        private void ApplyHIToInter_Click(object sender, RoutedEventArgs e)
        {
              SetHistoryItemToInterface(HistoryItem);
        }

        private void History_MouseRightButtonDown(object sender, RoutedEventArgs e) { 

                var Button = History.ContextMenu.Items[0] as MenuItem;
                Button.Header = AppLocalize.GetLocalize(AppLocalize.TranslateText.ApplyToInterface, appLanguage);
                HistoryItem = (sender as ListBoxItem).Content.ToString();
        }

        private void History_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (History.SelectedItems.Count != 1)
                e.Handled = true;
        }

        private void LanguageSwitcher_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var OldAnswerText = AppLocalize.GetLocalize(AppLocalize.TranslateText.Answer, appLanguage);
            LanguageSwitcher.Source = AppLocalize.GetLanguageIcon(appLanguage);
            appLanguage = appLanguage == AppLocalize.AppLanguage.English ? AppLocalize.AppLanguage.Russian : AppLocalize.AppLanguage.English;               
                AppLocalize.LocalizeAll(appLanguage, UIElmentsToTranslate.Length,UIElmentsToTranslate);
            MoveCustomRadioButtonContent(appLanguage == AppLocalize.AppLanguage.Russian ? -39f : 39f);
            ChangeButtonLocalize();
            if (AnswerLabel.Text.Contains(OldAnswerText))
            {
                AnswerLabel.Text = AnswerLabel.Text.Replace(OldAnswerText, AppLocalize.GetLocalize(AppLocalize.TranslateText.Answer, appLanguage));
            }
        }
    }
}
