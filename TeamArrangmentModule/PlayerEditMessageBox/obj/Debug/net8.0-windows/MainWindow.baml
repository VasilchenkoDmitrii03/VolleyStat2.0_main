   M S B A M L   `   `   `  ����O  KPlayerEditMessageBox, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:PlayerEditMessageBoxPlayerEditMessageBox  5   	   +    %PlayerEditMessageBox.PlayerEditMsgBox   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   4local"clr-namespace:PlayerEditMessageBox     5   	     � Title$  Player Edit��5   	   $	��200��6   $	��300��6*   $	��200��66   $	��200��6F   $	��300��6V   $	��300��6e   .��5	      �  � RowDefinitions 5
   
   �� 5      $��* �6   6)   �� 5      $��* �6   6)   �� 5      $��* �6   6)   �� 5      $��* �6   6)   �� 5      $��* �6   6)   �� 5      $��* �6   6)    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   6.   �� 5      $��* �6   6+   .+�5   
   �� $��0��6   $��0��6   .��6-   Number65   �� 5   
   $��1��6   $��0��6   .��6-   Amplua65   �� 5   
   $��2��6   $��0��6   .��6-   Name63   �� 5   
   $��3��6   $��0��6   .��6-   	Surname66   �� 5   
   $��4��6   $��0��6   .��6-   Height65   �� 5   
   $��0��6   $��1��6   ��6/   �� *Number+6I   �� 5   
   -   5        $��AmpluaCombobox��5      $��1��6)   $��1��66    �� SelectedValue 6F   �� *Amplua+6j   �� 5   
   $��2��6   $��1��6   ��6/   �� *PName+6H   �� 5   
   $��3��6   $��1��6   ��6/   �� *	Surname+6J   �� 5    
   $��4��6   $��1��6   ��6/   �� *	PHeight+6J   �� 5!   
   -   5        $��OkButton��5!   .   $��5��6   $��1��6   .��6U   Ok6Y   �� 5"   
   -   5        $��CancelButton��5"   .   $��5��6   $��0��6   .��6]   Cancel6e   5#      5$      