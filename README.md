
## Demonstration Video
https://youtu.be/OQNVCaxMzUI
# ReCVE
Is a local server that you can deploy to your network which will help you detect and visualize any CVEs on your network of computers. By installing the ReCVE deamon which will automatically connect to this server and start reporting installed software, cpu and memory usage and real time processes. You can then query the NIST CVE API to find any vulnerabilities on your network. If any vulnerbilities are found they will be stored and reported on the CVE page for you to see. 
## Dashboard
![recve](https://user-images.githubusercontent.com/7892014/235262211-8f6136f6-c6d4-40d7-aa70-4353e087c327.png)

## Network Cve View
![image](https://user-images.githubusercontent.com/7892014/235324139-c8593c28-336f-49ed-a9c7-96ef402c581b.png)

## Features
Retrieves unique software combinations (vendor, application, and version) from a local database.
Fetches new vulnerabilities for the specified software versions from the NIST API.
Updates the local CVE database with new vulnerabilities that are not already present.
Throttles API requests to respect the NIST API rate limits (50 requests per rolling 30-second window).
Handles concurrent API calls efficiently with separate instances of DbContext for each task.

## Realtime Computer Usage
![image](https://user-images.githubusercontent.com/7892014/235324015-9fce2629-06c7-498b-8539-caf03fc9806e.png)
You are able to see the realtime cpu and memory usage of any computer that is connected to the ReCVE server.
