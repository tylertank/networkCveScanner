# Network Cve Scanner
Network Cve Scanner is a tool that automatically fetches new vulnerabilities for a given list of software from the NIST API and updates a local CVE database.
It is part of a larger project called ReCVE that allows you to install daemons on windows and linux machines which will connect to this server and automatically collect your installed software data.

The server can be installed on a local machine with IIS and will host a local webserver to gain insight into how vulnerable the computers on your network are.

##Features
Retrieves unique software combinations (vendor, application, and version) from a local database.
Fetches new vulnerabilities for the specified software versions from the NIST API.
Updates the local CVE database with new vulnerabilities that are not already present.
Throttles API requests to respect the NIST API rate limits (50 requests per rolling 30-second window).
Handles concurrent API calls efficiently with separate instances of DbContext for each task.
