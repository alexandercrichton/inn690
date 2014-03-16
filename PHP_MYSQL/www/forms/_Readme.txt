
###########################################################################################
                                 EXTENDING THE FORM LIBRARY
###########################################################################################

If you would like to access a custom database in your custom object forms:
	1. Copy sql_knowledge.php and rename it sql_<your database name>.php.
	2. Update the database connection variables to point to your database
	3. Use require_once("sql_<your database name>.php") at the top of your php file 
	
	
Add a folder for your domain and put your pages in there.
When referencing them in a url make sure they are referenced as such:
http://<ip>:<port>/forms/<your_folder_name>/<page>.php

###########################################################################################
                       Potential extensions to functionality
###########################################################################################


get_object_functions.php
Determine which functions objects can perform based on its current state.
Eg. Can't perform certain functions if state is "OFF".

asset_service_page.php
Add a page to configure methods for assets.
Currently you can add a bunch of new assets, methods, etc...
But you can't associate assets with methods. 

