   M S B A M L   `   `   `  ����H  DVisualSorting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:VisualSortingVisualSorting  5   	       VisualSorting.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:VisualSorting     5   	     � Title$  
MainWindow��5   	   $	��900��6   $
��1600��6)   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��auto �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      .+�5   
   �� $��0��6   $
��Left=�6"   $��Center=�6=    �� Orientation$ 
Horizontal=�6X   .+�5      �� 6   �� 5      $
��Sort��6   $
��50px��6$   $
��20px��61   ��6?   �� *SortCommand+5      �� 5      6   �� 5      $��Shuffle��6   $
��50px��6'   $
��20px��64   ��6B   �� *ShuffleCommand+5      �� 5      6   �� 5      $��Reset��6   $
��50px��6%   $
��20px��62   ��6@   �� *ResetCommand+5       �� 5"      6   �� 5$      $
��20px��6\   ��6   �� *Sorts+ �� SelectedIndex 65   �� *SelectedSort+5%      �� 5'       �� Padding$ 20q�6   5)      5*      �� 5,   
   -   5        $��GraphControl��5,      $��1��6M   
���   ���6+   �� *	RectItems+��5-      �� 5.      .R�5/      �� $��GraphCanvas��6   -   5        5/   0   50      ��52      �� 53       �� 
TargetType" ��6   .�54      ��  �� Property �@��Left �� Value 64   �� *X+�� 55       �@��Bottom 66   �� *Y+56      ��58      �� 59      .R�5:      �� $��1��6�   ��6    �� *Width+��68   �� *Height+��6R   �� *	FillColor+��6m   �� *	FillColor+6�   5;      5=      5>      5?      