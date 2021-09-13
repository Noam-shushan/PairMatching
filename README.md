# PairMatching
Finds a match between students from Israel and students from around the world so that they can study together at parallel hours and according to their needs.
The source of the data is a Google Sheet that fills in after students fill out Google Forms.
The software is divided into 3 layers:

database is implemented in two ways:
- MongoDB in the Azure cloud
- saving files to disk in json format.

logic layer: 
- Reading the data from the Google Sheets.
- all the calculations and the matching.
- sending automatic and open email from the system.

display layer in WPF.

The program receives updates automatically from Azure clude.

![image](https://user-images.githubusercontent.com/40955004/133081795-c1e2cec0-fc1f-45fc-8f1c-2328a1ae0f49.png)

