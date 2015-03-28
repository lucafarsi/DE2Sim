README:
This file describes the operation of the DE2Sim tool.
This tool takes in MIF files and will simulate the behavior of the DE2 board aboard the DE2Bot.

HOW TO USE:
Open the application.
Use File->Open MIF File to select a MIF file to simulate.
The file contents will appear in the large text box, the PC will be set to wherever the first instruction is, which will 
be highlighted as the next instruction to be executed.
From here, you can either press the Step button to execute one instruction, or the Start button to begin the simulation.
The default rate at which instructions are executed is 100 Hz. This can be changed under the Simulate menu, , by selecting 
the Sim Speed... button and selecting one of the four options.
Once the simulation has started, it can be stopped by pressing the stop button.
The simulation can also be reset by pressing the Reset button, which will zero all values inside the simulation and set the 
PC back to the initial value.
flip switches operate by clicking on the, which will flip them.
Push buttons operate similarly to how they do one the normal DE2 board. Press down on the left mouse button to press them, 
and release the mouse button to release the push button.
Because there are no physical sensors, sensor input can be simulated by typing values into the corresponding sensor boxes. 
These values must be numbers, or the program will throw an error and the MIF file must be re-opened.
Battery functions are not simulated, and functions that attempt to retrieve battery level will just receive a normal battery 
level, which should not interfere with exection.
UART is also not simulated, as the simulator has no wireless capability. IN/OUT functions that have to do with UART will 
simply not return any result **THIS MEEANS THE ACCUMULATOR WILL HAVE THE SAME VALUE IT DID BEFORE AN IN FUNCTION FOR UART_DAT OR RDY**

CHANGELOG:
3/28/15: V1.0 initial release