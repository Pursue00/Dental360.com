   M S B A M L   `   `   `  ����L  HDentistManagement, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:DentistManagement.ViewModelDentistManagement.ViewModel  5      )    #DentistManagement.View.ButtonListUC   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:DentistManagement.ViewModel     5        � DataContext  5	      >   8DentistManagement.ViewModel.WorkforceManagementViewModel  5
   
    � 	Resources 5         UserTemplate1 ��Name  ButtonStyle
 � Tag �� RelativeSource �� AncestorType �� Path �� ElementName��%(  &	        �� 5   
   .R�5      �� $ button1��6   $	��100��6&   $��50��62   |�.  #���  5   F   ��5   P   �� *
WorkTime+ 6m   �� *
WorkTime+��6�   ��  �� $ UserControlL�$&  DataContext.OnButtonClickCommand���5      �� $ button1��6F   5      5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   	 � ColumnDefinitions	 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��0q�6   $��0��6%   $��0��65   $��10q�6B   ._�5      �� -   5        
 ��Name$
 listBox��5      $��0q�6(   $��Hidden=�63   $��Verdana)�6g    � Cursor$
 Hand��6|   $��0q�5      
������ #��[  6k   &   System.Windows.Media.TextOptions   TextHintingMode$ Animated=�6�   $��Normal"�6�   ��6@   �� *CategoryButtonList+��6�   Z� ��5      �� 5       .R�5!      $�  $� Orientation$ 
Horizontal=�61   5"      5$      5%      5*      5+      