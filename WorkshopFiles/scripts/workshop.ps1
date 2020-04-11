
############
## TO RUN ONLY A SECTION OF CODE, SELECT THE LINES YOU WANT TO RUN AND PRESS F8
###########

# You can use this to test the application or you can use any web client like
# a browser, Postman, Fiddler, CUrl, etc.
$api = "https://localhost:5001/Configuration"
Invoke-WebRequest -URI $api