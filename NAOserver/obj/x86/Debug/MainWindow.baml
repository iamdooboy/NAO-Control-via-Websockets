   M S B A M L   `   `   `  ����  	NAOserver    NAOserver.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$   NAO Control for PetFeeder ��5   	   $��271.284��6,   $	��420��6=    � 
ResizeMode$ CanMinimize=�6I    � Icon$ logo_NAO.ico��6�   .��5      � $	��234��6   $	��Top=�6    � RowDefinitions 5   
   �� 5      $
��186* �6   �� 5      $	��55* �6   .+�5
   
   �� -   5         ��Name$ connectButton��5
      $��Start��6(   $
��Left=�68   $��	43,86,0,0q�6S   $	��Top=�6f   $	��139��6~   $��70��6�   {�.  6�   �� 5   
   -   5        $ disconnectButton��5      $
��Stop��6+   $
��Left=�6:   $��
218,86,0,0q�6U   $	��Top=�6i   $	��139��6�   $��70��6�   6�   �� 5   
   -   5         ��Name$ ipBox��5      $
��Left=�6!   $��18��6<   $��
262,47,0,0q�6H    �� TextWrapping$
 Wrap=�6\   $��192.168.0.102��6p   $	��Top=�6�   $��95��6�    �� MaxLines$ 1��6�    �� 	MaxLength$ 15��6�   	 �� TextAlignment$	 Center=�6�   �� 5   
   -   5        $ portBox��5      $
��Left=�6#   $��18��6>   $��
307,15,0,0q�6J   $
 Wrap=�6^   $	��443��6r   $	��Top=�6}   $��50��6�   $ 4��6�   $ 1��6�   
 J� RenderTransformOrigin$
 
1.25,0.391&�6�   $	 Center=�6�   �� 5   
   -   5         ��Name$ 	portLabel��5      $��WebSocket Port:��6#   $
��Left=�6=   $��27��6X   $��	43,15,0,0q�6d   $	��Top=�6w   $��97��6�   �� 5   
   $��NAO IP:��6   $
��Left=�6"   $��	43,47,0,0q�6=   $	��Top=�6P   �� 5   
   -   5         ��Name$ logBox��5      $
��Left=�6$   $��
43,172,0,0q�6?   $
��Wrap=�6S   $	��314��6g   $	��...��6s   $
 0.5,0.5&�6~   $��2��6�   $��43��6�   $	��Top=�6�   5      5      