   M S B A M L   `   `   `  ����^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers  5   	   P LCoding4Fun.Kinect.Wpf, Version=1.5.0.0, Culture=neutral, PublicKeyToken=nullnKclr-namespace:Coding4Fun.Kinect.Wpf.Controls;assembly=Coding4Fun.Kinect.WpfCoding4Fun.Kinect.Wpf.Controls 5   	    SkeletalTracking!   SkeletalTracking.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   mmy^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     5   	   `ControlsKclr-namespace:Coding4Fun.Kinect.Wpf.Controls;assembly=Coding4Fun.Kinect.Wpf     5   	   -   5          � Title$  
MainWindow��5   	   $	��600��6   $	��800��6)    � WindowState$ 	Maximized=�5   "   .��5      �� -   5        $��
MainCanvas��5      .+�5	   
   ;   5Microsoft.Samples.Kinect.WpfViewers.KinectColorViewer  -   5        $��kinectColorViewer1��5	   W    �� Left$ 0��6   
 �� Top$ 0��6/   $	��778��6>   $	��561��6J   5   /Microsoft.Samples.Kinect.WpfViewers.ImageViewer   Kinect 5
      ��  �� ElementName$ kinectSensorChooser1�� �� Path$ Kinect�A� 5   
   -   5        $��leftEllipse��5   A   $	 282��6   $	 168��6$   $��25��65   $��25��6U   
������ J� Opacity$ 1Q�6w   
�������6�   A� 5   
   -   5        $��rightEllipse��5   X   $	 451��6   $	 168��6$   
������$��25��6L   $��25��6l   $ 1Q�6w   
�������6�   =   7Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser  5   
   -   5        $��kinectSensorChooser1��5   D   $	 225��6!   $	 239��63   $	��328��6`   �� 5   
   -   5        $��	headImage��5   >   $	 333��6   $ 83��6"   $��87��62   $
��Fill=�6O   $��84��6^   $/��)/SkeletalTracking;component/c4f-color.png��6i   �� 5   
   -   5        $��
rectangle1��5   C   $	 636��6   $ 70��6&   $	��100��66   
���   �$	��130��6d   �� 5   
   -	   5        $��rectangleWXYZ��5   D   $	 636��6   $	 195��6&   $	��100��67   
���   �$	��130��6h   �� 5   
   -
   5        $��rectangleTUV��5   D   $	 636��6   $	 320��6&   $	��100��67   
���   �$	��130��6g   �� 5   
   -   5        $��rectanglePQRS��5   D   $	 506��6   $	 448��6&   $	��100��67   
���   �$	��130��6h   �� 5   
   -   5        $��rectangleMNO��5   D   $	 323��6   $	 448��6&   $	��100��67   
���   �$	��130��6g   �� 5   
   -   5        $��rectangleJKL��5   D   $	 141��6   $	 448��6&   $	��100��67   
���   �$	��130��6g   �� 5   
   -   5        $��rectangleGHI��5   C   $ 12��6   $	 320��6%   $	��100��66   
���   �$	��130��6f   �� 5   
   -   5        $��rectangleDEF��5   C   $ 12��6   $	 195��6%   $	��100��66   
���   �$	��130��6f   �� 5   
   -   5        $��rectangleABC��5   B   $ 12��6   $ 70��6%   $	��100��65   
���   �$	��130��6e   �� 5   
   -   5        $��
rectangle2��5   B   $	 141��6   $ 13��6&   $��42��66   
���   �$	��495��6c   �� 5   
   -   5        $��
textBlock1��5   B   $ 57��6   $	 110��6%   $��39��66   $	��ABC��6T   $��67��6_   0  *Coding4Fun.Kinect.Wpf.Controls.HoverButton  5      -   5          Name$ kinectButton��5   #   	   	ImageSize$		 100Q�69   
   ImageSource$$
 /Resources/RedButton-Hover.png��6I      ActiveImageSource$% /Resources/RedButton-Active.png��6w      IsTriggeredOnHover . $	 631��6�   $ 34��6�   5      5      