   M S B A M L   `   `   `  ����C  ?eTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:eTrackereTracker  5   	   E AMoonPdfLib, Version=0.3.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:MoonPdfLib;assembly=MoonPdfLib
MoonPdfLib 5   	       eTracker.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:eTracker     5   	   C
moonPdfLib,clr-namespace:MoonPdfLib;assembly=MoonPdfLib     5   	     � Title$  
MainWindow��5	   	   $	��350��6   $	��525��6)    � WindowStartupLocation$ CenterScreen=�65    � WindowState$ 	Maximized=�6Z   .��5
      �  � ColumnDefinitions 5   
   �� 5      6!   �� 5      $	��180 �6   6-    � RowDefinitions 5   
   �� 5      6   �� 5      $��20 �6   6*   .+�5   
   � -   5         �Name$ mainGrid��5      $��0��6!    5      �� 5      $��60 �6    6/   �� 5      6"   .+�5      ��  �� Orientation$ 
Horizontal=�6   $��0��62   .+�5      �� -   5         ��Name$ LoadFile��5      $��80��6+   .��6M   
Load PDF6W   �� 5      -   5        $ StartBtn��5      {�.  $��80��6=   .��6_   Start6f   �� 5      -   5        $ StopBtn��5      {�.  $��80��6<   .��6]   Stop6c   5      5      �� 5   
   $��0��6   $��1��6"   .+�5      �� -   5         ��Name$ translateTxt��5      {�.  6A   �� 5       -   5        $ btnTranslate��5       {�.  .��6X   	Translate6c   �� 5!      -   5        	 ��Name$	 translatedTxtBlock��5!      66   �� 5"      $��Center=�6   $
��Bold"�61   .��6C   
Synonyms6M   �� 5#      -   5        
 ��Name$
 synoLst��5#      6(   5$      �� 5&   
   $��1��6   $��1��6!   �� 5'      .�6   	Duration:6    �� 5(      -	   5         ��Name$ durationRun��5(      �6'   �� *CurrentTimeSpan+6J   5)      5+      5,      