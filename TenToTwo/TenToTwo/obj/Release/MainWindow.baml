   M S B A M L   `   `   `  ����   NumSysConverter*    $NumericSystemConverterApp.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     � Title$  N.S Converter��$	��350��$	��544�� � 
ResizeMode$ CanMinimize=� � 	Resources  �� 
TargetType   
btTemplate �� CornerRadius  btCleanTemplate��%  &	        &	 �     �� " ��.R��� $
 17.5��
�������$	��1.5q�
���   �._��� $��ContentControl��$��Center=�$��Center=�$��Count��$	��100��$��45���� " ��.R��� $	 7.5��
�������$	��1.5q�
���   �._��� $��ContentControl��$��Center=�$��Center=�.���  � ColumnDefinitions �� $	��25* ��� $	��24* � � RowDefinitions �� �� �� �� �� �� ����  �� Color$ LightSlateGray�� �� Opacity$
 0.25Q�.+��� -   	 ��Name$	 Input��$��0��$��34(�$
 �� HorizontalScrollBarVisibility$

 Auto=� �� CharacterCasing$ Upper=�$	��1.5q�
�������$	��1.7q��� -    ��Name$
 Hint��$��0��$��Enter a number��$��30(�$	��1.8q����� $
 Gray���� -    ��Name$ LanguageSwitcher��$��0��$��1��$��209,10,10,15q�$
��Fill=�	� -    	�Name$ DecimalToBinary��$��1��$��Decimal to binary��$��24                      (�$��Center=�$��2��$��	0,0,254,0q�	� -   $ BinaryToDecimal��$��2��$��Binary to decimal��$��24                      (�$��Center=�$��2��$��	0,0,254,0q��� -    ��Name$ AnswerLabel��$��4��$��20(�$
��Wrap=�$��Center=� � ContextMenu �� .@�m� -   �� -	    ��Name$ CountButton��$��5��$��35(�#��[  $��	70,3,70,1q��� -
   $ FromText��$��3��$
��From��$��10,5,195,11q�$��24(�$��Right=��� -   $ ToLabel��$��3��$��To��$��119,5,107,5q�$��24(�$��Center=��� -   $ NsLabel��$��3��$��
207,5,5,11q�$	��N.s��$��24(�	� -   $ FromRadioButton��$��3��$��24(�$��	0,0,259,0q�$��Center=�$��44��$	��Top=�$��2���� -   $
	 From��$��3��$��24(�$��79,5,155,10q�$��Bottom=� �� 	MaxLength$ 2���� -   $	 To��$��3��$��24(�$��167,5,67,10q�$��Bottom=�$ 2���� -    ��Name$ History��$��1��$��4��$��1��$
��10,0q�$��2q�
������� �� SelectionMode$ Extended=� �� .@�m� -   �� -   $ HistoryBlock��$��2��$��0��$��History��$��39(�$��-4q�$��Center=��� -   $ ClearButton��$��5��$��-4q�$��18(�$��
71,5,71,20q�$��1��#��[ 