<h1>Welcome to Boostlingo.PeopleSoft! 🎉</h1>
<p>Please for the following programming exercise consider various failure scenarios that may result in the application halting unexpectedly, proceed to code defensively and handle any exceptions gracefully.</p>
<ol>
<li>Create a C# Console Application using net7 or above.</li>
<li>Read the below Json File directly from the specified URL.</li>
<li>https://microsoftedge.github.io/Demos/json-dummy-data/64KB.json</li>
<li>Read and parse the content of the file and insert it into a Database Table using the Relational Database of your choice.</li>
<li>Read the content of the Database Table in which the data has been stored and output its content to the Console (STDOUT) by Sorting by Person’s Last Name Then by First Name.</li>
<li>Submit your code solution to your personal GitHub page and share the link with us.</li>
<li>Prepare to discuss your solution with us.</li>
</ol>
<h2>🛠️ Installation Instructions</h2>
<ol>
<li>Clone this repository.</li>
<li>Navigate to the project path.</li>
<li>Run <code>dotnet restore</code> to install all dependencies.</li>
<li>Navigate to <i>Boostlingo.PeopleSoft/BoostLingo.PeopleSoft/</i> and edit the <b>appsettings.json</b> file.</li>
<li>Set the <code>DBConnection</code> value <code>data source=XXXXXXXXXXXXX;initial catalog=XXXXXXXXXXXXX;persist security info=True;user id=XXXXXXXXXXXXX;password=XXXXXXXXXXXXX;MultipleActiveResultSets=True;Encrypt=False;</code> to match your MS SQL server database. Make sure an empty database is available based on the connection string provided.</li>
<li>You may also modify the <code>Serilog > WriteTo > Path</code> value <code>C:\\Temp\\Log.txt</code> to customize the logging output directory.</li>
<li>When you run the application for the first time, the neccessary database tables will be created.</li>
<li>Navigate to <i>Boostlingo.PeopleSoft/BoostLingo.PeopleSoft/</i> and run <code>dotnet run</code> to start the application.</li>
</ol>
