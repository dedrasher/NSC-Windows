using System.Windows.Media.Imaging;
using System.IO;
using NumericSystemConverter.Properties;
using System;
using System.Drawing;
using System.Windows.Controls;
namespace NumericSystemConverterApp
{
   public static class AppLocalize
    {
        public enum TranslateText
        {
            EnterANumber, DecimalToBinary, BinaryToDecimal, From, To, Ns,  History, ClearHistory, ClearSelected, Answer, ClearHistoryQuestion, ClearSelectedQuestion, Warning, Count, VeryBigNumber, Error, InputIsNotCorrect
        }
        private static string[] EnglishInterface = {"Enter a number", "Decimal to binary", "Binary to decimal", "From","to","N.s"  , "History", "Clear history"
                ,"Clear selected","Answer: " , "Do you really wanna clear history?" , "Do you really wanna clear selected?","Warning!","Count", "Very big number!" , "Error", "Input is not correct!"};
        private static string[] RussianInterface = {"Введите число", "Десятич. в двоич.", "Двоич. в десятич.", "С","до","С.ч" ,  "История", "Очистить историю"
                ,"Очистить выбранное", "Ответ: ", "Вы правда хотите очистить историю?", "Вы правда хотите очистить выбранное?","Внимание!" , "Подсчет", "Слишком большое число!" , "Ошибка", "Ввод некорректен!"};

        public enum AppLanguage
        {
            Russian, English
        }
        public static void LocalizeAll(AppLanguage appLanguage, int count, params Control[] UIElements)
        {
            for (int i = 0; i < count; i++) { 
                    var control = UIElements[i] as ContentControl;
                    control.Content = appLanguage == AppLanguage.Russian ? RussianInterface[i] : EnglishInterface[i];
                }
            }
        
        
        public static BitmapImage GetLanguageIcon(AppLanguage appLanguage)
        { 
            byte[] BitmapToBytes(Bitmap Bitmap)
            {
                MemoryStream ms = null;
                try
                {
                    ms = new MemoryStream();
                    Bitmap.Save(ms, Bitmap.RawFormat);
                    byte[] byteImage = new byte[ms.Length];
                    byteImage = ms.ToArray();
                    return byteImage;
                }
                catch (ArgumentNullException ex)
                {
                    throw ex;
                }
                finally
                {
                    ms.Close();
                }
            }
            var logo = new BitmapImage();
            logo.BeginInit();
            logo.StreamSource = new MemoryStream(BitmapToBytes(appLanguage == AppLanguage.Russian ? Resources.RuIcon : Resources.UKIcon));
            logo.EndInit();
            return logo;
        }
        public static string GetAntotherLocalizeByString(string value)
        {
            return Array.IndexOf(EnglishInterface, value) > -1 ? RussianInterface[Array.IndexOf(EnglishInterface,value)] : 
                EnglishInterface[Array.IndexOf(RussianInterface, value)];
        }
        public static string GetLocalize(TranslateText translateText,AppLanguage appLanguage)
        {
            return appLanguage == AppLanguage.English ? EnglishInterface[(int)translateText] : RussianInterface[(int)translateText];
        }
    }
}
