﻿#IncludeFile .\Init.inc

*Navigate Popup Menu Object
*Mouse RightButtonClick
 MoveMouseTo = 655x355
*SendKeys 
 Keys = Down;Return
*Sleep 2000
*HandleDialog  
 Caption = Go To Date
 Respond = Cancel
