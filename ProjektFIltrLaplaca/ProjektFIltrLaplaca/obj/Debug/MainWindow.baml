   M S B A M L   `   `   `  ����N  JProjektFIltrLaplaca, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:ProjektFIltrLaplacaProjektFIltrLaplaca  5   	   $    ProjektFIltrLaplaca.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   3local!clr-namespace:ProjektFIltrLaplaca     5   	     � Title$  Filtr Laplace'a��5   	   $	��800��6!   $
��1000��6.   .��5
      � #��� ��6   .+�5   
   �� $��Center=�6   $
��Wrap=�61   $��Filtr Laplace'a��6E   $	��Top=�6\   $��56��6t   $	��240��6�   $
��40px(�6�   �� 5   
   -   5         ��Name$ text_searchbox��5      $��Center=�6*   $��45��6G   $��36,67,172.6,0q�6S    �� TextWrapping$
 Wrap=�6j   $�� Select photo to filter��6~   $��23(�6�   $	��Top=�6�   $	��785��6�   �� 5   
   -   5         ��Name$ btn_openFolders��5      $
��OPEN��6*   $��23(�69   $��Right=�6G   $��	0,67,30,0q�6c   $	��Top=�6v   $��98��6�   $��45��6�   6�   �� 5   
   -   5        $ btn_filterCpp��5      $��
FILTER CPP��6(   $��23(�6=   $��Center=�6K   $	��Top=�6i   $	��140��6�   $��45��6�   $��36,131,817.6,0q�6�   
����+�� J� RenderTransformOrigin$ 0.494,1.609&�6�   �� 5   
   -   5        $ btn_filterAsm��5      $��
FILTER ASM��6(   $��23(�6=   $��Center=�6K   $��211,131,642.6,0q�6h   $	��Top=�6�   $	��140��6�   $��45��6�   $ 0.666,0.844&�6�   �� 5   
   -   5         ��Name$ image��5      $��Center=�6   $	��500��6<   $��Bottom=�6I   $	��700��6d   $��0,0,0,50q�6p   �� 5   
   $��Number of threads (1:64)��6   $
��Left=�63   $��368,121,0,0q�6N   $	��Top=�6c   $	��144��6{   $��27��6�   $��12(�6�   �� 5   
   -   5        $ btn_runAlgorythm��5      $	��RUN��6+   $��23(�6:   $
��Left=�6H   $��528,131,0,0q�6c   $	��Top=�6x   $��89��6�   $��45��6�   6�   �� 5   
   -   5        $ numbersOfThreads��5      $��Center=�6,   $��28��6I   $��410,146,522,0q�6U   $
 Wrap=�6l   $��1��6�   $	��Top=�6�    �� TextAlignment$ Center=�6�   $��18(�6�   $��61��6�   �� 5   
   -   5        $
 Time��5      $
��Left=�6    $��45��6;   $��654,131,0,0q�6G   $ NoWrap=�6\   $��Time: 
Ticks: ��6r   $	��Top=�6�   $	��160��6�   $
 left=�6�   �� 5   
   -	   5        $ btn_restart��5      $��RESTART��6&   $��23(�69   $��Center=�6G   $��45��6d   $��865,131,30,0q�6p   $	��Top=�6�   $��98��6�   6�   5      5      